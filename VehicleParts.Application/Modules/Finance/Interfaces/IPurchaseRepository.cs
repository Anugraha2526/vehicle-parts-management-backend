using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface IPurchaseRepository
{
    Task<IReadOnlyList<Part>> GetPartsByIdsAsync(
        IReadOnlyCollection<Guid> partIds,
        CancellationToken cancellationToken = default);

    Task<PurchaseInvoice> CreatePurchaseInvoiceAsync(
        PurchaseInvoice invoice,
        CancellationToken cancellationToken = default);
}
