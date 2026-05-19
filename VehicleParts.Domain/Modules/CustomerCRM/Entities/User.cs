using System;
using System.Collections.Generic;
using VehicleParts.Domain.Common;
using VehicleParts.Domain.Modules.AdminCore.Enums;

namespace VehicleParts.Domain.Modules.CustomerCRM.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
