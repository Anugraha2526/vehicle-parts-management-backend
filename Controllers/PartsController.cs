using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vehicle_parts_management_backend.Domain.Entities;
using vehicle_parts_management_backend.Infrastructure.Data;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PartRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Create Request
        [HttpPost]
        public async Task<IActionResult> Create(PartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Status = "Pending";
            request.CreatedAtUtc = DateTime.UtcNow;

            _context.PartRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Part request submitted successfully",
                requestId = request.Id
            });
        }

        // ✅ Get All Requests
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _context.PartRequests
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var requests = await _context.PartRequests
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync();

            if (!requests.Any())
                return NotFound("No part requests found for this customer.");

            return Ok(requests);
        }
        // ✅ Update Status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var request = await _context.PartRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            request.Status = status;
            request.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}