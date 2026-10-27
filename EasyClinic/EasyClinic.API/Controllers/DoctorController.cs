using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Doctor;
using EasyClinic.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel;
using System.Security;
using System.Threading.Tasks;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    [Tags("Doctors")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all doctors.
        /// </summary>
        /// <param name="specializationId">Optional. Returns only the doctors of this specialization.</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The list of doctors (empty if none match).</response>
        /// <response code="401">The user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType<IEnumerable<DoctorDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]        
        //[EndpointName("GetDoctors")]
        //[EndpointSummary("Retrieves all Doctors")] // relate with OpenAPI way
        //[EndpointDescription("Retrieves all Doctors owned or accessible by the user and Filtering by specializationId.")]
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

        /// <summary>
        /// Retrieves a doctor by id.
        /// </summary>
        /// <param name="id">The doctor id (1 or greater).</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The doctor was found.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="404">No doctor exists with this id.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType<DoctorDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        //[EndpointName("GetDoctor")]
        //[EndpointSummary("Retrieves Doctor by Id")] // relate with OpenAPI way
        //[EndpointDescription("Retrieves specefic Doctor by id owned or accessible by the user.")]        
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
        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        /// <param name="dto">The doctor data.</param>
        /// <response code="201">The doctor was created.</response>
        /// <response code="400">Invalid data, or the specialization does not exist.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">Only admins can create doctors.</response>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]        
        [Consumes("application/json")]
        [ProducesResponseType<DoctorDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        //[EndpointName("CreateDoctor")]
        //[EndpointSummary("Creates a new doctor")] // relate with OpenAPI way
        //[EndpointDescription("Creates a new doctor for the current user and returns the created result.")]
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
