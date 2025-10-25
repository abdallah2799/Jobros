using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.EmployerDTOs
{
    public class EmployerDashboardStatsDto
    {
        public int TotalJobsPosted { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }

        public List<ApplicationsDTo> RecentApplications { get; set; } = new List<ApplicationsDTo>();
    }
}
