using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Admin
{
    public class DashboardStatsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalEmployers { get; set; }
        public int TotalJobSeekers { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }

        public Dictionary<string, int> JobsPerCategory { get; set; } = new Dictionary<string, int>();


        public Dictionary<string, int> UsersPerRole { get; set; } = new Dictionary<string, int>();
    }

    public class CategoryStatsDTO
    {
        public string CategoryName { get; set; }
        public int JobCount { get; set; }
    }

    public class RoleStatsDTO
    {
        public string RoleName { get; set; }
        public int Count { get; set; }
    }
}

