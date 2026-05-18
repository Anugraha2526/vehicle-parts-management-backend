using Microsoft.Extensions.Logging;
using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Services;

// implements business rules for part inventory management
public sealed class PartsService : IPartsService
{
    private readonly IPartsRepository _partsRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly ILogger<PartsService> _logger;

    public PartsService(IPartsRepository partsRepository, IVendorRepository vendorRepository, ILogger<PartsService> logger)
    {
        _partsRepository = partsRepository;
        _vendorRepository = vendorRepository;
        _logger = logger;
    }

    // get all parts with vendor name included
    public async Task<IReadOnlyList<PartResponseDto>> GetAllPartsAsync(CancellationToken ct = default)
    {
        var parts = await _partsRepository.GetAllAsync(ct);
        return parts.Select(ToDto).ToList();
    }

    public async Task<PartResponseDto?> GetPartByIdAsync(Guid id, CancellationToken ct = default)
    {
        var part = await _partsRepository.GetByIdAsync(id, ct);
        return part is null ? null : ToDto(part);
    }

    // verify vendor exists and part number is unique before inserting
    public async Task<PartResponseDto> CreatePartAsync(CreatePartDto dto, CancellationToken ct = default)
    {
        // reject if the vendor id does not match any vendor record
        var vendor = await _vendorRepository.GetByIdAsync(dto.VendorId, ct);
        if (vendor is null)
            throw new ArgumentException($"Vendor with id '{dto.VendorId}' was not found.");

        // reject duplicate part numbers case-insensitively
        if (await _partsRepository.PartNumberExistsAsync(dto.PartNumber, ct))
            throw new ArgumentException($"Part with number '{dto.PartNumber}' already exists.");

        var part = new Part
        {
            PartName = dto.PartName,
            PartNumber = dto.PartNumber,
            Category = dto.Category,
            VendorId = dto.VendorId,
            QuantityInStock = dto.QuantityPurchased,
            UnitCost = dto.UnitCost,
            SellingPrice = dto.SellingPrice,
            Description = dto.Description
        };

        var created = await _partsRepository.CreateAsync(part, ct);
        // attach vendor to navigation so vendor name appears in the response
        created.Vendor = vendor;
        _logger.LogInformation("part created with id {Id}", created.Id);

        // warn if initial stock is already below the low-stock threshold
        if (created.QuantityInStock < 10)
            _logger.LogWarning("low stock: part {Id} has {Qty} units", created.Id, created.QuantityInStock);

        return ToDto(created);
    }

    // update only provided fields and accumulate any new stock from purchase
    public async Task<PartResponseDto?> UpdatePartAsync(Guid id, UpdatePartDto dto, CancellationToken ct = default)
    {
        var part = await _partsRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Part with id '{id}' was not found.");

        // check uniqueness only when part number is actually changing
        if (dto.PartNumber is not null && !string.Equals(part.PartNumber, dto.PartNumber, StringComparison.OrdinalIgnoreCase))
        {
            if (await _partsRepository.PartNumberExistsForOtherAsync(dto.PartNumber, id, ct))
                throw new ArgumentException($"Part with number '{dto.PartNumber}' already exists.");
        }

        // verify new vendor exists before applying the change
        if (dto.VendorId.HasValue)
        {
            var vendor = await _vendorRepository.GetByIdAsync(dto.VendorId.Value, ct);
            if (vendor is null)
                throw new ArgumentException($"Vendor with id '{dto.VendorId}' was not found.");
            part.VendorId = dto.VendorId.Value;
            // update navigation so vendor name reflects the new vendor in the response
            part.Vendor = vendor;
        }

        if (dto.PartName is not null) part.PartName = dto.PartName;
        if (dto.PartNumber is not null) part.PartNumber = dto.PartNumber;
        if (dto.Category is not null) part.Category = dto.Category;
        if (dto.UnitCost.HasValue) part.UnitCost = dto.UnitCost.Value;
        if (dto.SellingPrice.HasValue) part.SellingPrice = dto.SellingPrice.Value;
        if (dto.Description is not null) part.Description = dto.Description;

        if (dto.QuantityPurchased.HasValue)
        {
            if (dto.QuantityPurchased.Value == 0)
                throw new ArgumentException("Quantity adjustment cannot be zero.");
            var newStock = part.QuantityInStock + dto.QuantityPurchased.Value;
            if (newStock < 0)
                throw new ArgumentException($"Cannot remove {Math.Abs(dto.QuantityPurchased.Value)} units — only {part.QuantityInStock} in stock.");
            part.QuantityInStock = newStock;
        }

        part.Touch();

        var updated = await _partsRepository.UpdateAsync(part, ct);

        // log warning when stock drops below threshold
        if (updated.QuantityInStock < 10)
            _logger.LogWarning("low stock: part {Id} has {Qty} units", updated.Id, updated.QuantityInStock);

        _logger.LogInformation("part {Id} updated", updated.Id);
        return ToDto(updated);
    }

    // delete by id and log the outcome regardless of whether the part existed
    public async Task<bool> DeletePartAsync(Guid id, CancellationToken ct = default)
    {
        var deleted = await _partsRepository.DeleteAsync(id, ct);

        if (!deleted)
        {
            _logger.LogWarning("part {Id} not found for deletion", id);
            return false;
        }

        _logger.LogInformation("part {Id} deleted", id);
        return true;
    }

    // return parts below the given stock threshold ordered by most critical first
    public async Task<IReadOnlyList<PartResponseDto>> GetLowStockPartsAsync(int threshold = 10, CancellationToken ct = default)
    {
        var parts = await _partsRepository.GetLowStockPartsAsync(threshold, ct);
        return parts.Select(ToDto).ToList();
    }

    // map entity to response shape including the computed IsLowStock field
    private static PartResponseDto ToDto(Part p) => new()
    {
        Id = p.Id,
        PartName = p.PartName,
        PartNumber = p.PartNumber,
        Category = p.Category,
        VendorId = p.VendorId,
        VendorName = p.Vendor?.VendorName ?? string.Empty,
        QuantityInStock = p.QuantityInStock,
        UnitCost = p.UnitCost,
        SellingPrice = p.SellingPrice,
        Description = p.Description,
        IsLowStock = p.QuantityInStock < 10,
        CreatedAtUtc = p.CreatedAtUtc,
        UpdatedAtUtc = p.UpdatedAtUtc
    };
}
