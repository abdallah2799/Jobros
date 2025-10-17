using AutoMapper;
using Core.DTOs.Auth;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.DTO_Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // From UserRegisterDTO to ApplicationUser variants
            CreateMap<UserRegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<UserRegisterDTO, Employer>()
                .IncludeBase<UserRegisterDTO, ApplicationUser>();

            CreateMap<UserRegisterDTO, JobSeeker>()
                .IncludeBase<UserRegisterDTO, ApplicationUser>();

            CreateMap<UserRegisterDTO, Admin>()
                .IncludeBase<UserRegisterDTO, ApplicationUser>();

            // From ApplicationUser (or derived) to UserResponseDTO
            CreateMap<ApplicationUser, UserResponseDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom<RoleResolver>()); 
        }
    }
}

