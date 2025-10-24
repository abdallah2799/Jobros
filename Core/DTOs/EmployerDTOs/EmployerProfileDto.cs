using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.EmployerDTOs
{
    public class EmployerProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
