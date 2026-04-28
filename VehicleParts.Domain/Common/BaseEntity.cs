namespace VehicleParts.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

    public void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
