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
        DateTime? referenceDateUtc = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedType = type.Trim().ToLowerInvariant();
        var referenceUtc = (referenceDateUtc ?? DateTime.UtcNow).ToUniversalTime();
        var (startUtc, endUtc) = ResolvePeriod(normalizedType, referenceUtc);

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

        var purchaseTransactions = await purchaseQuery
            .AsNoTracking()
            .Select(invoice => new FinancialTransactionDto
            {
                EntryType = "Purchase",
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TransactionDateUtc = invoice.PurchasedAtUtc,
                ItemCount = invoice.Items.Sum(item => item.Quantity),
                TotalAmount = invoice.TotalAmount
            })
            .ToListAsync(cancellationToken);

        var salesTransactions = await salesQuery
            .AsNoTracking()
            .Select(invoice => new FinancialTransactionDto
            {
                EntryType = "Sale",
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TransactionDateUtc = invoice.SoldAtUtc,
                ItemCount = invoice.Items.Sum(item => item.Quantity),
                TotalAmount = invoice.TotalAmount
            })
            .ToListAsync(cancellationToken);

        var transactions = purchaseTransactions
            .Concat(salesTransactions)
            .OrderByDescending(entry => entry.TransactionDateUtc)
            .ToArray();

        var rawPurchasePartMetrics = await _dbContext.PurchaseInvoiceItems
            .AsNoTracking()
            .Where(item =>
                item.PurchaseInvoice != null &&
                item.PurchaseInvoice.PurchasedAtUtc >= startUtc &&
                item.PurchaseInvoice.PurchasedAtUtc <= endUtc)
            .GroupBy(item => item.PartId)
            .Select(group => new
            {
                PartId = group.Key,
                Quantity = group.Sum(item => item.Quantity),
                Amount = group.Sum(item => item.UnitCost * item.Quantity)
            })
            .OrderByDescending(item => item.Amount)
            .Take(5)
            .ToListAsync(cancellationToken);

        var purchasePartIds = rawPurchasePartMetrics.Select(item => item.PartId).ToArray();
        var purchasePartNames = await _dbContext.Parts
            .AsNoTracking()
            .Where(part => purchasePartIds.Contains(part.Id))
            .ToDictionaryAsync(part => part.Id, part => part.PartName, cancellationToken);

        var topPurchaseParts = rawPurchasePartMetrics
            .Select(item => new TopPartMetricDto
            {
                PartId = item.PartId,
                PartName = purchasePartNames.TryGetValue(item.PartId, out var name)
                    ? name
                    : $"Part {item.PartId}",
                Quantity = item.Quantity,
                Amount = item.Amount
            })
            .ToArray();

        var topSalesParts = await _dbContext.SalesInvoiceItems
            .AsNoTracking()
            .Where(item =>
                item.SalesInvoice != null &&
                item.SalesInvoice.SoldAtUtc >= startUtc &&
                item.SalesInvoice.SoldAtUtc <= endUtc)
            .GroupBy(item => new { item.PartId, item.PartName })
            .Select(group => new TopPartMetricDto
            {
                PartId = group.Key.PartId,
                PartName = group.Key.PartName,
                Quantity = group.Sum(item => item.Quantity),
                Amount = group.Sum(item => item.UnitPrice * item.Quantity)
            })
            .OrderByDescending(item => item.Amount)
            .Take(5)
            .ToArrayAsync(cancellationToken);

        return new FinancialReportDto
        {
            PeriodType = normalizedType,
            ReferenceDateUtc = referenceUtc,
            PeriodStartUtc = startUtc,
            PeriodEndUtc = endUtc,
            GeneratedAtUtc = DateTime.UtcNow,
            PurchaseInvoiceCount = purchaseInvoiceCount,
            TotalPurchaseAmount = totalPurchaseAmount,
            SalesInvoiceCount = salesInvoiceCount,
            TotalSalesAmount = totalSalesAmount,
            NetAmount = totalSalesAmount - totalPurchaseAmount,
            Transactions = transactions,
            TopPurchaseParts = topPurchaseParts,
            TopSalesParts = topSalesParts
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
