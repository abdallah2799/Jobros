using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Admin
{
    public class AnnouncementDTO
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; } // Will be treated as HTML
    }
}