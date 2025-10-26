using Core.Interfaces.IServices.IQueries;
using Core.Interfaces.IServices.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UI.Models.JobSeeker;

namespace UI.Controllers
{
    [Route("Jobs")]
    public class JobsController : Controller
    {
        private readonly IJobSeekerQueryService _queryService;


        public JobsController(IJobSeekerQueryService queryService)
        {
            _queryService = queryService;
        }


        private int? CurrentUserId
        {
            get
            {
                if (User?.Identity?.IsAuthenticated ?? false)
                {
                    return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                }
                return null;
            }
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string keyword = null, int? categoryId = null, string employer = null, string location = null, string jobType = null, int page = 1)
        {
            var jobs = await _queryService.GetActiveJobsAsync(keyword, categoryId, employer, location, jobType, page, 10);
            var totalCount = await _queryService.GetActiveJobsTotalCountAsync(keyword, categoryId, employer, location, jobType);
            var totalPages = (int)Math.Ceiling(totalCount / 10.0);

            var model = new BrowseJobsViewModel
            {
                Jobs = jobs,
                Keyword = keyword ?? "",
                CategoryId = categoryId,
                Employer = employer ?? "",
                Location = location ?? "",
                JobType = jobType ?? "",
                Page = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _queryService.GetJobByIdAsync(id);
            if (job == null) return NotFound();


            var alreadyApllied = false;
           

            if (CurrentUserId.HasValue)
            {
                alreadyApllied = await _queryService.HasAppliedAsync(CurrentUserId.Value, id);
            }


            var model = new JobDetailsViewModel
            {
                Job = job,
                AlreadyApplied = alreadyApllied
            };
 

            return View(model);
        }
    }
}
