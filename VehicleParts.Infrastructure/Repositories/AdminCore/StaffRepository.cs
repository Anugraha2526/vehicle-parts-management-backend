using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Repositories.AdminCore;

public sealed class StaffRepository : IStaffRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StaffRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers.FindAsync([id], cancellationToken);

    public async Task<StaffMember?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers
            .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);

    public async Task<IReadOnlyList<StaffMember>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers
            .OrderBy(s => s.FullName)
            .ToListAsync(cancellationToken);

    public async Task<StaffMember> CreateAsync(StaffMember staffMember, CancellationToken cancellationToken = default)
    {
        await _dbContext.StaffMembers.AddAsync(staffMember, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staffMember;
    }

    public async Task<StaffMember> UpdateAsync(StaffMember staffMember, CancellationToken cancellationToken = default)
    {
        _dbContext.StaffMembers.Update(staffMember);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staffMember;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staffMember = await _dbContext.StaffMembers.FindAsync([id], cancellationToken);
        if (staffMember is not null)
        {
            _dbContext.StaffMembers.Remove(staffMember);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers.AnyAsync(s => s.Email == email, cancellationToken);
}
