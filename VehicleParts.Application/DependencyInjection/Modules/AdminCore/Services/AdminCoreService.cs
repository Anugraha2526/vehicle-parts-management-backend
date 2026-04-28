using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Application.Modules.AdminCore.Services;

public sealed class AdminCoreService : IAdminCoreService
{
    public Task<ServiceResult> RegisterStaffAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Staff registration use case is wired."));

    public Task<ServiceResult> GetAllStaffAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("List staff use case is wired."));

    public Task<ServiceResult> AddVendorAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Vendor create use case is wired."));

    public Task<ServiceResult> GetVendorsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Vendor list use case is wired."));

    public Task<ServiceResult> UpdateVendorAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Vendor update use case is wired for id {id}."));

    public Task<ServiceResult> DeleteVendorAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Vendor delete use case is wired for id {id}."));

    public Task<ServiceResult> AddPartAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Part create use case is wired."));

    public Task<ServiceResult> UpdatePartAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Part update use case is wired for id {id}."));

    public Task<ServiceResult> DeletePartAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Part delete use case is wired for id {id}."));

    public Task<ServiceResult> GetAllPartsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Part list use case is wired."));
}
