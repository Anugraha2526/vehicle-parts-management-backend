using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerPortal.Entities;

public sealed class PartRequest : BaseEntity
    {
        public int Id { get; set; }
        public Guid PartId { get; set; }
        public Guid CustomerId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string PartName { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
