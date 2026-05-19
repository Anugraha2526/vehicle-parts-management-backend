using System.ComponentModel.DataAnnotations;

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
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters and include an uppercase letter, a lowercase letter, a number, and a special character.")]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }
}
