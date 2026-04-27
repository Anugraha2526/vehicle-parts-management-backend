using Microsoft.EntityFrameworkCore;
using vehicle_parts_management_backend.Application.Interfaces;
using vehicle_parts_management_backend.Domain.Entities;
using vehicle_parts_management_backend.Infrastructure.Data;

namespace vehicle_parts_management_backend.Infrastructure.Repositories
{
    // talks directly to the database for all staff-related queries
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // find one staff member using their id
        public async Task<Staff?> GetByIdAsync(Guid id)
        {
            return await _context.Staff.FindAsync(id);
        }

        // find one staff member using their email
        public async Task<Staff?> GetByEmailAsync(string email)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        // get all staff records from the database
        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staff
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        // add a new staff record and save to database
        public async Task<Staff> CreateAsync(Staff staff)
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        // update an existing staff record and save changes
        public async Task<Staff> UpdateAsync(Staff staff)
        {
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        // delete a staff record from the database
        public async Task DeleteAsync(Guid id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff is not null)
            {
                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();
            }
        }

        // return true if any staff already uses this email
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Staff.AnyAsync(s => s.Email == email);
        }
    }
}
