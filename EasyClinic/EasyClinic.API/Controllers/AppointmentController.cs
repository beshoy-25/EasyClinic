using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Appointment;
using EasyClinic.API.DTOs.Doctor;
using EasyClinic.API.DTOs.DoctorService;
using EasyClinic.Domain.Entities;
using EasyClinic.API.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EasyClinic.API.Extensions;
using EasyClinic.Domain.Abstractions;
using EasyClinic.Domain.Errors;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.StaffOnly)]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ClinicSettings _settings;
        private readonly TimeProvider _timeProvider;

        public AppointmentController(
            AppDbContext context,
            IOptions<ClinicSettings> options,
            TimeProvider timeProvider)
        {
            _context = context;
            _settings = options.Value;
            _timeProvider = timeProvider;
        }
        private ObjectResult ProblemFrom(Error error) =>
                Problem(statusCode: error.Type.ToStatusCode(), title: error.Code, detail: error.Description);

        /// <summary>
        /// Retrieves all appointments.
        /// </summary>
        /// <param name="patientId">Optional. Returns only the appointments of this pation.</param>
        /// <param name="doctorServiceId">Optional. Returns only the appointments of this doctor.</param>
        /// <param name="appointmentDate">Optional. Returns only the appointments of this date.</param>
        /// <param name="appointmentStatusId">Optional. Returns only the appointments of this status.</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The list of appointments (empty if none match).</response>
        /// <response code="401">The user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType<IEnumerable<AppointmentDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAll(
            [FromQuery] int? patientId,
            [FromQuery] int? doctorServiceId,
            [FromQuery] int? appointmentStatusId,
            [FromQuery] DateTimeOffset? appointmentDate,
            CancellationToken cancellationToken)
        {

            var query = _context.Appointments.AsQueryable();

            if (patientId.HasValue)
                query = query.Where(a => a.PatientId == patientId.Value);

            if (doctorServiceId.HasValue)
                query = query.Where(a => a.DoctorServiceId == doctorServiceId.Value);

            if (appointmentStatusId.HasValue)
                query = query.Where(a => a.AppointmentStatusId == appointmentStatusId.Value);

            if (appointmentDate.HasValue)
            {
                var startOfDay = appointmentDate.Value.Date;
                var endOfDay = startOfDay.AddDays(1);

                query = query.Where(a => a.AppointmentDate >= startOfDay && a.AppointmentDate < endOfDay);
            }   

            var appointment = await query
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentStatusId = a.AppointmentStatusId,
                    AppointmentStatusName = a.AppointmentStatus.Name,
                    DoctorServiceId = a.DoctorServiceId,
                    DoctorName = a.DoctorService.Doctor.FullName,
                    MedicalServiceName = a.DoctorService.MedicalService.Name,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name,
                    TotalPrice = a.TotalPrice,
                    Notes = a.Notes
                }).ToListAsync(cancellationToken);

            return Ok(appointment);
        }

        /// <summary>
        /// Retrieves a appointment by id.
        /// </summary>
        /// <param name="id">The appointment id (1 or greater).</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The appointment was found.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="404">No appointment exists with this id.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType<AppointmentDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments
                .Where(a => a.Id == id)
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentStatusId = a.AppointmentStatusId,
                    AppointmentStatusName = a.AppointmentStatus.Name,
                    DoctorServiceId = a.DoctorServiceId,
                    DoctorName = a.DoctorService.Doctor.FullName,
                    MedicalServiceName = a.DoctorService.MedicalService.Name,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name,
                    TotalPrice = a.TotalPrice,
                    Notes = a.Notes
                }).FirstOrDefaultAsync(cancellationToken);

            if (appointment is null)
                return NotFound();

            return Ok(appointment);
        }

        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <param name="dto">The appointment data.</param>
        /// <response code="201">The appointment was created.</response>
        /// <response code="400">Invalid data: a date in the past, outside working hours, or an id that does not exist.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">The user does not have permission to create appointments.</response>
        /// <response code="409">The maximum number of appointments for that day has been reached.</response>
        [HttpPost]
        [Authorize(Policy = Policies.StaffOnly)]
        [Consumes("application/json")]
        [ProducesResponseType<AppointmentDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentDTO>> Create(CreateAppointmentDTO dto)
        {            
            var patientExist = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExist)
                return ProblemFrom(AppointmentErrors.PatientNotFound(dto.PatientId));

            var appointmentStatusExist = await _context.AppointmentStatuses.AnyAsync(d => d.Id == dto.AppointmentStatusId);
            if (!appointmentStatusExist)
                return ProblemFrom(AppointmentErrors.StatusNotFound(dto.AppointmentStatusId));

            // جوه الـ DoctorService، هنوصل لـ MedicalService.Price
            var doctorService = await _context.DoctorServices
                .Include(ds => ds.MedicalService)
                .Include(ds => ds.Doctor)
                .FirstOrDefaultAsync(ds => ds.Id == dto.DoctorServiceId);

            if (doctorService is null)
                return ProblemFrom(AppointmentErrors.DoctorServiceNotFound(dto.DoctorServiceId));

            // 2) قواعد الحجز نفسها (من غير Database) جوه الـ Entity
            var result = Appointment.Create(
                patientId: dto.PatientId,
                doctorServiceId: dto.DoctorServiceId,
                appointmentStatusId: dto.AppointmentStatusId,
                appointmentDate: dto.AppointmentDate,
                servicePrice: doctorService.MedicalService.Price,
                notes: dto.Notes,
                now: _timeProvider.GetUtcNow(),
                openingHour: _settings.OpeningHour,
                closingHour: _settings.ClosingHour);

            if (result.IsFailure)
                return ProblemFrom(result.Error);

            var appointment = result.Value;

            var appointmentsPerDay = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate.Date == dto.AppointmentDate.Date);

            if (appointmentsPerDay >= _settings.MaxAppointmentsPerDay)
                return ProblemFrom(AppointmentErrors.DailyLimitReached);

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var appointmentStatus = await _context.AppointmentStatuses.FindAsync(dto.AppointmentStatusId);
            var patient = await _context.Patients.FindAsync(dto.PatientId);

            var resultDto = new AppointmentDTO
            {
                Id = appointment.Id,
                AppointmentStatusId = appointment.AppointmentStatusId,
                AppointmentStatusName = appointmentStatus!.Name,
                DoctorServiceId = appointment.DoctorServiceId,
                DoctorName = doctorService.Doctor.FullName,
                MedicalServiceName = doctorService.MedicalService.Name,
                PatientId = appointment.PatientId,
                PatientName = patient!.Name,
                AppointmentDate = appointment.AppointmentDate,
                TotalPrice = appointment.TotalPrice,
                Notes = appointment.Notes
            };

            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, resultDto);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, CreateAppointmentDTO dto)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment is null)
                return NotFound();

            if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return ProblemFrom(AppointmentErrors.PatientNotFound(dto.PatientId));

            if (!await _context.AppointmentStatuses.AnyAsync(s => s.Id == dto.AppointmentStatusId))
                return ProblemFrom(AppointmentErrors.StatusNotFound(dto.AppointmentStatusId));

            var doctorService = await _context.DoctorServices
                .Include(ds => ds.MedicalService)
                .FirstOrDefaultAsync(ds => ds.Id == dto.DoctorServiceId);
            if (doctorService is null)
                return ProblemFrom(AppointmentErrors.DoctorServiceNotFound(dto.DoctorServiceId));

            // ليه الشرط ده؟ جاوب في آخر الرد
            if (dto.AppointmentDate != appointment.AppointmentDate)
            {
                var rescheduled = appointment.Reschedule(
                    dto.AppointmentDate, _timeProvider.GetUtcNow(),
                    _settings.OpeningHour, _settings.ClosingHour);

                if (rescheduled.IsFailure)
                    return ProblemFrom(rescheduled.Error);
            }

            if (dto.DoctorServiceId != appointment.DoctorServiceId)
                appointment.ChangeDoctorService(dto.DoctorServiceId, doctorService.MedicalService.Price);

            appointment.ChangePatient(dto.PatientId);
            appointment.ChangeStatus(dto.AppointmentStatusId);
            appointment.UpdateNotes(dto.Notes);

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPatch("{id:int:min(1)}")]
        public async Task<IActionResult> Patch(int id, PatchAppointmentDTO dto)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment is null)
                return NotFound();

            if (dto.PatientId.HasValue)
            {
                if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId.Value))
                    return ProblemFrom(AppointmentErrors.PatientNotFound(dto.PatientId.Value));

                appointment.ChangePatient(dto.PatientId.Value);
            }

            if (dto.AppointmentStatusId.HasValue)
            {
                if (!await _context.AppointmentStatuses.AnyAsync(s => s.Id == dto.AppointmentStatusId.Value))
                    return ProblemFrom(AppointmentErrors.StatusNotFound(dto.AppointmentStatusId.Value));

                appointment.ChangeStatus(dto.AppointmentStatusId.Value);
            }

            if (dto.DoctorServiceId.HasValue && dto.DoctorServiceId.Value != appointment.DoctorServiceId)
            {
                var doctorService = await _context.DoctorServices
                    .Include(ds => ds.MedicalService)
                    .FirstOrDefaultAsync(ds => ds.Id == dto.DoctorServiceId.Value);

                if (doctorService is null)
                    return ProblemFrom(AppointmentErrors.DoctorServiceNotFound(dto.DoctorServiceId.Value));

                appointment.ChangeDoctorService(doctorService.Id, doctorService.MedicalService.Price);
            }

            if (dto.AppointmentDate.HasValue && dto.AppointmentDate.Value != appointment.AppointmentDate)
            {
                var rescheduled = appointment.Reschedule(
                    dto.AppointmentDate.Value, _timeProvider.GetUtcNow(),
                    _settings.OpeningHour, _settings.ClosingHour);

                if (rescheduled.IsFailure)
                    return ProblemFrom(rescheduled.Error);
            }

            if (dto.Notes is not null)
                appointment.UpdateNotes(dto.Notes);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment is null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
