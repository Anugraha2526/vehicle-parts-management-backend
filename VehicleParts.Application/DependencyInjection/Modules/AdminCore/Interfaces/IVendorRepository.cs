using VehicleParts.Domain.Modules.AdminCore.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

public interface IVendorRepository
{
    Task<IReadOnlyList<Vendor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Vendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Vendor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<Vendor> CreateAsync(Vendor vendor, CancellationToken cancellationToken = default);
    Task<Vendor> UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
