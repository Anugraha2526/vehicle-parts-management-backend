using VehicleParts.Domain.Common;
using VehicleParts.Domain.Modules.AdminCore.Enums;

namespace VehicleParts.Domain.Modules.AdminCore.Entities;

// represents a system user with admin or staff role
public sealed class StaffMember : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Staff;
    public bool IsActive { get; set; } = true;
}
