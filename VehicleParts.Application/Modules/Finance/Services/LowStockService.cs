using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Application.Modules.Finance.Services;

public sealed class LowStockService : ILowStockService
{
    public Task<ServiceResult<IReadOnlyList<LowStockAlertDto>>> ScanAndNotifyLowStockAsync(
        int threshold = 10,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            ServiceResult<IReadOnlyList<LowStockAlertDto>>.Fail(
                "Temporarily disabled. Member 2 implementation is parked in Member2_Finance_Backup."));
    }

    public Task<ServiceResult<IReadOnlyList<LowStockAlertDto>>> GetActiveAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            ServiceResult<IReadOnlyList<LowStockAlertDto>>.Ok(
                Array.Empty<LowStockAlertDto>(),
                "Architecture skeleton only. Finance module implementation is parked in backup."));
    }

    public Task<ServiceResult> AcknowledgeAlertAsync(
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            ServiceResult.Fail(
                "Temporarily disabled. Member 2 implementation is parked in Member2_Finance_Backup."));
    }
}
