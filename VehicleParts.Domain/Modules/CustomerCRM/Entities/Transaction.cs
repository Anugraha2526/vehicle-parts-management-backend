using System;
using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerCRM.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}
