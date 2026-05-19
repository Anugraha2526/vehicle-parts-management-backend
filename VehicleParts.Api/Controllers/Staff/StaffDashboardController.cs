using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Api.Controllers.Staff;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
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

    [HttpGet("customer-reports")]
    public async Task<IActionResult> GetCustomerReports(CancellationToken cancellationToken)
    {
        var customerRole = VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Customer;

        var customers = await _context.Users
            .Where(u => u.Role == customerRole)
            .Select(c => new
            {
                c.Id,
                c.FullName,
                c.Email,
                c.PhoneNumber,
                InvoiceCount = _context.SalesInvoices.Count(s => s.CustomerId == c.Id),
                TotalSpent = _context.SalesInvoices.Where(s => s.CustomerId == c.Id).Sum(s => (decimal?)s.TotalAmount) ?? 0,
                PendingAmount = _context.SalesInvoices.Where(s => s.CustomerId == c.Id && !s.IsPaid).Sum(s => (decimal?)s.TotalAmount) ?? 0,
                PendingInvoicesCount = _context.SalesInvoices.Count(s => s.CustomerId == c.Id && !s.IsPaid)
            })
            .ToListAsync(cancellationToken);

        var regulars = customers
            .OrderByDescending(c => c.InvoiceCount)
            .Take(10);

        var highSpenders = customers
            .OrderByDescending(c => c.TotalSpent)
            .Take(10);

        var pendingCredits = customers
            .Where(c => c.PendingInvoicesCount > 0)
            .OrderByDescending(c => c.PendingAmount)
            .Take(10);

        return Ok(new
        {
            AllCustomers = customers,
            Regulars = regulars,
            HighSpenders = highSpenders,
            PendingCredits = pendingCredits
        });
    }
}
