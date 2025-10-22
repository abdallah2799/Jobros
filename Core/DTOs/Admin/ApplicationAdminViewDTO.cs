using System;

namespace Core.DTOs.Admin
{
    public class ApplicationAdminViewDTO
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string ApplicantName { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}