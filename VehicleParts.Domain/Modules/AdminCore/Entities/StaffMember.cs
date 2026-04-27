using VehicleParts.Domain.Common;
using VehicleParts.Domain.Modules.AdminCore.Enums;

namespace VehicleParts.Domain.Modules.AdminCore.Entities;

public sealed class StaffMember : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Staff;
}
