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
using VehicleParts.Domain.Modules.AdminCore.Enums;
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

            // all roles (Customer, Staff, Admin) live in the Users table
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == emailLower);
            if (emailExists)
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
                Role = UserRole.Customer
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
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Customer);

            if (user == null) return null;

            return new CustomerProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address ?? string.Empty,
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
                .Where(u => u.Role == UserRole.Customer);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                queryable = queryable.Where(u => 
                    u.FullName.ToLower().Contains(lowerQuery) || 
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(query)) ||
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
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address ?? string.Empty,
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
            if (user == null || user.Role != UserRole.Customer) return false;

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddVehicleAsync(Guid customerId, VehicleDto dto)
        {
            var user = await _context.Users.FindAsync(customerId);
            if (user == null || user.Role != UserRole.Customer) return false;

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
            var emailLower = dto.Email.Trim().ToLower();

            // all roles (Customer, Staff, Admin) live in the Users table
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower);

            if (user == null) return null;

            bool isPasswordCorrect = false;
            try {
                isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            } catch {
                isPasswordCorrect = user.PasswordHash == dto.Password;
            }

            if (!isPasswordCorrect) return null;

            var role = user.Role.ToString();
            var token = GenerateJwtToken(user.Id, user.Email, user.FullName, role);

            return new LoginResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Role = role,
                Token = token
            };
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
