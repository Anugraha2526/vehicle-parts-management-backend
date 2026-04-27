using vehicle_parts_management_backend.Domain.Enums;

namespace vehicle_parts_management_backend.Application.DTOs.StaffDTOs
{
    // data the admin sends when updating a staff member's info — all fields nullable so partial updates preserve existing values
    public class UpdateStaffDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
