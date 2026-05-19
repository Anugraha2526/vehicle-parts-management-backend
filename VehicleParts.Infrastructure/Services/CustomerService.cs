using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration _configuration;

        public CustomerService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Guid> RegisterCustomerAsync(RegisterCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            var emailLower = dto.Email.Trim().ToLower();

            // Check email uniqueness across Users (Customers) and StaffMembers
            var emailExistsInUsers = await _context.Users.AnyAsync(u => u.Email.ToLower() == emailLower);
            var emailExistsInStaff = await _context.StaffMembers.AnyAsync(s => s.Email.ToLower() == emailLower);

            if (emailExistsInUsers || emailExistsInStaff)
            {
                throw new ArgumentException($"Email '{dto.Email}' is already registered.");
            }

            // Check phone number uniqueness across Customers (Users)
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                var phoneTrim = dto.PhoneNumber.Trim();
                var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == phoneTrim);
                if (phoneExists)
                {
                    throw new ArgumentException($"Phone number '{dto.PhoneNumber}' is already in use.");
                }
            }

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

        public async Task<bool> UpdateVehicleAsync(Guid customerId, Guid vehicleId, VehicleDto dto)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == customerId);
            if (vehicle == null) return false;

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Make = dto.Make;
            vehicle.Model = dto.Model;
            vehicle.Year = dto.Year;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVehicleAsync(Guid customerId, Guid vehicleId)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == customerId);
            if (vehicle == null) return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var emailLower = dto.Email.Trim().ToLower();

            // 1. Try to find in Users (Customers)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower);

            if (user != null)
            {
                bool isPasswordCorrect = false;
                try {
                    isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                } catch {
                    isPasswordCorrect = user.PasswordHash == dto.Password;
                }

                if (!isPasswordCorrect) return null;

                var token = GenerateJwtToken(user.Id, user.Email, user.FullName, user.Role);

                return new LoginResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Role = user.Role,
                    Token = token
                };
            }

            // 2. Try to find in StaffMembers (Admins / Staff)
            var staff = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.Email.ToLower() == emailLower);

            if (staff != null)
            {
                bool isPasswordCorrect = false;
                try {
                    isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, staff.PasswordHash);
                } catch {
                    isPasswordCorrect = staff.PasswordHash == dto.Password;
                }

                if (!isPasswordCorrect) return null;

                var roleStr = staff.Role.ToString(); // "Admin" or "Staff"
                var token = GenerateJwtToken(staff.Id, staff.Email, staff.FullName, roleStr);

                return new LoginResponseDto
                {
                    Id = staff.Id,
                    FullName = staff.FullName,
                    Role = roleStr,
                    Token = token
                };
            }

            return null;
        }

        private string GenerateJwtToken(Guid userId, string email, string fullName, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var keyStr = jwtSettings["Key"] ?? "CHANGE-THIS-TO-A-LONG-RANDOM-SECRET-KEY";
            var keyBytes = Encoding.UTF8.GetBytes(keyStr);
            var issuer = jwtSettings["Issuer"] ?? "chitospare";
            var audience = jwtSettings["Audience"] ?? "chitospare-users";

            var expiryInDays = 7;
            if (int.TryParse(jwtSettings["ExpiryInDays"], out var parsedDays))
            {
                expiryInDays = parsedDays;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role),
                new Claim("role", role)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(expiryInDays),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
