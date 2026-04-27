using VehicleParts.Domain.Modules.AdminCore.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

public interface IStaffRepository
{
    Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StaffMember?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffMember>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StaffMember> CreateAsync(StaffMember staffMember, CancellationToken cancellationToken = default);
    Task<StaffMember> UpdateAsync(StaffMember staffMember, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
