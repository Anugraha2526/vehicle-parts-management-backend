using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.Finance.Entities;

public sealed class PurchaseInvoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public DateTime PurchasedAtUtc { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    // TODO(Member 2): implement real finance rules/calculations.
    public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
}
