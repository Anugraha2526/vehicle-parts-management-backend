using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.DTOs;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Repositories.Finance;

public sealed class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReportRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FinancialReportDto> GetFinancialReportAsync(
        string type,
        CancellationToken cancellationToken = default)
    {
        var normalizedType = type.Trim().ToLowerInvariant();
        var (startUtc, endUtc) = ResolvePeriod(normalizedType, DateTime.UtcNow);

        var purchaseQuery = _dbContext.PurchaseInvoices
            .Where(invoice => invoice.PurchasedAtUtc >= startUtc && invoice.PurchasedAtUtc <= endUtc);

        var salesQuery = _dbContext.SalesInvoices
            .Where(invoice => invoice.SoldAtUtc >= startUtc && invoice.SoldAtUtc <= endUtc);

        var purchaseInvoiceCount = await purchaseQuery.CountAsync(cancellationToken);
        var salesInvoiceCount = await salesQuery.CountAsync(cancellationToken);

        var totalPurchaseAmount = await purchaseQuery
            .SumAsync(invoice => (decimal?)invoice.TotalAmount, cancellationToken) ?? 0m;

        var totalSalesAmount = await salesQuery
            .SumAsync(invoice => (decimal?)invoice.TotalAmount, cancellationToken) ?? 0m;

        return new FinancialReportDto
        {
            PeriodType = normalizedType,
            PeriodStartUtc = startUtc,
            PeriodEndUtc = endUtc,
            PurchaseInvoiceCount = purchaseInvoiceCount,
            TotalPurchaseAmount = totalPurchaseAmount,
            SalesInvoiceCount = salesInvoiceCount,
            TotalSalesAmount = totalSalesAmount,
            NetAmount = totalSalesAmount - totalPurchaseAmount
        };
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalCustomers = await _dbContext.Users.CountAsync(u => u.Role == VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Customer, cancellationToken);
        var totalVehicles = await _dbContext.Vehicles.CountAsync(cancellationToken);
        var totalTransactions = await _dbContext.Transactions.CountAsync(cancellationToken);

        var today = DateTime.UtcNow.Date;
        var registeredToday = await _dbContext.Users.CountAsync(u => u.Role == VehicleParts.Domain.Modules.AdminCore.Enums.UserRole.Customer && u.CreatedAtUtc >= today, cancellationToken);

        return new DashboardSummaryDto
        {
            TotalCustomers = totalCustomers,
            TotalVehicles = totalVehicles,
            TotalTransactions = totalTransactions,
            RegisteredToday = registeredToday
        };
    }

    private static (DateTime startUtc, DateTime endUtc) ResolvePeriod(string type, DateTime nowUtc)
    {
        return type switch
        {
            "daily" => (nowUtc.Date, nowUtc.Date.AddDays(1).AddTicks(-1)),
            "monthly" => (
                new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddTicks(-1)
            ),
            "yearly" => (
                new DateTime(nowUtc.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(nowUtc.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddYears(1).AddTicks(-1)
            ),
            _ => throw new ArgumentException("Unsupported report type. Use daily, monthly, or yearly.")
        };
    }
}
