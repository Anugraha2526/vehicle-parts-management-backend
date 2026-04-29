using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vehicle_parts_management_backend.Domain.Entities;
using vehicle_parts_management_backend.Infrastructure.Data;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Create Appointment
        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (appointment.AppointmentAtUtc < DateTime.UtcNow)
                return BadRequest("Appointment date cannot be in the past.");

            // ✅ Force UTC for PostgreSQL
            appointment.AppointmentAtUtc =
                DateTime.SpecifyKind(appointment.AppointmentAtUtc, DateTimeKind.Utc);

            appointment.CreatedAtUtc = DateTime.UtcNow;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Appointment created successfully",
                appointmentId = appointment.Id
            });
        }

        // ✅ Get All
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(a => a.AppointmentAtUtc)
                .ToListAsync();

            return Ok(appointments);
        }

        // ✅ Get By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        // ✅ Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Appointment updated)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound();

            appointment.AppointmentAtUtc =
                DateTime.SpecifyKind(updated.AppointmentAtUtc, DateTimeKind.Utc);

            appointment.Notes = updated.Notes;
            appointment.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(appointment);
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}