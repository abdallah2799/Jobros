using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Job
{
    public class CreateJobDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Requirements are required")]
        [StringLength(500, ErrorMessage = "Requirements can't be longer than 500 characters")]
        public string Requirements { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200, ErrorMessage = "Location can't be longer than 200 characters")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Salary range is required")]
        [StringLength(50, ErrorMessage = "Salary range can't be longer than 50 characters")]
        [RegularExpression(@"^\d{1,7}-\d{1,7}$",
        ErrorMessage = "Salary range must be in the format 'min-max', e.g., 5000-10000")]
        public string SalaryRange { get; set; }

        [Required(ErrorMessage = "Job type is required")]
        [StringLength(50, ErrorMessage = "Job type can't be longer than 50 characters")]
        public string JobType { get; set; }

        public bool IsActive { get; set; } = true; 

        [Required(ErrorMessage = "EmployerId is required")]
        public int EmployerId { get; set; }

        public int CategoryId { get; set; }
    }
}
