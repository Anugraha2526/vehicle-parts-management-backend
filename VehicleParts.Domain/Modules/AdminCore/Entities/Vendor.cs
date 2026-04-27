using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.AdminCore.Entities;

public sealed class Vendor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
}
