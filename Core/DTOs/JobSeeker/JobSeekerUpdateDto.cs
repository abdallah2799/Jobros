using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.JobSeeker
{
    public class JobSeekerUpdateDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string Bio { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Skills cannot exceed 300 characters.")]
        public string Skills { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50.")]
        public int? ExperienceYears { get; set; }
    }
}