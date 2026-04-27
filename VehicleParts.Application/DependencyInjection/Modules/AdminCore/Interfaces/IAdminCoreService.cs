using VehicleParts.Application.Common.Models;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

public interface IAdminCoreService
{
    Task<ServiceResult> RegisterStaffAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> GetAllStaffAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> AddVendorAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> GetVendorsAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateVendorAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteVendorAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult> AddPartAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdatePartAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeletePartAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult> GetAllPartsAsync(CancellationToken cancellationToken = default);
}
