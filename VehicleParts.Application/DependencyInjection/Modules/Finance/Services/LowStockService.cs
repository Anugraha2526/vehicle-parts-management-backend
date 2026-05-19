using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Domain.Modules.Finance.Entities;

namespace VehicleParts.Application.Modules.Finance.Services;

public sealed class LowStockService : ILowStockService
{
    private readonly ILowStockRepository _lowStockRepository;

    public LowStockService(ILowStockRepository lowStockRepository)
    {
        _lowStockRepository = lowStockRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<LowStockAlertDto>>> ScanAndNotifyLowStockAsync(
        int threshold = 10,
        CancellationToken cancellationToken = default)
    {
        if (threshold <= 0)
        {
            return ServiceResult<IReadOnlyList<LowStockAlertDto>>.Fail("Threshold must be greater than zero.");
        }

        var lowStockParts = await _lowStockRepository.GetPartsBelowThresholdAsync(threshold, cancellationToken);
        var activeAlerts = await _lowStockRepository.GetActiveAlertsAsync(cancellationToken);
        var lowPartsById = lowStockParts.ToDictionary(part => part.Id);
        var groupedAlerts = activeAlerts
            .GroupBy(alert => alert.PartId)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(alert => alert.NotifiedAtUtc).ToList());

        var now = DateTime.UtcNow;
        var resolvedCount = 0;
        var updatedCount = 0;
        var hasPendingChanges = false;

        foreach (var entry in groupedAlerts)
        {
            var alertsForPart = entry.Value;
            var activeAlert = alertsForPart[0];

            // Keep only one active alert per part. Older duplicates are auto-resolved.
            for (var index = 1; index < alertsForPart.Count; index++)
            {
                var duplicate = alertsForPart[index];
                duplicate.IsAcknowledged = true;
                duplicate.AcknowledgedAtUtc = now;
                duplicate.Touch();
                resolvedCount++;
                hasPendingChanges = true;
            }

            if (!lowPartsById.TryGetValue(entry.Key, out var currentPart))
            {
                activeAlert.IsAcknowledged = true;
                activeAlert.AcknowledgedAtUtc = now;
                activeAlert.Touch();
                resolvedCount++;
                hasPendingChanges = true;
                continue;
            }

            var changed = false;

            if (!string.Equals(activeAlert.PartName, currentPart.PartName, StringComparison.Ordinal))
            {
                activeAlert.PartName = currentPart.PartName;
                changed = true;
            }

            if (activeAlert.CurrentStockQuantity != currentPart.QuantityInStock)
            {
                activeAlert.CurrentStockQuantity = currentPart.QuantityInStock;
                changed = true;
            }

            if (activeAlert.Threshold != threshold)
            {
                activeAlert.Threshold = threshold;
                changed = true;
            }

            if (changed)
            {
                activeAlert.Touch();
                updatedCount++;
                hasPendingChanges = true;
            }
        }

        var newAlerts = new List<LowStockNotification>();
        foreach (var part in lowStockParts)
        {
            if (groupedAlerts.ContainsKey(part.Id))
            {
                continue;
            }

            newAlerts.Add(new LowStockNotification
            {
                PartId = part.Id,
                PartName = part.PartName,
                CurrentStockQuantity = part.QuantityInStock,
                Threshold = threshold,
                IsAcknowledged = false,
                NotifiedAtUtc = now
            });
        }

        if (newAlerts.Count > 0)
        {
            await _lowStockRepository.AddAlertsAsync(newAlerts, cancellationToken);
        }
        else if (hasPendingChanges)
        {
            await _lowStockRepository.SaveChangesAsync(cancellationToken);
        }

        activeAlerts = await _lowStockRepository.GetActiveAlertsAsync(cancellationToken);
        var response = activeAlerts.Select(MapAlert).ToArray();

        var message = newAlerts.Count == 0 && updatedCount == 0 && resolvedCount == 0 && lowStockParts.Count == 0
            ? "No low stock parts found."
            : $"Low stock scan complete. {newAlerts.Count} new, {updatedCount} updated, {resolvedCount} resolved.";

        return ServiceResult<IReadOnlyList<LowStockAlertDto>>.Ok(response, message);
    }

    public async Task<ServiceResult<IReadOnlyList<LowStockAlertDto>>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        var alerts = await _lowStockRepository.GetActiveAlertsAsync(cancellationToken);
        var response = alerts.Select(MapAlert).ToArray();
        return ServiceResult<IReadOnlyList<LowStockAlertDto>>.Ok(response, "Active low stock alerts fetched.");
    }

    public async Task<ServiceResult> AcknowledgeAlertAsync(
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        var acknowledged = await _lowStockRepository.AcknowledgeAlertAsync(alertId, cancellationToken);
        if (!acknowledged)
        {
            return ServiceResult.Fail("Low stock alert not found or already acknowledged.");
        }

        return ServiceResult.Ok("Low stock alert acknowledged.");
    }

    private static LowStockAlertDto MapAlert(LowStockNotification alert)
    {
        return new LowStockAlertDto
        {
            AlertId = alert.Id,
            PartId = alert.PartId,
            PartName = alert.PartName,
            CurrentStockQuantity = alert.CurrentStockQuantity,
            Threshold = alert.Threshold,
            NotifiedAtUtc = alert.NotifiedAtUtc,
            IsAcknowledged = alert.IsAcknowledged
        };
    }
}
