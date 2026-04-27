using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.Modules.Sales.DTOs;

public sealed class SalesInvoiceItemRequestDto
{
    [Required]
    public Guid PartId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}
