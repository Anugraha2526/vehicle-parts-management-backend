using System.ComponentModel.DataAnnotations;
using VehicleParts.Domain.Modules.AdminCore.Enums;

namespace VehicleParts.Application.Modules.AdminCore.DTOs;

public sealed class RegisterStaffDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Staff;
}
