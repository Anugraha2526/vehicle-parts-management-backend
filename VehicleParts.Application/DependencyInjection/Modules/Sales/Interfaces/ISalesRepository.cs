using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Sales.Entities;

namespace VehicleParts.Application.Modules.Sales.Interfaces;

public interface ISalesRepository
{
    /// <summary>Fetches the parts for the given IDs to validate existence and capture pricing.</summary>
    Task<List<Part>> GetPartsByIdsAsync(Guid[] partIds, CancellationToken cancellationToken);

    /// <summary>Persists the invoice (with its items) and decrements stock for each part.</summary>
    Task<SalesInvoice> CreateSalesInvoiceAsync(SalesInvoice invoice, CancellationToken cancellationToken);

    /// <summary>Returns an invoice with its items by ID (used for email sending).</summary>
    Task<SalesInvoice?> GetSalesInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken);

    /// <summary>Returns recent sales invoices.</summary>
    Task<List<SalesInvoice>> GetRecentInvoicesAsync(int limit, CancellationToken cancellationToken);

    /// <summary>Fetches unpaid invoices strictly older than a specific month threshold.</summary>
    Task<List<SalesInvoice>> GetUnpaidInvoicesAsync(int olderThanMonths, CancellationToken cancellationToken);

    /// <summary>Marks an invoice safely as paid and commits to EF Tracking.</summary>
    Task<bool> MarkInvoiceAsPaidAsync(Guid invoiceId, CancellationToken cancellationToken);
    Task<bool> MarkInvoiceAsUnpaidAsync(Guid invoiceId, CancellationToken cancellationToken);
}
