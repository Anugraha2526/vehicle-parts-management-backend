using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.AdminCore.Entities;

public sealed class Vendor : BaseEntity
{
    public string VendorName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Notes { get; set; }
}
