using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Doctor;
using EasyClinic.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Threading.Tasks;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetAll(
            [FromQuery] int? specializationId,CancellationToken cancellationToken)
        {

            var query = _context.Doctors.AsQueryable();

            if (specializationId.HasValue)
            {
                query = query.Where(d => d.SpecializationId == specializationId.Value);
            }

            var doctor = await query.Select(d => new DoctorDTO
            {
                Id = d.Id,
                Email = d.Email,
                FullName = d.FullName,
                PhoneNumber = d.PhoneNumber,
                SpecializationId = d.SpecializationId,
                SpecializationName = d.Specialization.Name          
            }).ToListAsync(cancellationToken);

            return Ok(doctor);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<DoctorDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var doctor = await _context.Doctors
                .Where(d =>  d.Id == id)
                .Select(d => new DoctorDTO
                {
                    Id = d.Id,
                    Email = d.Email,
                    FullName = d.FullName,
                    PhoneNumber = d.PhoneNumber,
                    SpecializationId = d.SpecializationId,
                    SpecializationName = d.Specialization.Name
                }).FirstOrDefaultAsync(cancellationToken);

            if (doctor is null)
                return NotFound();


            return Ok(doctor);
        }

        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<DoctorDTO>> Create(CreateDoctorDTO dto)
        {
            var specializationExist = await _context.Specializations
                    .AnyAsync(d => d.Id == dto.SpecializationId);

            if(!specializationExist)
                return Problem(
                    detail: $"Specialization with id {dto.SpecializationId} does not exist.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Specialization"
                    ); 

            var doctor = new Doctor
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                SpecializationId = dto.SpecializationId
            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var specialization = await _context.Specializations.FindAsync(dto.SpecializationId);

            var resultDto = new DoctorDTO
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                SpecializationId= doctor.SpecializationId,
                SpecializationName = specialization!.Name,                
            };

            return CreatedAtAction(nameof(GetById),new {id = doctor.Id},resultDto);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(int id, CreateDoctorDTO dto)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor is null)
                return NotFound();

            var specializationExists = await _context.Specializations.AnyAsync(s => s.Id == dto.SpecializationId);
            if (!specializationExists)                            
                return Problem(
                    detail: $"Specialization with id {dto.SpecializationId} does not exist.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Specialization"
                    );

            doctor.FullName = dto.FullName;
            doctor.PhoneNumber = dto.PhoneNumber;
            doctor.Email = dto.Email;
            doctor.SpecializationId = dto.SpecializationId;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Patch(int id, PatchDoctorDto dto)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if(doctor is null) 
                return NotFound();

            if (dto.SpecializationId.HasValue)
            {
                var specializationExists = await _context.Specializations.AnyAsync(s => s.Id == dto.SpecializationId);
                if (!specializationExists)
                    return Problem(
                        detail: $"Specialization with id {dto.SpecializationId} does not exist.",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid Specialization");

                doctor.SpecializationId = dto.SpecializationId.Value;
            }

            if (dto.FullName is not null)
            {
                doctor.FullName = dto.FullName;
            }

            if(dto.PhoneNumber is not null)
            {
                doctor.PhoneNumber = dto.PhoneNumber;
            }

            if(dto.Email is not null)
            {
                doctor.Email = dto.Email;
            }

           
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor is null)
                return NotFound();

            var hasDoctorServices = await _context.DoctorServices.AnyAsync(ds => ds.DoctorId == id);
            if (hasDoctorServices)                
                    return Problem(
                        detail: "Cannot delete this doctor because they have services linked to them.",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid Specialization"
                        ); 

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
