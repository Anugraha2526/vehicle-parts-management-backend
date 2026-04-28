using System.ComponentModel.DataAnnotations;

namespace vehicle_parts_management_backend.Domain.Entities
{
    public class Part
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int QuantityInStock { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}