using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;

namespace VehicleParts.Infrastructure.Repositories.Finance;

public sealed class LowStockRepository : ILowStockRepository
{
    public Task<IReadOnlyList<Part>> GetPartsBelowThresholdAsync(
        int threshold,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }

    public Task<IReadOnlyDictionary<Guid, LowStockNotification>> GetOpenAlertsByPartIdsAsync(
        IReadOnlyCollection<Guid> partIds,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }

    public Task AddAlertsAsync(
        IReadOnlyCollection<LowStockNotification> alerts,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }

    public Task<IReadOnlyList<LowStockNotification>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }

    public Task<bool> AcknowledgeAlertAsync(
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }
}
