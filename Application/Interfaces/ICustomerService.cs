using System.Collections.Generic;
using System.Threading.Tasks;
using vehicle_parts_management_backend.Application.DTOs;

namespace vehicle_parts_management_backend.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<int> RegisterCustomerAsync(RegisterCustomerDto dto);
        Task<CustomerProfileDto?> GetCustomerProfileAsync(int id);
        Task<List<CustomerProfileDto>> SearchCustomersAsync(string query);
        Task<bool> UpdateCustomerProfileAsync(int id, UpdateProfileDto dto);
        Task<bool> AddVehicleAsync(int customerId, VehicleDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    }
}
