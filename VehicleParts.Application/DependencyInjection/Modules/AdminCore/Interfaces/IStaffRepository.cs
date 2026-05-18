using VehicleParts.Domain.Modules.CustomerCRM.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

// defines data access contract for staff operations
public interface IStaffRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User staffMember, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User staffMember, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
