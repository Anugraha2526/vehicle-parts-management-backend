using vehicle_parts_management_backend.Application.DTOs.StaffDTOs;

namespace vehicle_parts_management_backend.Application.Interfaces
{
    // contract for all business logic operations related to staff
    public interface IStaffService
    {
        // register a new staff member and return their info
        Task<StaffResponseDto> RegisterStaffAsync(RegisterStaffDto dto);

        // get a list of all staff members
        Task<IEnumerable<StaffResponseDto>> GetAllStaffAsync();

        // get a single staff member by their id
        Task<StaffResponseDto> GetStaffByIdAsync(Guid id);

        // update staff details like name or role
        Task<StaffResponseDto> UpdateStaffAsync(Guid id, UpdateStaffDto dto);

        // delete a staff member permanently
        Task DeleteStaffAsync(Guid id);

        // switch staff active status between active and inactive
        Task<StaffResponseDto> ToggleActiveStatusAsync(Guid id);
    }
}
