namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class PurchaseInvoiceLineItemResponseDto
{
    public Guid PartId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public decimal LineTotal { get; init; }
}
