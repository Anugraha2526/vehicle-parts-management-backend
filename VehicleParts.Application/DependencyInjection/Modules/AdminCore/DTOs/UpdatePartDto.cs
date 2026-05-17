using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.Modules.AdminCore.DTOs;

// all fields nullable so partial updates preserve existing values
public sealed class UpdatePartDto
{
    [MaxLength(200)]
    public string? PartName { get; set; }

    [MaxLength(50)]
    public string? PartNumber { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public Guid? VendorId { get; set; }

    // added to existing QuantityInStock, not a direct stock override
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int? QuantityPurchased { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0.")]
    public decimal? UnitCost { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than 0.")]
    public decimal? SellingPrice { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
