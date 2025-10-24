using Core.DTOs.Job;

namespace UI.Models.Employer
{
    public class EditJobViewModel
    {
        public int EmployerId { get; set; }
        public int JobId { get; set; }
        public EditJobDto Job { get; set; } = new EditJobDto();
    }
}
