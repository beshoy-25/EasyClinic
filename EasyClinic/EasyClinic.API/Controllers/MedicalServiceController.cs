using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Doctor;
using EasyClinic.API.DTOs.MedicalService;
using EasyClinic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EasyClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalServiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MedicalServiceController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all MedicalServices.
        /// </summary>        
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The list of Medical Services (empty if none match).</response>
        /// <response code="401">The user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType<IEnumerable<MedicalServiceDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MedicalServiceDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var medicalService = await _context.MedicalServices
                .Select(m => new MedicalServiceDTO
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    DurationInMinutes = m.DurationInMinutes,
                    Price = m.Price
                }).ToListAsync(cancellationToken);
            return Ok(medicalService);
        }
        /// <summary>
        /// Retrieves a Medical Service by id.
        /// </summary>
        /// <param name="id">The Medical Service id (1 or greater).</param>
        /// <param name="cancellationToken">Cancels the query if the client disconnects.</param>
        /// <response code="200">The Medical Service was found.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="404">No Medical Service exists with this id.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType<MedicalServiceDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedicalServiceDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var medicalService = await _context.MedicalServices
                .Where(m => m.Id == id)
                .Select(m => new MedicalServiceDTO
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    DurationInMinutes = m.DurationInMinutes,
                    Price = m.Price
                }).FirstOrDefaultAsync(cancellationToken);
            if (medicalService is null)
                return NotFound();


            return Ok(medicalService);
        }
        /// <summary>
        /// Creates a new Medical Service.
        /// </summary>
        /// <param name="dto">The Medical Service data.</param>
        /// <response code="201">The Medical Service was created.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">The user is not authenticated.</response>
        /// <response code="403">Only admins can create MedicalServices.</response>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        [Consumes("application/json")]
        [ProducesResponseType<MedicalServiceDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MedicalServiceDTO>> Create(CreateMedicalServiceDTO dto)
        {
            var medicalService = new MedicalService
            {
                Name = dto.Name,
                Description = dto.Description,
                DurationInMinutes = dto.DurationInMinutes,
                Price = dto.Price
            };
            _context.MedicalServices.Add(medicalService);
            await _context.SaveChangesAsync();

            var resultDto = new MedicalServiceDTO
            {
                Id = medicalService.Id,
                Price = medicalService.Price,
                Name = medicalService.Name,
                Description = medicalService.Description,
                DurationInMinutes = medicalService.DurationInMinutes
            };
            return CreatedAtAction(nameof(GetById), new { id = medicalService.Id }, resultDto);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(int id, CreateMedicalServiceDTO dto)
        {
            var medicalService = await _context.MedicalServices.FindAsync(id);

            if (medicalService is null)
                return NotFound();

            medicalService.Name = dto.Name;
            medicalService.Description = dto.Description;
            medicalService.Price = dto.Price;
            medicalService.DurationInMinutes = dto.DurationInMinutes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id)
        {
            var medicalService = await _context.MedicalServices.FindAsync(id);

            if (medicalService is null)
                return NotFound();

            var hasDoctorServices = await _context.DoctorServices.AnyAsync(ds => ds.MedicalServiceId == id);
            if (hasDoctorServices)                
                return Problem(detail: "Cannot delete this medical service because it has doctors linked to it.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid Deleting");
            _context.MedicalServices.Remove(medicalService);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
