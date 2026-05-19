namespace VehicleParts.Application.Modules.AdminCore.DTOs;

// read-only response shape returned by all parts endpoints
public sealed class PartResponseDto
{
    public Guid Id { get; init; }
    public string PartName { get; init; } = string.Empty;
    public string PartNumber { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public Guid VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public int QuantityInStock { get; init; }
    public decimal UnitCost { get; init; }
    public decimal SellingPrice { get; init; }
    public string? Description { get; init; }
    // computed field — true when stock falls below the 10-unit threshold
    public bool IsLowStock { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
