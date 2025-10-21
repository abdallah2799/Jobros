using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Admin
{
    public class AdminSettingsDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

      
        public bool NotificationsEnabled { get; set; }

        [Required(ErrorMessage = "Language is required")]
        public string Language { get; set; }
    }
}
