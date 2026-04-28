using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.Modules.Sales.DTOs;

public sealed class CreateSalesInvoiceDto
{
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>ID of the staff member processing the sale.</summary>
    [Required]
    public Guid StaffId { get; set; }

    [Required, MinLength(1, ErrorMessage = "At least one item is required.")]
    public List<SalesInvoiceItemRequestDto> Items { get; set; } = [];
}
