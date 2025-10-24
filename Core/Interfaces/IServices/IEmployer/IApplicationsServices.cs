
using Core.DTOs.EmployerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices.IEmployer
{
    public interface IApplicationsServices
    {
        Task<IEnumerable<ApplicationsDTo>> GetApplicationsByEmployerAsync(int employerId, string? jobTitle = null, string? applicantName = null);
        Task<ApplicationDetailsDto?> GetApplicationDetailsAsync(int applicationId, int employerId);
        Task<bool> AcceptApplicationAsync(int applicationId, int employerId);
        Task<bool> RejectApplicationAsync(int applicationId, int employerId);
        Task<byte[]> ExportApplicantsAsync(int jobId, string format);
    }
}
