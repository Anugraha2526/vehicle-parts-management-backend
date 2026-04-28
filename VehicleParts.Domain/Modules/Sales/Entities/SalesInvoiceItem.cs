using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.Sales.Entities;

/// <summary>A single line item in a sales invoice (maps to ORDER_ITEMS in ERD).</summary>
public sealed class SalesInvoiceItem : BaseEntity
{
    public Guid SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; } = null!;

    public Guid PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }

    /// <summary>Unit price captured at time of sale.</summary>
    public decimal UnitPrice { get; set; }

    public decimal SubTotal => Quantity * UnitPrice;
}
