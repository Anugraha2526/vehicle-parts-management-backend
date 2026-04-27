using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Domain.Modules.AdminCore.Entities;

namespace VehicleParts.Application.Modules.AdminCore.Services;

public sealed class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IPasswordHasher _passwordHasher;

    public StaffService(IStaffRepository staffRepository, IPasswordHasher passwordHasher)
    {
        _staffRepository = staffRepository;
        _passwordHasher = passwordHasher;
    }

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
        return ToDto(created);
    }

    public async Task<IReadOnlyList<StaffResponseDto>> GetAllStaffAsync(CancellationToken cancellationToken = default)
    {
        var staffList = await _staffRepository.GetAllAsync(cancellationToken);
        return staffList.Select(ToDto).ToList();
    }

    public async Task<StaffResponseDto> GetStaffByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

        return ToDto(staffMember);
    }

    public async Task<StaffResponseDto> UpdateStaffAsync(Guid id, UpdateStaffDto dto, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

        if (dto.FullName != null) staffMember.FullName = dto.FullName;
        if (dto.Email != null) staffMember.Email = dto.Email;
        if (dto.Password != null) staffMember.PasswordHash = _passwordHasher.Hash(dto.Password);
        if (dto.Role != null) staffMember.Role = dto.Role.Value;
        if (dto.IsActive != null) staffMember.IsActive = dto.IsActive.Value;
        staffMember.Touch();

        var updated = await _staffRepository.UpdateAsync(staffMember, cancellationToken);
        return ToDto(updated);
    }

    public async Task DeleteStaffAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

        await _staffRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<StaffResponseDto> ToggleActiveStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

        staffMember.IsActive = !staffMember.IsActive;
        staffMember.Touch();

        var updated = await _staffRepository.UpdateAsync(staffMember, cancellationToken);
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
