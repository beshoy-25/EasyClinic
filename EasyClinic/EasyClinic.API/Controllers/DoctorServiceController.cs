using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.DoctorService;
using EasyClinic.API.Models;
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

        [HttpGet]
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
        [HttpGet("{id:int:min(1)}")]
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
                return NotFound();


            return Ok(doctorService);
            
        }

        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<DoctorServiceDTO>> Create(CreateDoctorServiceDTO dto)
        {
            var doctorExist = await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if(!doctorExist)
                return Problem(detail: $"Doctor with id {dto.DoctorId} does not exist.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid Doctor");

            var serviceExist = await _context.MedicalServices.AnyAsync(d => d.Id == dto.MedicalServiceId);
            if(!serviceExist)
                return Problem(detail: $"MedicalService with id {dto.MedicalServiceId} does not exist.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid MedicalService");

            var alreadyExists = await _context.DoctorServices
                .AnyAsync(ds => ds.DoctorId == dto.DoctorId && ds.MedicalServiceId == dto.MedicalServiceId);

            if (alreadyExists)
                return Problem(detail: "This doctor is already linked to this medical service.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid linking");

            var doctorService = new DoctorService
            {
                DoctorId = dto.DoctorId,
                MedicalServiceId = dto.MedicalServiceId
            };

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
                return NotFound();

            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists)
                return Problem(detail: $"Doctor with id {dto.DoctorId} does not exist.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid Doctor");

            var serviceExists = await _context.MedicalServices.AnyAsync(m => m.Id == dto.MedicalServiceId);
            if (!serviceExists)
                return Problem(detail: $"MedicalService with id {dto.MedicalServiceId} does not exist.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid MedicalService");

            var duplicateExists = await _context.DoctorServices
                .AnyAsync(ds => ds.Id != id && ds.DoctorId == dto.DoctorId && ds.MedicalServiceId == dto.MedicalServiceId);
            if (duplicateExists)
                return Problem(detail: "This doctor is already linked to this medical service.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid linking");

            doctorService.DoctorId = dto.DoctorId;
            doctorService.MedicalServiceId = dto.MedicalServiceId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id)
        {
            var doctorService = await _context.DoctorServices.FindAsync(id);

            if (doctorService is null)
                return NotFound();

            var hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorServiceId == id);
            if (hasAppointments)                
                return Problem(detail: "Cannot delete this doctor-service link because it has appointments linked to it.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid Deleting");
            _context.DoctorServices.Remove(doctorService);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
