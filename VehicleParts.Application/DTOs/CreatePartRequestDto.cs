namespace VehicleParts.Application.DTOs.CustomerPortal
{
    public class CreatePartRequestDto
    {
        public Guid PartId { get; set; }
        public Guid CustomerId { get; set; }        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string PartName { get; set; } = string.Empty;
    }
}