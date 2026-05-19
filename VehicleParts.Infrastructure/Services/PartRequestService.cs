using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.DTOs.CustomerPortal;
using VehicleParts.Application.Interfaces;
using VehicleParts.Infrastructure.Persistence;
using VehicleParts.Domain.Modules.CustomerPortal.Entities;


namespace VehicleParts.Infrastructure.Services
{
    public class PartRequestService : IPartRequestService
    {
        private readonly ApplicationDbContext _context;

        public PartRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PartRequest> CreateAsync(CreatePartRequestDto dto)
        {
            var entity = new PartRequest
            {
                PartId = dto.PartId,
                CustomerId = dto.CustomerId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                Total = dto.Quantity * dto.UnitPrice, // ✅ calculate safely
                PartName = dto.PartName
            };

            await _context.PartRequests.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

public async Task<List<PartRequest>> GetByCustomerIdAsync(Guid customerId)        {
            return await _context.PartRequests
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }
    }
}