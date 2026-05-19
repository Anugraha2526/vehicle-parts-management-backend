using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Repositories.AdminCore;

// EF Core repository for all data access against the Parts table
public sealed class PartsRepository : IPartsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PartsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // get all parts with vendor name included
    public async Task<IReadOnlyList<Part>> GetAllAsync(CancellationToken ct = default)
        => await _dbContext.Parts
            .AsNoTracking()
            .Include(p => p.Vendor)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(ct);

    // fetch single part with vendor loaded, returns null if not found
    public async Task<Part?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbContext.Parts
            .AsNoTracking()
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    // check if part number already exists before creating
    public async Task<bool> PartNumberExistsAsync(string partNumber, CancellationToken ct = default)
        => await _dbContext.Parts
            .AnyAsync(p => p.PartNumber.ToLower() == partNumber.ToLower(), ct);

    // check uniqueness excluding the part currently being updated
    public async Task<bool> PartNumberExistsForOtherAsync(string partNumber, Guid excludeId, CancellationToken ct = default)
        => await _dbContext.Parts
            .AnyAsync(p => p.PartNumber.ToLower() == partNumber.ToLower() && p.Id != excludeId, ct);

    // return parts below threshold ordered by lowest stock first
    public async Task<IReadOnlyList<Part>> GetLowStockPartsAsync(int threshold = 10, CancellationToken ct = default)
        => await _dbContext.Parts
            .AsNoTracking()
            .Include(p => p.Vendor)
            .Where(p => p.QuantityInStock < threshold)
            .OrderBy(p => p.QuantityInStock)
            .ToListAsync(ct);

    public async Task<Part> CreateAsync(Part part, CancellationToken ct = default)
    {
        await _dbContext.Parts.AddAsync(part, ct);
        await _dbContext.SaveChangesAsync(ct);
        return part;
    }

    // save changes and prevent EF from issuing a spurious UPDATE on the vendor row
    public async Task<Part> UpdateAsync(Part part, CancellationToken ct = default)
    {
        _dbContext.Parts.Update(part);
        if (part.Vendor is not null)
            _dbContext.Entry(part.Vendor).State = EntityState.Unchanged;
        await _dbContext.SaveChangesAsync(ct);
        return part;
    }

    // delete part by id, return false if not found
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var part = await _dbContext.Parts.FindAsync([id], ct);
        if (part is null) return false;

        _dbContext.Parts.Remove(part);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}
