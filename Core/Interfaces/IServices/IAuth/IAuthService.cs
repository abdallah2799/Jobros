using Core.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IAuth
{
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterAsync(UserRegisterDTO dto);
        Task<UserResponseDTO> LoginAsync(UserLoginDTO dto);
        Task ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> EmailExistsAsync(string email);
        Task LogoutAsync();
    }
}
