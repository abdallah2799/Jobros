namespace Core.DTOs.JobSeeker
{
    public class JobSeekerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Skills { get; set; }
        public int? ExperienceYears { get; set; }
        public string ResumeUrl { get; set; }
    }
}