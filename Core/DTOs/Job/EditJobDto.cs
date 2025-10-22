using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Job
{
    public class EditJobDto
    {
        [Required(ErrorMessage = "Job Id is required")]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters")]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Requirements can't be longer than 500 characters")]
        public string Requirements { get; set; }

        [StringLength(200, ErrorMessage = "Location can't be longer than 200 characters")]
        public string Location { get; set; }

        [RegularExpression(@"^\d{1,7}-\d{1,7}$",
            ErrorMessage = "Salary range must be in the format 'min-max', e.g., 5000-10000")]
        public string SalaryRange { get; set; }

        [StringLength(50, ErrorMessage = "Job type can't be longer than 50 characters")]
        public string JobType { get; set; }

        public bool? IsActive { get; set; }
    }
}
