using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.JobSeeker
{
    public class JobSeekerDashboardStatsDto
    {
        public string JobSeekerName { get; set; }
        public string TopAppliedJobTitle { get; set; }
        public int TopAppliedJobApplicationsCount { get; set; }
        public int TotalJobsAppliedFor { get; set; }
        public int AcceptedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int PendingApplications { get; set; }
    }
}
