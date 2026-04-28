using System.ComponentModel.DataAnnotations;

namespace vehicle_parts_management_backend.Domain.Entities
{
    public class PartRequest
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(150)]
        public string RequestedPartName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        // Example values: Pending, Approved, Rejected, Ordered

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }
    }
}