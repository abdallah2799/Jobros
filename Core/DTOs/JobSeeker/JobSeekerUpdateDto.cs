namespace Core.DTOs.JobSeeker
{
    public class JobSeekerUpdateDto
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? Skills { get; set; }
        public int? ExperienceYears { get; set; }
        public string? ResumeUrl { get; set; }
    }
}
