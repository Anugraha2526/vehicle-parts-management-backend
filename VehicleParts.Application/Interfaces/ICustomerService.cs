using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleParts.Application.DTOs;

namespace VehicleParts.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Guid> RegisterCustomerAsync(RegisterCustomerDto dto);
        Task<CustomerProfileDto?> GetCustomerProfileAsync(Guid id);
        Task<List<CustomerProfileDto>> SearchCustomersAsync(string query);
        Task<bool> UpdateCustomerProfileAsync(Guid id, UpdateProfileDto dto);
        Task<bool> AddVehicleAsync(Guid customerId, VehicleDto dto);
        Task<bool> UpdateVehicleAsync(Guid customerId, Guid vehicleId, VehicleDto dto);
        Task<bool> DeleteVehicleAsync(Guid customerId, Guid vehicleId);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    }
}
