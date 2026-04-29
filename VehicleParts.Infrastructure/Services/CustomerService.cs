using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleParts.Application.DTOs;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Modules.CustomerCRM.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> RegisterCustomerAsync(RegisterCustomerDto dto)
        {
            var passwordHash = string.IsNullOrEmpty(dto.Password) ? "" : BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                PasswordHash = passwordHash,
                Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(dto.VehicleNumber))
            {
                var vehicle = new Vehicle
                {
                    UserId = user.Id,
                    VehicleNumber = dto.VehicleNumber,
                    Make = dto.Make,
                    Model = dto.Model,
                    Year = dto.Year
                };
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }

            return user.Id;
        }

        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Vehicles)
                .Include(u => u.Transactions)
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == "Customer");

            if (user == null) return null;

            return new CustomerProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CreatedAt = user.CreatedAtUtc,
                Vehicles = user.Vehicles.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    VehicleNumber = v.VehicleNumber,
                    Make = v.Make,
                    Model = v.Model,
                    Year = v.Year
                }).ToList(),
                Transactions = user.Transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Type = t.Type,
                    TotalAmount = t.TotalAmount,
                    Date = t.Date,
                    Description = t.Description
                }).ToList()
            };
        }

        public async Task<List<CustomerProfileDto>> SearchCustomersAsync(string query)
        {
            var queryable = _context.Users
                .Include(u => u.Vehicles)
                .Where(u => u.Role.ToLower() == "customer");

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                queryable = queryable.Where(u => 
                    u.FullName.ToLower().Contains(lowerQuery) || 
                    u.PhoneNumber.Contains(query) || 
                    u.Id.ToString() == query || 
                    u.Vehicles.Any(v => v.VehicleNumber.ToLower().Contains(lowerQuery))
                );
            }

            var users = await queryable.ToListAsync();

            return users.Select(user => new CustomerProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Vehicles = user.Vehicles.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    VehicleNumber = v.VehicleNumber,
                    Make = v.Make,
                    Model = v.Model,
                    Year = v.Year
                }).ToList()
            }).ToList();
        }

        public async Task<bool> UpdateCustomerProfileAsync(Guid id, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Role != "Customer") return false;

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddVehicleAsync(Guid customerId, VehicleDto dto)
        {
            var user = await _context.Users.FindAsync(customerId);
            if (user == null || user.Role.ToLower() != "customer") return false;

            var vehicle = new Vehicle
            {
                UserId = customerId,
                VehicleNumber = dto.VehicleNumber,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (user == null) return null;

            bool isPasswordCorrect = false;
            try {
                isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            } catch {
                isPasswordCorrect = user.PasswordHash == dto.Password;
            }

            if (!isPasswordCorrect) return null;

            return new LoginResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = user.Role,
                Token = "mock-jwt-token"
            };
        }
    }
}
