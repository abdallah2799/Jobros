using System;

namespace Core.DTOs.Application
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int JobSeekerId { get; set; }
        public string JobSeekerName { get; set; }
        public string EmployerName { get; set; }

        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public string CoverLetter { get; set; }
        public string JobTitle { get; set; }
    }
}
