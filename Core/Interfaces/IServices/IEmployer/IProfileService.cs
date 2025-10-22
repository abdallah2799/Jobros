using Core.DTOs.EmployerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IEmployer
{
    public interface IProfileService
    {
        Task<EmployerProfileDto?> GetProfileAsync(int employerId);
        Task<bool> UpdateProfileAsync(int employerId, EditEmployerProfileDto model);
        Task<bool> ChangePasswordAsync(int employerId, string currentPassword, string newPassword);
        Task<bool> ChangePasswordAsync(int employerId, ChangePasswordDto model);
    }
}
