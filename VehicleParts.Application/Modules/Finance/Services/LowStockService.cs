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
        if (lowStockParts.Count == 0)
        {
            return ServiceResult<IReadOnlyList<LowStockAlertDto>>.Ok(Array.Empty<LowStockAlertDto>(), "No low stock parts found.");
        }

        var partIds = lowStockParts.Select(part => part.Id).ToArray();
        var openAlerts = await _lowStockRepository.GetOpenAlertsByPartIdsAsync(partIds, cancellationToken);

        var newAlerts = new List<LowStockNotification>();
        foreach (var part in lowStockParts)
        {
            if (openAlerts.ContainsKey(part.Id))
            {
                continue;
            }

            newAlerts.Add(new LowStockNotification
            {
                PartId = part.Id,
                PartName = part.Name,
                CurrentStockQuantity = part.StockQuantity,
                Threshold = threshold,
                IsAcknowledged = false,
                NotifiedAtUtc = DateTime.UtcNow
            });
        }

        if (newAlerts.Count > 0)
        {
            await _lowStockRepository.AddAlertsAsync(newAlerts, cancellationToken);
        }

        var activeAlerts = await _lowStockRepository.GetActiveAlertsAsync(cancellationToken);
        var response = activeAlerts.Select(MapAlert).ToArray();

        var message = newAlerts.Count > 0
            ? $"Low stock scan complete. {newAlerts.Count} new alert(s) created."
            : "Low stock scan complete. Existing alerts are still active.";

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
