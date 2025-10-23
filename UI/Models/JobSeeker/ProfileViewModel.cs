namespace UI.Models.JobSeeker
{
    public class ProfileViewModel
    {
        public Core.DTOs.JobSeeker.JobSeekerDto Profile { get; set; } = new();
        public IFormFile? ResumeFile { get; set; }
    }
}
