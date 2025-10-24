using Core.DTOs.EmployerDTOs;

namespace UI.Models.Employer
{
    public class EditEmployerProfileViewModel
    {
        public int EmployerId { get; set; }
        public EditEmployerProfileDto Profile { get; set; } = new EditEmployerProfileDto();
    }
}
