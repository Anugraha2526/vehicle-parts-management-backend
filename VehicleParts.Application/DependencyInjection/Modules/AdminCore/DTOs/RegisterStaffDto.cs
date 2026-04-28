using System.ComponentModel.DataAnnotations;
using VehicleParts.Domain.Modules.AdminCore.Enums;

namespace VehicleParts.Application.Modules.AdminCore.DTOs;

public sealed class RegisterStaffDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Staff;
}
