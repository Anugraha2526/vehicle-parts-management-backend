using vehicle_parts_management_backend.Domain.Common;
using vehicle_parts_management_backend.Domain.Enums;

namespace vehicle_parts_management_backend.Domain.Entities
{
    // staff entity that maps to the staff table in the database
    public class Staff : BaseEntity
    {
        // full name of the staff member
        public string FullName { get; set; } = string.Empty;

        // email used to log in, must be unique
        public string Email { get; set; } = string.Empty;

        // hashed version of the password, never store plain text
        public string PasswordHash { get; set; } = string.Empty;

        // role assigned to this staff member
        public UserRole Role { get; set; } = UserRole.Admin;

        // whether this staff account is currently active
        public bool IsActive { get; set; } = true;
    }
}
