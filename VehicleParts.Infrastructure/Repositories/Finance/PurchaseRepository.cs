using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Repositories.Finance;

public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PurchaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Part>> GetPartsByIdsAsync(
        IReadOnlyCollection<Guid> partIds,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parts
            .Where(part => partIds.Contains(part.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<PurchaseInvoice> CreatePurchaseInvoiceAsync(
        PurchaseInvoice invoice,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        await _dbContext.PurchaseInvoices.AddAsync(invoice, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return invoice;
    }
}
