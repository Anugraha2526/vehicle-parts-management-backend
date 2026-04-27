using AutoMapper;
using vehicle_parts_management_backend.Application.DTOs.StaffDTOs;
using vehicle_parts_management_backend.Domain.Entities;

namespace vehicle_parts_management_backend.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // map register request to staff entity, ignore password hash since we set it manually
            CreateMap<RegisterStaffDto, Staff>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // map staff entity to response dto, convert role enum to a readable string
            CreateMap<Staff, StaffResponseDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            // map update fields onto an existing staff entity
            CreateMap<UpdateStaffDto, Staff>();
        }
    }
}
