using System.ComponentModel.DataAnnotations;

namespace vehicle_parts_management_backend.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        // ✅ Just a normal column (no FK)
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime AppointmentAtUtc { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }
    }
}