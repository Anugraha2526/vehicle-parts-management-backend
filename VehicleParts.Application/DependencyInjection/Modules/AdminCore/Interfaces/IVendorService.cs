using VehicleParts.Application.Modules.AdminCore.DTOs;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

public interface IVendorService
{
    Task<IReadOnlyList<VendorResponseDto>> GetAllVendorsAsync(CancellationToken cancellationToken = default);
    Task<VendorResponseDto?> GetVendorByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VendorResponseDto> CreateVendorAsync(CreateVendorDto dto, CancellationToken cancellationToken = default);
    Task<VendorResponseDto?> UpdateVendorAsync(Guid id, UpdateVendorDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteVendorAsync(Guid id, CancellationToken cancellationToken = default);
}
