using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface IPurchaseRepository
{
    Task<bool> VendorExistsAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Part>> GetPartsByIdsAsync(
        IReadOnlyCollection<Guid> partIds,
        CancellationToken cancellationToken = default);

    Task<PurchaseInvoice> CreatePurchaseInvoiceAsync(
        PurchaseInvoice invoice,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PurchaseInvoice>> GetPurchaseInvoicesAsync(
        CancellationToken cancellationToken = default);

    Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);
}
