using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface ILowStockService
{
    Task<ServiceResult<IReadOnlyList<LowStockAlertDto>>> ScanAndNotifyLowStockAsync(
        int threshold = 10,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<IReadOnlyList<LowStockAlertDto>>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default);

    Task<ServiceResult> AcknowledgeAlertAsync(
        Guid alertId,
        CancellationToken cancellationToken = default);
}
