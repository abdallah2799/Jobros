namespace UI.Models.JobSeeker
{
    public class ApplicationsViewModel
    {
        public IEnumerable<Core.DTOs.Application.ApplicationDto> Applications { get; set; } = new List<Core.DTOs.Application.ApplicationDto>();
    }
}

