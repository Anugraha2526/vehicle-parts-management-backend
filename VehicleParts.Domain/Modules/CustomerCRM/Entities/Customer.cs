using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerCRM.Entities;

public sealed class Customer : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}
