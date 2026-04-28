using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using vehicle_parts_management_backend.Application.DTOs;
using vehicle_parts_management_backend.Infrastructure.Data;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var today = DateTime.UtcNow.Date;

            // Real Data Queries
            var totalCustomers = await _context.Users.CountAsync(u => u.Role.ToLower() == "customer");
            var totalVehicles = await _context.Vehicles.CountAsync();
            var registeredToday = await _context.Users.CountAsync(u => u.Role.ToLower() == "customer" && u.CreatedAt >= today);
            var totalTransactions = await _context.Transactions.CountAsync();
            
            var summary = new DashboardSummaryDto
            {
                TotalCustomers = totalCustomers,
                TotalVehicles = totalVehicles,
                RegisteredToday = registeredToday,
                TotalTransactions = totalTransactions
            };

            return Ok(summary);
        }

        [HttpGet("financial")]
        public IActionResult GetFinancialReports([FromQuery] string type) => Ok();

        [HttpGet("customer")]
        public IActionResult GetCustomerReports([FromQuery] string type) => Ok();
    }
}
