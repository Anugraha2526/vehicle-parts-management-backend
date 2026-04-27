using AutoMapper;
using vehicle_parts_management_backend.Application.DTOs.StaffDTOs;
using vehicle_parts_management_backend.Application.Interfaces;
using vehicle_parts_management_backend.Domain.Entities;

namespace vehicle_parts_management_backend.Application.Services
{
    // handles all business logic for staff management
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public StaffService(IStaffRepository staffRepository, IMapper mapper)
        {
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        // register a new staff member with a hashed password
        public async Task<StaffResponseDto> RegisterStaffAsync(RegisterStaffDto dto)
        {
            // stop registration if the email is already taken
            if (await _staffRepository.EmailExistsAsync(dto.Email))
                throw new ArgumentException($"Email '{dto.Email}' is already registered.");

            // map the dto fields to a staff entity
            var staff = _mapper.Map<Staff>(dto);

            // hash the plain text password before saving
            staff.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var created = await _staffRepository.CreateAsync(staff);
            return _mapper.Map<StaffResponseDto>(created);
        }

        // get all staff members from the database
        public async Task<IEnumerable<StaffResponseDto>> GetAllStaffAsync()
        {
            var staffList = await _staffRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StaffResponseDto>>(staffList);
        }

        // get a single staff member, throw error if not found
        public async Task<StaffResponseDto> GetStaffByIdAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

            return _mapper.Map<StaffResponseDto>(staff);
        }

        // update a staff member's name, role, or active status — only overwrites fields that are present in the request
        public async Task<StaffResponseDto> UpdateStaffAsync(Guid id, UpdateStaffDto dto)
        {
            var staff = await _staffRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

            if (dto.FullName != null) staff.FullName = dto.FullName;
            if (dto.Email != null) staff.Email = dto.Email;
            if (dto.Password != null) staff.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (dto.Role != null) staff.Role = dto.Role.Value;
            if (dto.IsActive != null) staff.IsActive = dto.IsActive.Value;
            staff.UpdatedAt = DateTime.UtcNow;

            var updated = await _staffRepository.UpdateAsync(staff);
            return _mapper.Map<StaffResponseDto>(updated);
        }

        // delete a staff member after confirming they exist
        public async Task DeleteStaffAsync(Guid id)
        {
            _ = await _staffRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

            await _staffRepository.DeleteAsync(id);
        }

        // flip the staff active status from true to false or false to true
        public async Task<StaffResponseDto> ToggleActiveStatusAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Staff with id '{id}' was not found.");

            staff.IsActive = !staff.IsActive;
            staff.UpdatedAt = DateTime.UtcNow;

            var updated = await _staffRepository.UpdateAsync(staff);
            return _mapper.Map<StaffResponseDto>(updated);
        }
    }
}
