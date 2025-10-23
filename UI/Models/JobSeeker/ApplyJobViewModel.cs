namespace UI.Models.JobSeeker
{
    public class ApplyJobViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = "";
        public string CoverLetter { get; set; } = "";
        public IFormFile? ResumeFile { get; set; }
    }
}
