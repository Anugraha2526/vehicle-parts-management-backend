using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.Modules.AdminCore.DTOs;

public sealed class CreateVendorDto
{
    [Required]
    [MaxLength(200)]
    public string VendorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContactPerson { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
