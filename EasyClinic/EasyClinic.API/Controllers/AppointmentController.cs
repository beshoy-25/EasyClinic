using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Appointment;
using EasyClinic.API.DTOs.DoctorService;
using EasyClinic.API.Models;
using EasyClinic.API.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ClinicSettings _settings;

        public AppointmentController(AppDbContext context, IOptions<ClinicSettings> options)
        {
            _context = context;
            _settings = options.Value;
        }
        [HttpGet]
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

        [HttpGet("{id:int:min(1)}")]
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

        [HttpPost]
        public async Task<ActionResult<AppointmentDTO>> Create(CreateAppointmentDTO dto)
        {
            //if (dto.AppointmentDate < DateTimeOffset.Now)
            //    return Problem(
            //         detail: $"Appointments must be in the future.",
            //         statusCode: StatusCodes.Status400BadRequest,
            //         title: "Invalid Appointment Date"
            //     );

            //var hour = dto.AppointmentDate.Hour;
            //if (hour < _settings.OpeningHour || hour >= _settings.ClosingHour)
            //    return Problem(
            //        detail: $"Appointments must be between {_settings.OpeningHour}:00 and {_settings.ClosingHour}:00.",
            //        statusCode: StatusCodes.Status400BadRequest,
            //        title: "Invalid Appointment Time"
            //    );

            if (_context.Appointments.Any(a => a.AppointmentDate == dto.AppointmentDate))
                return Problem(
                    detail: "We are very sorry, This appoint was booked . please book another appoint.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Booking"); 

            var AppointmentsPerDay = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date ==  dto.AppointmentDate.Date);
            if (AppointmentsPerDay >= _settings.MaxAppointmentsPerDay)
                return Problem(
                  detail: $"We are very sorry, we have reached the maximum appointments per day. please book another day.",
                  statusCode: StatusCodes.Status400BadRequest,
                  title: "Invalid Booking"); 
            
            var patientExist = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExist)
                return Problem(
                    detail: $"Patient with id {dto.PatientId} does not exist.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Ptient"); 

            var appointmentStatusExist = await _context.AppointmentStatuses.AnyAsync(d => d.Id == dto.AppointmentStatusId);
            if (!appointmentStatusExist)
                return Problem(
                    detail: $"AppointmentStatus with id {dto.AppointmentStatusId} does not exist.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Booking"); 
            
            // جوه الـ DoctorService، هنوصل لـ MedicalService.Price
            var doctorService = await _context.DoctorServices
                .Include(ds => ds.MedicalService)
                .Include(ds => ds.Doctor)
                .FirstOrDefaultAsync(ds => ds.Id == dto.DoctorServiceId);

            if (doctorService is null)
                return Problem(
                    detail: $"DoctorService with id {dto.DoctorServiceId} does not exist.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Booking"); 

            decimal totalPrice = doctorService.MedicalService.Price;   // السيرفر بيحسبها بنفسه

            var appointment = new Appointment
            {
                AppointmentStatusId = dto.AppointmentStatusId,
                DoctorServiceId = dto.DoctorServiceId,
                PatientId= dto.PatientId,
                AppointmentDate = dto.AppointmentDate,
                Notes = dto.Notes  ,
                TotalPrice = totalPrice
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var appointmentStatus = await _context.AppointmentStatuses.FindAsync(dto.AppointmentStatusId);
            //var findDoctorService = await _context.DoctorServices.FindAsync(dto.DoctorServiceId);
            var patient = await _context.Patients.FindAsync(dto.PatientId);

            var resultDto = new AppointmentDTO
            {
                Id = appointment.Id,               
                AppointmentStatusId = appointment.AppointmentStatusId,
                AppointmentStatusName = appointmentStatus!.Name,
                DoctorServiceId = appointment.DoctorServiceId,
                DoctorName = doctorService!.Doctor.FullName,               
                MedicalServiceName = doctorService!.MedicalService.Name,
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

            var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists)
                return Problem(
                    detail: $"Patient with id {dto.PatientId} does not exist.", 
                    statusCode: StatusCodes.Status400BadRequest, 
                    title: "Invalid Patient");

            var appointmentStatusExists = await _context.AppointmentStatuses.AnyAsync(s => s.Id == dto.AppointmentStatusId);
            if (!appointmentStatusExists)
                return Problem(
                    detail: $"AppointmentStatus with id {dto.AppointmentStatusId} does not exist.", 
                    statusCode: StatusCodes.Status400BadRequest, 
                    title: "Invalid AppointmentStatus");

            var doctorService = await _context.DoctorServices
                .Include(ds => ds.MedicalService)
                .FirstOrDefaultAsync(ds => ds.Id == dto.DoctorServiceId);

            if (doctorService is null)
                return Problem(
                    detail: $"DoctorService with id {dto.DoctorServiceId} does not exist.",
                    statusCode: StatusCodes.Status400BadRequest, 
                    title: "Invalid DoctorService");

            // نحسب السعر الجديد بس لو الـ DoctorService اتغيّر فعلاً
            if (appointment.DoctorServiceId != dto.DoctorServiceId)
            {
                appointment.TotalPrice = doctorService.MedicalService.Price;
            }

            appointment.PatientId = dto.PatientId;
            appointment.DoctorServiceId = dto.DoctorServiceId;
            appointment.AppointmentStatusId = dto.AppointmentStatusId;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.Notes = dto.Notes;

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
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId.Value);
                if (!patientExists)
                    return Problem(
                        detail: $"Patient with id {dto.PatientId} does not exist.",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid Patient");

                appointment.PatientId = dto.PatientId.Value;
            }

            if (dto.AppointmentStatusId.HasValue)
            {
                var statusExists = await _context.AppointmentStatuses.AnyAsync(s => s.Id == dto.AppointmentStatusId.Value);
                if (!statusExists)
                    return Problem(
                        detail: $"AppointmentStatus with id {dto.AppointmentStatusId} does not exist.",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid AppointmentStatus");

                appointment.AppointmentStatusId = dto.AppointmentStatusId.Value;
            }

            if (dto.DoctorServiceId.HasValue)
            {
                var doctorService = await _context.DoctorServices
                    .Include(ds => ds.MedicalService)
                    .FirstOrDefaultAsync(ds => ds.Id == dto.DoctorServiceId.Value);

                if (doctorService is null)
                    return Problem(
                        detail: $"DoctorService with id {dto.DoctorServiceId} does not exist.",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid DoctorService");

                appointment.DoctorServiceId = dto.DoctorServiceId.Value;
                appointment.TotalPrice = doctorService.MedicalService.Price;   // فتكر: بس لو الخدمة اتغيرت فعلاً
            }

            if (dto.AppointmentDate.HasValue)
            {
                appointment.AppointmentDate = dto.AppointmentDate.Value;
            }

            if (dto.Notes is not null)
            {
                appointment.Notes = dto.Notes;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
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
