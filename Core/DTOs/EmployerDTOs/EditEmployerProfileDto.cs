using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.EmployerDTOs
{
    public class EditEmployerProfileDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Industry { get; set; }
        public string? Website { get; set; }
        public string Location { get; set; }

        public string? Description { get; set; }
    }
}
