using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Domain.Modules.Finance.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Repositories.Finance;

public sealed class LowStockRepository : ILowStockRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LowStockRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Part>> GetPartsBelowThresholdAsync(
        int threshold,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parts
            .Where(part => part.StockQuantity < threshold)
            .OrderBy(part => part.StockQuantity)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, LowStockNotification>> GetOpenAlertsByPartIdsAsync(
        IReadOnlyCollection<Guid> partIds,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LowStockNotifications
            .Where(alert => !alert.IsAcknowledged && partIds.Contains(alert.PartId))
            .GroupBy(alert => alert.PartId)
            .Select(group => group.OrderByDescending(alert => alert.NotifiedAtUtc).First())
            .ToDictionaryAsync(alert => alert.PartId, alert => alert, cancellationToken);
    }

    public async Task AddAlertsAsync(
        IReadOnlyCollection<LowStockNotification> alerts,
        CancellationToken cancellationToken = default)
    {
        if (alerts.Count == 0)
        {
            return;
        }

        await _dbContext.LowStockNotifications.AddRangeAsync(alerts, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LowStockNotification>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LowStockNotifications
            .Where(alert => !alert.IsAcknowledged)
            .OrderByDescending(alert => alert.NotifiedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AcknowledgeAlertAsync(
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        var alert = await _dbContext.LowStockNotifications
            .FirstOrDefaultAsync(item => item.Id == alertId && !item.IsAcknowledged, cancellationToken);

        if (alert is null)
        {
            return false;
        }

        alert.IsAcknowledged = true;
        alert.AcknowledgedAtUtc = DateTime.UtcNow;
        alert.Touch();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
