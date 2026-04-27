namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class PurchaseInvoiceItemDto
{
    public Guid PartId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
}
