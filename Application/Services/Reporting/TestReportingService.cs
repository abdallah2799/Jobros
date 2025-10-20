using Core.Interfaces.IUnitOfWorks;
using Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Reporting
{
    public class TestReportingService : ITestReportingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TestReportingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<object>> GetApplicantsReportAsync()
        {
            // load Applications including related Job and JobSeeker entities
            var applications = await _unitOfWork.Applications
                                                .GetAllAsync("Job,JobSeeker");

            var result = applications.Select(a => new
            {
                ApplicationId = a.Id,
                ApplicantName = a.JobSeeker?.FullName,
                JobTitle = a.Job?.Title,
                Status = a.Status,
                AppliedDate = a.AppliedAt
            }).ToList();

            return result;
        }
    }
}
