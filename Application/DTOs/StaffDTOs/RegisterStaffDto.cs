using System.ComponentModel.DataAnnotations;
using vehicle_parts_management_backend.Domain.Enums;

namespace vehicle_parts_management_backend.Application.DTOs.StaffDTOs
{
    // data the admin sends when registering a new staff member
    public class RegisterStaffDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // plain text password, gets hashed before saving
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        // role to assign to the new staff member
        [Required]
        public UserRole Role { get; set; } = UserRole.Staff;
    }
}
