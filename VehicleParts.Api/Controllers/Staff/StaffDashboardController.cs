using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Api.Controllers.Staff;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Staff,Admin")]
public class StaffDashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StaffDashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken)
    {
        var totalCustomers = await _context.Users.CountAsync(u => u.Role == VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Customer, cancellationToken);
        var totalVehicles = await _context.Vehicles.CountAsync(cancellationToken);
        
        var invoicesCount = await _context.SalesInvoices.CountAsync(cancellationToken);
        var lowStockCount = await _context.Parts.CountAsync(p => p.QuantityInStock < 5, cancellationToken);

        // Chart Data: last 7 days sales
        var today = DateTime.UtcNow.Date;
        var startOfWeek = today.AddDays(-6);
        
        var recentSales = await _context.SalesInvoices
            .Where(s => s.CreatedAtUtc >= startOfWeek)
            .GroupBy(s => s.CreatedAtUtc.Date)
            .Select(g => new {
                Date = g.Key,
                TotalAmount = g.Sum(s => s.TotalAmount)
            })
            .ToListAsync(cancellationToken);

        // Fill in empty days
        var weeklySales = new List<object>();
        for (int i = 0; i <= 6; i++)
        {
            var day = startOfWeek.AddDays(i);
            var sale = recentSales.FirstOrDefault(s => s.Date == day);
            weeklySales.Add(new {
                name = day.ToString("ddd"), // Mon, Tue, etc.
                Sales = sale?.TotalAmount ?? 0m
            });
        }

        return Ok(new {
            TotalCustomers = totalCustomers,
            TotalVehicles = totalVehicles,
            TotalSalesInvoices = invoicesCount,
            LowStockParts = lowStockCount,
            WeeklySales = weeklySales
        });
    }

    // Parts list for invoice creation — accessible to Staff without needing PartsController
    [HttpGet("parts")]
    public async Task<IActionResult> GetPartsForInvoice(CancellationToken cancellationToken)
    {
        var parts = await _context.Parts
            .Select(p => new {
                p.Id,
                p.PartName,
                p.SellingPrice,
                p.QuantityInStock
            })
            .ToListAsync(cancellationToken);

        return Ok(parts);
    }
}
