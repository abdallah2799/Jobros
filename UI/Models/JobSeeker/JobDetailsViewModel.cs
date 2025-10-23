namespace UI.Models.JobSeeker
{
    public class JobDetailsViewModel
    {
        public Core.DTOs.Job.JobDto Job { get; set; }
        public bool AlreadyApplied { get; set; }
        public string CoverLetter { get; set; } = "";
    }
}
