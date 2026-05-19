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

    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters and include an uppercase letter, a lowercase letter, a number, and a special character.")]
    [MaxLength(200)]
    public string? Password { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsActive { get; set; }
}
