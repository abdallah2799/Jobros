using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ITestReportingService
    {
        Task<IEnumerable<object>> GetApplicantsReportAsync();
        Task<IEnumerable<object>> GetApplicantsReportAsync(int JobId);
    }
}
