using System;
using System.Collections.Generic;

namespace Core.DTOs.EmployerDTOs
{
    public class EmployerDashboardDto
    {
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }

        public List<ApplicationsDTo> RecentApplications { get; set; } = new List<ApplicationsDTo>();
    }

}
