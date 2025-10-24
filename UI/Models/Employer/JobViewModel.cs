using Core.DTOs.Job;

namespace UI.Models.Employer
{
    public class JobViewModel
    {
        public JobDto Job { get; set; } = new JobDto();
        public int EmployerId { get; set; }
    }
}
