using System;

namespace vehicle_parts_management_backend.Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key to User
        public string VehicleNumber { get; set; } = string.Empty; // e.g., License Plate
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; } = null!;
    }
}
