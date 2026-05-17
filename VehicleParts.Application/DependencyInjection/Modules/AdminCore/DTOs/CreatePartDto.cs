using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.Modules.AdminCore.DTOs;

// request body for adding a new part — QuantityPurchased sets the initial stock
public sealed class CreatePartDto
{
    [Required]
    [MaxLength(200)]
    public string PartName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public Guid VendorId { get; set; }

    // becomes the initial QuantityInStock value on the new part record
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int QuantityPurchased { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0.")]
    public decimal UnitCost { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than 0.")]
    public decimal SellingPrice { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
