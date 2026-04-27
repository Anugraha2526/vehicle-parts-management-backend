using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.Finance.Entities;

public sealed class PurchaseInvoiceItem : BaseEntity
{
    public Guid PurchaseInvoiceId { get; set; }
    public PurchaseInvoice? PurchaseInvoice { get; set; }

    public Guid PartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }

    public decimal LineTotal => UnitCost * Quantity;
}
