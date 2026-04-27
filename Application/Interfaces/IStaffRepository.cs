using vehicle_parts_management_backend.Domain.Entities;

namespace vehicle_parts_management_backend.Application.Interfaces
{
    // contract for all database operations related to staff
    public interface IStaffRepository
    {
        // get a staff member by their unique id
        Task<Staff?> GetByIdAsync(Guid id);

        // get a staff member by their email address
        Task<Staff?> GetByEmailAsync(string email);

        // get every staff member in the database
        Task<IEnumerable<Staff>> GetAllAsync();

        // save a new staff member to the database
        Task<Staff> CreateAsync(Staff staff);

        // update an existing staff member in the database
        Task<Staff> UpdateAsync(Staff staff);

        // remove a staff member from the database permanently
        Task DeleteAsync(Guid id);

        // check if an email is already used by another staff
        Task<bool> EmailExistsAsync(string email);
    }
}
