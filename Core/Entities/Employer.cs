using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Employer : User
    {
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        // Navigation property for jobs posted by this employer
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
