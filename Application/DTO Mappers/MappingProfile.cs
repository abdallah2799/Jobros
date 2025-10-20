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

            // Job -> JobDto
            CreateMap<Core.Entities.Job, Core.DTOs.Job.JobDto>()
                .ForMember(d => d.EmployerName, opt => opt.MapFrom(s => s.Employer.CompanyName ?? s.Employer.FullName));

            // Application -> ApplicationDto
            CreateMap<Core.Entities.Application, Core.DTOs.Application.ApplicationDto>()
                .ForMember(d => d.JobTitle, opt => opt.MapFrom(s => s.Job.Title));

            // JobSeeker -> JobSeekerDto
            CreateMap<Core.Entities.JobSeeker, Core.DTOs.JobSeeker.JobSeekerDto>();
        }
    }
}

