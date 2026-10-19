//using EasyClinic.API.Data;
//using EasyClinic.API.DTOs.Specializations;
//using EasyClinic.API.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace EasyClinic.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class SpecializationsController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        public SpecializationsController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<SpecializationDTO>>> GetAll(CancellationToken cancellationToken)
//        {
//            var specializations = await _context.Specializations.
//                Select(s => new SpecializationDTO
//                {
//                    Id = s.Id,
//                    Name = s.Name,
//                })
//                .ToListAsync(cancellationToken);

//            return Ok(specializations);
//        }

//        [HttpGet("{id:int:min(1)}")]
//        public async Task<ActionResult<SpecializationDTO>> GetById(int id, CancellationToken cancellationToken) {
//            var specialization = await _context.Specializations
//                .Where(s => s.Id == id)
//                .Select(s => new SpecializationDTO
//                {
//                    Id = s.Id,
//                    Name = s.Name,
//                }).FirstOrDefaultAsync(cancellationToken);

//            if (specialization is null)
//                return NotFound();

//            return Ok(specialization);
//        }
//        [HttpPost]
//        public async Task<ActionResult<SpecializationDTO>> Create(CreateSpecializationDTO dto)
//        {
//            var specialization = new Specialization
//            {
//                Name = dto.Name
//            };

//            _context.Specializations.Add(specialization);
//            await _context.SaveChangesAsync();

//            var resultDto = new SpecializationDTO
//            {
//                Id = specialization.Id,
//                Name = specialization.Name,
//            };
//            return CreatedAtAction(nameof(GetById), new { id = specialization.Id }, resultDto);
//        }

//        [HttpPut("{id:int:min(1)}")]
//        public async Task<IActionResult> Update(int id, CreateSpecializationDTO dto)
//        {
//            var specialization = await _context.Specializations.FindAsync(id);

//            if (specialization is null)
//                return NotFound();

//            specialization.Name = dto.Name;

//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        [HttpDelete("{id:int:min(1)}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var specialization = await _context.Specializations.FindAsync(id);

//            if (specialization is null)
//                return NotFound();

//            var hasDoctors = await _context.Doctors.AnyAsync(d => d.SpecializationId == id);
//            if (hasDoctors)
//                return BadRequest("Cannot delete this specialization because it has doctors linked to it.");

//            _context.Specializations.Remove(specialization);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//    }
//}
