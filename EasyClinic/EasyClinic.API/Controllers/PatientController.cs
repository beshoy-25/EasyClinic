using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Patient;
using EasyClinic.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
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
        
        [HttpGet("{id:int:min(1)}")]
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
        
        [HttpPost]
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
