using Core.DTOs.EmployerDTOs;

namespace UI.Models.Employer
{
    // Optional viewmodel wrapper for the employer profile
    public class EmployerProfileViewModel
    {
        public int EmployerId { get; set; }
        public EmployerProfileDto Profile { get; set; } = new EmployerProfileDto();
    }
}
