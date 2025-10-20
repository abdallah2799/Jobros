using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public partial class Employer : ApplicationUser
    {
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool IsVerified { get; set; } = false;

        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
