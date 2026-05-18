using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Enums;
using VehicleParts.Domain.Modules.CustomerCRM.Entities;
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
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Admin || u.Role == UserRole.Staff)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    // find one staff member by email, case-insensitive
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Admin || u.Role == UserRole.Staff)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);

    // get all staff ordered by most recently created first
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Admin || u.Role == UserRole.Staff)
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    // add new staff record and save
    public async Task<User> CreateAsync(User staffMember, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(staffMember, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staffMember;
    }

    // update existing staff record and save
    public async Task<User> UpdateAsync(User staffMember, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(staffMember);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staffMember;
    }

    // delete staff record by id, guard against accidentally deleting a customer
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync([id], cancellationToken);
        if (user is not null && (user.Role == UserRole.Admin || user.Role == UserRole.Staff))
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // check if any user already uses this email
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
}
