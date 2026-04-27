using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.Sales.Entities;

public sealed class SalesInvoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }

    /// <summary>Staff member who processed this sale.</summary>
    public Guid StaffId { get; set; }

    public DateTime SoldAtUtc { get; set; } = DateTime.UtcNow;


    /// <summary>Subtotal before any discount.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>True when the loyalty 10% discount was applied (subtotal > 5000).</summary>
    public bool LoyaltyDiscountApplied { get; set; }

    /// <summary>Discount amount deducted from SubTotal.</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>Final amount the customer pays (SubTotal - DiscountAmount).</summary>
    public decimal TotalAmount { get; set; }

    public List<SalesInvoiceItem> Items { get; set; } = [];

    /// <summary>
    /// Recalculates SubTotal, applies loyalty discount when SubTotal &gt; 5000,
    /// and sets TotalAmount.
    /// </summary>
    public void RecalculateTotal()
    {
        SubTotal = Items.Sum(item => item.Quantity * item.UnitPrice);

        if (SubTotal > 5000)
        {
            LoyaltyDiscountApplied = true;
            DiscountAmount = Math.Round(SubTotal * 0.10m, 2);
        }
        else
        {
            LoyaltyDiscountApplied = false;
            DiscountAmount = 0;
        }

        TotalAmount = SubTotal - DiscountAmount;
    }
}
