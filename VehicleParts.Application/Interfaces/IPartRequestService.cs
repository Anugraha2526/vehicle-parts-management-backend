using VehicleParts.Application.DTOs.CustomerPortal;
using VehicleParts.Domain.Modules.CustomerPortal.Entities;

namespace VehicleParts.Application.Interfaces
{
    public interface IPartRequestService
    {
        Task<PartRequest> CreateAsync(CreatePartRequestDto dto);
Task<List<PartRequest>> GetByCustomerIdAsync(Guid customerId);    }
}