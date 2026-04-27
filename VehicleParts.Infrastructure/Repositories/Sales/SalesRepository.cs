using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Modules.Sales.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Sales.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Repositories.Sales;

public sealed class SalesRepository : ISalesRepository
{
    private readonly ApplicationDbContext _db;

    public SalesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Part>> GetPartsByIdsAsync(Guid[] partIds, CancellationToken cancellationToken)
    {
        return await _db.Parts
            .Where(p => partIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<SalesInvoice> CreateSalesInvoiceAsync(SalesInvoice invoice, CancellationToken cancellationToken)
    {
        // Decrement stock for each part sold
        var partIds = invoice.Items.Select(i => i.PartId).ToArray();
        var parts = await _db.Parts
            .Where(p => partIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var partMap = parts.ToDictionary(p => p.Id);
        foreach (var item in invoice.Items)
        {
            if (partMap.TryGetValue(item.PartId, out var part))
            {
                part.StockQuantity -= item.Quantity;
                part.Touch();
            }
        }

        _db.SalesInvoices.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);
        return invoice;
    }

    public async Task<SalesInvoice?> GetSalesInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        return await _db.SalesInvoices
            .Include(inv => inv.Items)
            .FirstOrDefaultAsync(inv => inv.Id == invoiceId, cancellationToken);
    }
}
