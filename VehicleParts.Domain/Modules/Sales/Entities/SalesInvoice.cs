using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.Sales.Entities;

public sealed class SalesInvoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SoldAtUtc { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
}
