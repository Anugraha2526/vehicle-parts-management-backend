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

    // find one staff member by id without tracking
    public async Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    // find one staff member by email, case-insensitive
    public async Task<StaffMember?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower(), cancellationToken);

    // get all staff ordered by most recently created first
    public async Task<IReadOnlyList<StaffMember>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    // add new staff record and save
    public async Task<StaffMember> CreateAsync(StaffMember staffMember, CancellationToken cancellationToken = default)
    {
        await _dbContext.StaffMembers.AddAsync(staffMember, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staffMember;
    }

    // update existing staff record and save
    public async Task<StaffMember> UpdateAsync(StaffMember staffMember, CancellationToken cancellationToken = default)
    {
        _dbContext.StaffMembers.Update(staffMember);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staffMember;
    }

    // delete staff record by id if it exists
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staffMember = await _dbContext.StaffMembers.FindAsync([id], cancellationToken);
        if (staffMember is not null)
        {
            _dbContext.StaffMembers.Remove(staffMember);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // check if any staff member already uses this email
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.StaffMembers
            .AnyAsync(s => s.Email.ToLower() == email.ToLower(), cancellationToken);
}
