using Microsoft.Extensions.Logging;
using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Services;

public sealed class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<StaffService> _logger;

    public StaffService(
        IStaffRepository staffRepository,
        IPasswordHasher passwordHasher,
        ILogger<StaffService> logger)
    {
        _staffRepository = staffRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    // get all staff members ordered by most recently created
    public async Task<IReadOnlyList<StaffResponseDto>> GetAllStaffAsync(CancellationToken cancellationToken = default)
    {
        var staffList = await _staffRepository.GetAllAsync(cancellationToken);
        return staffList.Select(ToDto).ToList();
    }

    // get single staff member, throw if not found
    public async Task<StaffResponseDto> GetStaffByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff member with id '{id}' was not found.");

        return ToDto(staffMember);
    }

    // check email uniqueness before creating, hash password before saving
    public async Task<StaffResponseDto> RegisterStaffAsync(RegisterStaffDto dto, CancellationToken cancellationToken = default)
    {
        if (await _staffRepository.EmailExistsAsync(dto.Email, cancellationToken))
            throw new ArgumentException($"Email '{dto.Email}' is already registered.");

        var staffMember = new StaffMember
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            Role = dto.Role
        };

        var created = await _staffRepository.CreateAsync(staffMember, cancellationToken);
        _logger.LogInformation("staff member created with id {Id}", created.Id);
        return ToDto(created);
    }

    // update only fields that were actually provided
    public async Task<StaffResponseDto> UpdateStaffAsync(Guid id, UpdateStaffDto dto, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff member with id '{id}' was not found.");

        // check email uniqueness only when email is changing
        if (dto.Email != null && !string.Equals(staffMember.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _staffRepository.EmailExistsAsync(dto.Email, cancellationToken))
                throw new ArgumentException($"Email '{dto.Email}' is already registered.");
        }

        if (dto.FullName != null) staffMember.FullName = dto.FullName;
        if (dto.Email != null) staffMember.Email = dto.Email;
        if (dto.Password != null) staffMember.PasswordHash = _passwordHasher.Hash(dto.Password);
        if (dto.Role != null) staffMember.Role = dto.Role.Value;
        if (dto.IsActive != null) staffMember.IsActive = dto.IsActive.Value;
        staffMember.Touch();

        var updated = await _staffRepository.UpdateAsync(staffMember, cancellationToken);
        _logger.LogInformation("staff member {Id} updated", updated.Id);
        return ToDto(updated);
    }

    // verify staff exists before deleting
    public async Task DeleteStaffAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff member with id '{id}' was not found.");

        await _staffRepository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("staff member {Id} deleted", id);
    }

    // toggle active status between true and false
    public async Task<StaffResponseDto> ToggleActiveStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff member with id '{id}' was not found.");

        staffMember.IsActive = !staffMember.IsActive;
        staffMember.Touch();

        var updated = await _staffRepository.UpdateAsync(staffMember, cancellationToken);
        _logger.LogInformation("staff member {Id} active status toggled to {Status}", updated.Id, updated.IsActive);
        return ToDto(updated);
    }

    private static StaffResponseDto ToDto(StaffMember s) => new()
    {
        Id = s.Id,
        FullName = s.FullName,
        Email = s.Email,
        Role = s.Role.ToString(),
        IsActive = s.IsActive,
        CreatedAtUtc = s.CreatedAtUtc,
        UpdatedAtUtc = s.UpdatedAtUtc
    };
}
