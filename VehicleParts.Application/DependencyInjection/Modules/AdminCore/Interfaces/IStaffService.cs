using VehicleParts.Application.Modules.AdminCore.DTOs;

namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

// defines business logic contract for staff management
public interface IStaffService
{
    Task<StaffResponseDto> RegisterStaffAsync(RegisterStaffDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffResponseDto>> GetAllStaffAsync(CancellationToken cancellationToken = default);
    Task<StaffResponseDto> GetStaffByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StaffResponseDto> UpdateStaffAsync(Guid id, UpdateStaffDto dto, CancellationToken cancellationToken = default);
    Task DeleteStaffAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StaffResponseDto> ToggleActiveStatusAsync(Guid id, CancellationToken cancellationToken = default);
}
