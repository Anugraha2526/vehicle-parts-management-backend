using Microsoft.Extensions.Logging;
using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Services;

public sealed class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ILogger<VendorService> _logger;

    public VendorService(IVendorRepository vendorRepository, ILogger<VendorService> logger)
    {
        _vendorRepository = vendorRepository;
        _logger = logger;
    }

    // get all vendors ordered by creation date descending
    public async Task<IReadOnlyList<VendorResponseDto>> GetAllVendorsAsync(CancellationToken cancellationToken = default)
    {
        var vendors = await _vendorRepository.GetAllAsync(cancellationToken);
        return vendors.Select(ToDto).ToList();
    }

    // get a single vendor by id, return null if not found
    public async Task<VendorResponseDto?> GetVendorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id, cancellationToken);
        return vendor is null ? null : ToDto(vendor);
    }

    // check if email already exists before creating
    public async Task<VendorResponseDto> CreateVendorAsync(CreateVendorDto dto, CancellationToken cancellationToken = default)
    {
        if (await _vendorRepository.EmailExistsAsync(dto.Email, cancellationToken))
            throw new ArgumentException($"Vendor with email '{dto.Email}' already exists.");

        var vendor = new Vendor
        {
            VendorName = dto.VendorName,
            ContactPerson = dto.ContactPerson,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            Notes = dto.Notes
        };

        var created = await _vendorRepository.CreateAsync(vendor, cancellationToken);
        _logger.LogInformation("vendor created with id {Id}", created.Id);
        return ToDto(created);
    }

    // update only fields that were actually provided
    public async Task<VendorResponseDto?> UpdateVendorAsync(Guid id, UpdateVendorDto dto, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Vendor with id '{id}' was not found.");

        // check email uniqueness only when email is changing
        if (dto.Email != null && !string.Equals(vendor.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _vendorRepository.EmailExistsAsync(dto.Email, cancellationToken))
                throw new ArgumentException($"Vendor with email '{dto.Email}' already exists.");
        }

        if (dto.VendorName != null) vendor.VendorName = dto.VendorName;
        if (dto.ContactPerson != null) vendor.ContactPerson = dto.ContactPerson;
        if (dto.Phone != null) vendor.Phone = dto.Phone;
        if (dto.Email != null) vendor.Email = dto.Email;
        if (dto.Address != null) vendor.Address = dto.Address;
        if (dto.Notes != null) vendor.Notes = dto.Notes;
        vendor.Touch();

        var updated = await _vendorRepository.UpdateAsync(vendor, cancellationToken);
        _logger.LogInformation("vendor {Id} updated", updated.Id);
        return ToDto(updated);
    }

    // delete vendor by id, log warning if not found
    public async Task<bool> DeleteVendorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _vendorRepository.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            _logger.LogWarning("vendor {Id} not found for deletion", id);
            return false;
        }

        _logger.LogInformation("vendor {Id} deleted", id);
        return true;
    }

    private static VendorResponseDto ToDto(Vendor v) => new()
    {
        Id = v.Id,
        VendorName = v.VendorName,
        ContactPerson = v.ContactPerson,
        Phone = v.Phone,
        Email = v.Email,
        Address = v.Address,
        Notes = v.Notes,
        CreatedAtUtc = v.CreatedAtUtc,
        UpdatedAtUtc = v.UpdatedAtUtc
    };
}
