using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.AdminCore.Entities;

public sealed class Part : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? OemCode { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
}
