//using EasyClinic.API.Data;
//using EasyClinic.API.DTOs.AppointmentStatus;
//using EasyClinic.Domain.Entities;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace EasyClinic.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AppointmentStatusController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        public AppointmentStatusController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<AppointmentStatusDTO>>> GetAll(CancellationToken cancellationToken)
//        {
//            var appointmentStatus = await _context.AppointmentStatuses
//            .Select(a => new AppointmentStatusDTO 
//            {
//                Id = a.Id,
//                Name = a.Name,
//                Description = a.Description,
//            })
//            .ToListAsync(cancellationToken);

//            return Ok(appointmentStatus);
//        }

//        [HttpGet("{id:int:min(1)}")]
//        public async Task<ActionResult<IEnumerable<AppointmentStatusDTO>>> GetById(int id, CancellationToken cancellationToken)
//        {
//            var appointmentStatus = await _context.AppointmentStatuses
//                .Where(a => a.Id == id)
//                .Select(a => new AppointmentStatusDTO
//                {
//                    Id = a.Id,
//                    Name = a.Name,
//                    Description = a.Description,
//                }).FirstOrDefaultAsync(cancellationToken);

//            if (appointmentStatus is null)
//                return NotFound();

//            return Ok(appointmentStatus);
//        }

//        [HttpPost]
//        public async Task<ActionResult<AppointmentStatusDTO>> Create(CreateAppointmentStatusDTO dto)
//        {
//            var appointmentStatus = new AppointmentStatus
//            {
//                Name = dto.Name,
//                Description = dto.Description,
//            };

//            _context.AppointmentStatuses.Add(appointmentStatus);
//            await _context.SaveChangesAsync();

//            var resultDto = new AppointmentStatusDTO
//            {
//                Id = appointmentStatus.Id,
//                Name = appointmentStatus.Name,
//                Description = appointmentStatus.Description
//            };

//            return CreatedAtAction(nameof(GetById),new {id = appointmentStatus.Id}, resultDto);
//        }

//        [HttpPut("{id:int:min(1)}")]
//        public async Task<IActionResult> Update(int id, CreateAppointmentStatusDTO dto)
//        {
//            var appointmentStatus = await _context.AppointmentStatuses.FindAsync(id);

//            if (appointmentStatus is null)
//                return NotFound();

//            appointmentStatus.Name = dto.Name;
//            appointmentStatus.Description = dto.Description;

//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        [HttpDelete("{id:int:min(1)}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var appointmentStatus = await _context.AppointmentStatuses.FindAsync(id);

//            if (appointmentStatus is null)
//                return NotFound();

//            var hasAppointments = await _context.Appointments.AnyAsync(a => a.AppointmentStatusId == id);
//            if (hasAppointments)
//                return BadRequest("Cannot delete this status because it has appointments linked to it.");

//            _context.AppointmentStatuses.Remove(appointmentStatus);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
