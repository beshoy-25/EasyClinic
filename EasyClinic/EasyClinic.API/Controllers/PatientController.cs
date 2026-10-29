using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Doctor;
using EasyClinic.API.DTOs.Patient;
using EasyClinic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.StaffOnly)]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all Patient.
        /// </summary>        
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The list of Patients (empty if none match).</response>
        /// <response code="401">The user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType<IEnumerable<PatientDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll(CancellationToken cancellationToken)
        {            
            var patient = await _context.Patients.Select(a => new PatientDto
            {
                Id = a.Id,
                DateOfBirth = a.DateOfBirth,
                Name = a.Name,
                Gender = a.Gender.ToString(),
                MedicalHistory = a.MedicalHistory,
                PhoneNumber = a.PhoneNumber
            }).ToListAsync(cancellationToken);


            return Ok(patient);
        }

        /// <summary>
        /// Retrieves a Patient by id.
        /// </summary>
        /// <param name="id">The Patient id (1 or greater).</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The Patient was found.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="404">No Patient exists with this id.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType<PatientDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDto>> GetById(int id, CancellationToken cancellationToken)
        {            
            var patient = await _context.Patients
                .Where(a => a.Id == id)
                .Select(a => new PatientDto
                {
                    Id = a.Id,
                    DateOfBirth = a.DateOfBirth,
                    Name = a.Name,
                    Gender = a.Gender.ToString(),
                    MedicalHistory = a.MedicalHistory,
                    PhoneNumber= a.PhoneNumber
                }).FirstOrDefaultAsync(cancellationToken);

            if(patient is null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        /// <summary>
        /// Creates a new Patient.
        /// </summary>
        /// <param name="dto">The Patient data.</param>
        /// <response code="201">The Patient was created.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">Only admins can create Patients.</response>
        [HttpPost]        
        [Consumes("application/json")]
        [ProducesResponseType<PatientDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PatientDto>> Create(CreatePatientDTO dto)
        {
            if(!Enum.TryParse<Gender>(dto.Gender,out var result))
            {
                return Problem(detail: "Invalid gender value. Must be 'Male' or 'Femal'.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid value");
            }

            var patient = new Patient
            {
                Name = dto.Name,
                DateOfBirth = dto.DateOfBirth,
                Gender = result,
                MedicalHistory = dto.MedicalHistory,
                PhoneNumber = dto.PhoneNumber
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var resultDto = new PatientDto
            {
                Id = patient.Id,
                DateOfBirth = patient.DateOfBirth,
                Name = patient.Name,
                Gender = dto.Gender,
                PhoneNumber = patient.PhoneNumber,
                MedicalHistory = patient.MedicalHistory
            };


            return CreatedAtAction(nameof(GetById), new {id = patient.Id}, resultDto);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, CreatePatientDTO dto)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient is null)
                return NotFound();

            if (!Enum.TryParse<Gender>(dto.Gender, out var result))
            {
                return Problem(detail: "Invalid gender value. Must be 'Male' or 'Femal'.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid value");
            }
            // عدّل كل الخصائص من الـ dto
            patient.Name = dto.Name;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Gender = result;
            patient.MedicalHistory = dto.MedicalHistory;
            patient.PhoneNumber = dto.PhoneNumber;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient is null)
                return NotFound();

            var hasAppointments = await _context.Appointments.AnyAsync(a => a.PatientId == id);
            if (hasAppointments)                
            return Problem(detail: "Cannot delete this patient because they have appointments linked to them.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid Deleting");
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
