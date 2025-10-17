using AutoMapper;
using Core.DTOs.Auth;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.DTO_Mappers
{
    public class RoleResolver : IValueResolver<ApplicationUser, UserResponseDTO, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleResolver(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string Resolve(ApplicationUser source, UserResponseDTO destination, string destMember, ResolutionContext context)
        {
            var roles = _userManager.GetRolesAsync(source).Result;
            return roles.FirstOrDefault() ?? "User";
        }
    }
}


