using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.Modules.AdminCore.DTOs;

// all fields nullable so partial updates preserve existing values
public sealed class UpdateVendorDto
{
    [MaxLength(200)]
    public string? VendorName { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [EmailAddress]
    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
