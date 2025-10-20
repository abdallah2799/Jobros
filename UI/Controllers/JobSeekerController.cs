using Core.DTOs.Application;
using Core.DTOs.Job;
using Core.DTOs.JobSeeker;
using Core.Interfaces.IServices.Commands;
using Core.Interfaces.IServices.IQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobSeekerController : Controller
    {
    }
}
