using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface ILowStockRepository
{
    Task<IReadOnlyList<Part>> GetPartsBelowThresholdAsync(
        int threshold,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, LowStockNotification>> GetOpenAlertsByPartIdsAsync(
        IReadOnlyCollection<Guid> partIds,
        CancellationToken cancellationToken = default);

    Task AddAlertsAsync(
        IReadOnlyCollection<LowStockNotification> alerts,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LowStockNotification>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default);

    Task<bool> AcknowledgeAlertAsync(
        Guid alertId,
        CancellationToken cancellationToken = default);
}
