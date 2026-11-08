using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Doctor;
using EasyClinic.API.DTOs.DoctorService;
using EasyClinic.API.Extensions;
using EasyClinic.Domain.Entities;
using EasyClinic.Domain.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorServiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DoctorServiceController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all Doctor Services.
        /// </summary>
        /// <param name="doctorId">Optional. Returns only the Doctor Services of this doctor.</param>
        /// <param name="medicalServiceId">Optional. Returns only the Doctor Services of this medical Service.</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The list of Doctor Services (empty if none match).</response>
        /// <response code="401">The user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType<IEnumerable<DoctorServiceDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<DoctorServiceDTO>>> GetAll(
            [FromQuery] int? doctorId,
            [FromQuery] int? medicalServiceId,
            CancellationToken cancellationToken)
        {
            var query = _context.DoctorServices.AsQueryable();

            if (doctorId.HasValue)
            {
                query = query.Where(ds => ds.DoctorId == doctorId.Value);
            }

            if (medicalServiceId.HasValue)
            {
                query = query.Where(ds => ds.MedicalServiceId == medicalServiceId.Value);
            }

            var doctorService = await query
                .Select(ds => new DoctorServiceDTO
                {
                    Id = ds.Id,
                    DoctorId = ds.DoctorId,
                    DoctorName = ds.Doctor.FullName,
                    MedicalServiceId = ds.MedicalServiceId,
                    MedicalServiceName = ds.MedicalService.Name
                }).ToListAsync(cancellationToken);

            return Ok(doctorService);
        }
        /// <summary>
        /// Retrieves a Doctor Service by id.
        /// </summary>
        /// <param name="id">The Doctor Service id (1 or greater).</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The Doctor Service was found.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="404">No Doctor Service exists with this id.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType<DoctorServiceDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorServiceDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var doctorService = await _context.DoctorServices
                .Where(ds => ds.Id == id)
                .Select(ds => new DoctorServiceDTO
                {
                    Id = ds.Id,
                    DoctorId = ds.DoctorId,
                    DoctorName = ds.Doctor.FullName,
                    MedicalServiceId = ds.MedicalServiceId,
                    MedicalServiceName = ds.MedicalService.Name
                }).FirstOrDefaultAsync(cancellationToken);

            if (doctorService is null)
                return this.ToProblem(DoctorServiceErrors.NotFound(id));


            return Ok(doctorService);
            
        }

        /// <summary>
        /// Creates a new Doctor Service.
        /// </summary>
        /// <param name="dto">The Doctor Service data.</param>
        /// <response code="201">The Doctor Service was created.</response>
        /// <response code="400">Invalid data, or the specialization does not exist.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">Only admins can create Doctor Service.</response>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        [Consumes("application/json")]
        [ProducesResponseType<DoctorServiceDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DoctorServiceDTO>> Create(CreateDoctorServiceDTO dto)
        {
            if (!await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId))
                return this.ToProblem(DoctorServiceErrors.DoctorNotFound(dto.DoctorId));

            if (!await _context.MedicalServices.AnyAsync(m => m.Id == dto.MedicalServiceId))
                return this.ToProblem(DoctorServiceErrors.MedicalServiceNotFound(dto.MedicalServiceId));

            if (await _context.DoctorServices.AnyAsync(ds =>
                        ds.DoctorId == dto.DoctorId && ds.MedicalServiceId == dto.MedicalServiceId))
                return this.ToProblem(DoctorServiceErrors.AlreadyExists);

            var doctorService = DoctorService.Create(dto.DoctorId, dto.MedicalServiceId);

            _context.DoctorServices.Add(doctorService);
            await _context.SaveChangesAsync();

            var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
            var medicalServices = await _context.MedicalServices.FindAsync(dto.MedicalServiceId);

            var resultDto = new DoctorServiceDTO
            {
                Id = doctorService.Id,
                DoctorId = doctorService.DoctorId,
                DoctorName = doctor!.FullName,
                MedicalServiceId = doctorService.MedicalServiceId,
                MedicalServiceName = medicalServices!.Name
            };

            return CreatedAtAction(nameof(GetById),new {id = doctorService.Id},resultDto);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(int id, CreateDoctorServiceDTO dto)
        {
            var doctorService = await _context.DoctorServices.FindAsync(id);

            if (doctorService is null)
                return this.ToProblem(DoctorServiceErrors.NotFound(id));
            
            var changesLink = dto.DoctorId != doctorService.DoctorId
                           || dto.MedicalServiceId != doctorService.MedicalServiceId;

            if (changesLink && await _context.Appointments.AnyAsync(a => a.DoctorServiceId == id))
                return this.ToProblem(DoctorServiceErrors.HasAppointments);

            if (!await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId))
                return this.ToProblem(DoctorServiceErrors.DoctorNotFound(dto.DoctorId));

            if (!await _context.MedicalServices.AnyAsync(m => m.Id == dto.MedicalServiceId))
                return this.ToProblem(DoctorServiceErrors.MedicalServiceNotFound(dto.MedicalServiceId));

            if (await _context.DoctorServices.AnyAsync(ds =>
            ds.DoctorId == dto.DoctorId && ds.MedicalServiceId == dto.MedicalServiceId))
                return this.ToProblem(DoctorServiceErrors.AlreadyExists);

            doctorService.Update(dto.DoctorId, dto.MedicalServiceId);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id)
        {
            var doctorService = await _context.DoctorServices.FindAsync(id);

            if (doctorService is null)
                return this.ToProblem(DoctorServiceErrors.NotFound(id));

            var hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorServiceId == id);
            if (hasAppointments)
                return this.ToProblem(DoctorServiceErrors.HasAppointments);
            _context.DoctorServices.Remove(doctorService);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
