namespace UI.Models.JobSeeker
{
    public class BrowseJobsViewModel
    {
        public IEnumerable<Core.DTOs.Job.JobDto> Jobs { get; set; } = new List<Core.DTOs.Job.JobDto>();
        public string Keyword { get; set; } = "";
        public int? CategoryId { get; set; }
        public string Employer { get; set; } = "";
        public string Location { get; set; } = "";
        public string JobType { get; set; } = "";
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}
