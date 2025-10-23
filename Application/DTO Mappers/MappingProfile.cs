using AutoMapper;
using Core.DTOs.Admin;
using Core.DTOs.Application;
using Core.DTOs.Auth;
using Core.DTOs.EmployerDTOs;
using Core.DTOs.Job;
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

            CreateMap<Job, JobAdminViewDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Employer.CompanyName));

            CreateMap<Core.Entities.Application, ApplicationAdminViewDTO>()
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => src.JobSeeker.FullName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Job.Employer.CompanyName));

            CreateMap<Employer, EmployerDropdownDTO>();


            // mapping Employer to EmployerDTO
            CreateMap<Core.Entities.Application, ApplicationsDTo>()
              .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
              .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => src.JobSeeker.FullName))
              .ForMember(dest => dest.ApplicantEmail, opt => opt.MapFrom(src => src.JobSeeker.Email));

            CreateMap<Core.Entities.Application, ApplicationDetailsDto>()
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => src.JobSeeker.FullName))
                .ForMember(dest => dest.ApplicantEmail, opt => opt.MapFrom(src => src.JobSeeker.Email))
                .ForMember(dest => dest.CvFilePath, opt => opt.MapFrom(src => src.JobSeeker.ResumeUrl));

            CreateMap<Employer, EmployerProfileDto>().ReverseMap();
            CreateMap<EditEmployerProfileDto, Employer>().ReverseMap();

            CreateMap<CreateJobDto, Job>().ReverseMap();
            CreateMap<EditJobDto, Job>().ReverseMap();

        }
    }
}

