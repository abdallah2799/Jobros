using Core.DTOs.Auth; // Where your UserResponseDTO lives
using System.Collections.Generic;

namespace UI.Models.Admin
{
    public class UserManagementViewModel
    {
        public IEnumerable<UserResponseDTO> AllUsers { get; set; }
        public IEnumerable<UserResponseDTO> UnverifiedEmployers { get; set; }
    }
}