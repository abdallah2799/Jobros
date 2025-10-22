using System;

namespace Core.DTOs.Admin
{
    public class OverdueApplicationDTO
    {
        public int ApplicationId { get; set; }
        public string JobTitle { get; set; }
        public string ApplicantName { get; set; }
        public string CompanyName { get; set; } // The employer to contact
        public string EmployerEmail { get; set; } // For follow-up
        public DateTime AppliedAt { get; set; }
        public int DaysPending { get; set; }
    }
}