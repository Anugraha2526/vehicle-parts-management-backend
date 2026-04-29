using System;
using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerCRM.Entities
{
    public class Vehicle : BaseEntity
    {
        public Guid UserId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; } = null!;
    }
}
