using VehicleParts.Domain.Modules.AdminCore.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

// data access contract for parts operations
public interface IPartsRepository
{
    Task<IReadOnlyList<Part>> GetAllAsync(CancellationToken ct = default);
    Task<Part?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> PartNumberExistsAsync(string partNumber, CancellationToken ct = default);
    // excludes the part being edited to allow keeping the same number on update
    Task<bool> PartNumberExistsForOtherAsync(string partNumber, Guid excludeId, CancellationToken ct = default);
    Task<IReadOnlyList<Part>> GetLowStockPartsAsync(int threshold = 10, CancellationToken ct = default);
    Task<Part> CreateAsync(Part part, CancellationToken ct = default);
    Task<Part> UpdateAsync(Part part, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
