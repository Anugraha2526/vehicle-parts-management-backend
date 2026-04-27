using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Repositories.AdminCore;

public sealed class VendorRepository : IVendorRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VendorRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // get all vendors ordered by creation date descending
    public async Task<IReadOnlyList<Vendor>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Vendors
            .AsNoTracking()
            .OrderByDescending(v => v.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    // find one vendor by id without tracking
    public async Task<Vendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    // find one vendor by email, case-insensitive
    public async Task<Vendor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Email.ToLower() == email.ToLower(), cancellationToken);

    // check if any vendor already uses this email
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _dbContext.Vendors
            .AnyAsync(v => v.Email.ToLower() == email.ToLower(), cancellationToken);

    // add new vendor record and save
    public async Task<Vendor> CreateAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        await _dbContext.Vendors.AddAsync(vendor, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return vendor;
    }

    // update existing vendor record and save
    public async Task<Vendor> UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        _dbContext.Vendors.Update(vendor);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return vendor;
    }

    // delete vendor by id, return false if not found
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vendor = await _dbContext.Vendors.FindAsync([id], cancellationToken);
        if (vendor is null) return false;

        _dbContext.Vendors.Remove(vendor);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
