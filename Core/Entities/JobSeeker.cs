using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class JobSeeker : User
    {
        public string Bio { get; set; }
        public string Skills { get; set; }
        public int? ExperienceYears { get; set; }
        public string ResumeUrl { get; set; }

        // Navigation property for applications submitted by this job seeker
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
