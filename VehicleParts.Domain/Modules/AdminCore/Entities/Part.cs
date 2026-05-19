using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.AdminCore.Entities;

// stores part inventory, pricing, and supplier reference
public sealed class Part : BaseEntity
{
    public string PartName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    // loaded via Include — null when vendor join is skipped
    public Vendor? Vendor { get; set; }
    public int QuantityInStock { get; set; }
    public decimal UnitCost { get; set; }
    public decimal SellingPrice { get; set; }
    public string? Description { get; set; }
}
