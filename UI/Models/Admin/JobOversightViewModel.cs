using Core.DTOs.Admin;
using Core.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectListItem
using System.Collections.Generic;

namespace UI.Models.Admin
{
    public class JobOversightViewModel
    {
        // The filtered list of jobs to display in the table
        public IEnumerable<JobAdminViewDTO> Jobs { get; set; }

        // The list of all employers to populate the filter dropdown
        public IEnumerable<SelectListItem> EmployerList { get; set; }

        // Properties to hold the current filter values from the form
        public int? SelectedEmployerId { get; set; }
        public bool? IsActiveFilter { get; set; }
    }
}