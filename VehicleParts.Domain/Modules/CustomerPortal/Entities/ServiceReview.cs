using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerPortal.Entities;

public sealed class ServiceReview : BaseEntity
{
    public Guid CustomerId { get; set; }

    // 1–5 star rating
    public int Rating { get; set; }

    public string ServiceType { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
