using VehicleParts.Application.Modules.AdminCore.DTOs;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

// business logic contract for parts management
public interface IPartsService
{
    Task<IReadOnlyList<PartResponseDto>> GetAllPartsAsync(CancellationToken ct = default);
    Task<PartResponseDto?> GetPartByIdAsync(Guid id, CancellationToken ct = default);
    Task<PartResponseDto> CreatePartAsync(CreatePartDto dto, CancellationToken ct = default);
    Task<PartResponseDto?> UpdatePartAsync(Guid id, UpdatePartDto dto, CancellationToken ct = default);
    Task<bool> DeletePartAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<PartResponseDto>> GetLowStockPartsAsync(int threshold = 10, CancellationToken ct = default);
}
