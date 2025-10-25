using Core.DTOs.Job;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace UI.Models.Employer
{
    public class CreateJobViewModel
    {
        public int EmployerId { get; set; }
        public CreateJobDto Job { get; set; } = new CreateJobDto();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();


    }
}
