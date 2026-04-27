using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;

namespace VehicleParts.Infrastructure.Repositories.Finance;

public sealed class PurchaseRepository : IPurchaseRepository
{
    public Task<IReadOnlyList<Part>> GetPartsByIdsAsync(
        IReadOnlyCollection<Guid> partIds,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }

    public Task<PurchaseInvoice> CreatePurchaseInvoiceAsync(
        PurchaseInvoice invoice,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }
}
