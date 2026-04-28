using System.ComponentModel.DataAnnotations;
using VehicleParts.Domain.Modules.AdminCore.Enums;

namespace VehicleParts.Application.Modules.AdminCore.DTOs;

// all fields nullable so partial updates preserve existing values
public sealed class UpdateStaffDto
{
    [MaxLength(100)]
    public string? FullName { get; set; }

    [EmailAddress]
    [MaxLength(150)]
    public string? Email { get; set; }

    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [MaxLength(200)]
    public string? Password { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsActive { get; set; }
}
