using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "User")]
public class JobApplicationController : Controller
{
    private readonly IJobApplicationService _jobApplicationService;
    public JobApplicationController(
        IJobApplicationService jobApplicationService)
    {
        _jobApplicationService = jobApplicationService;
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int vacancyId,
        int resumeId)
    {
        var result = await _jobApplicationService.CreateJobApplication(vacancyId, resumeId);
        if(result.RequiresLogin)
            return Challenge();
        if (result.alreadyExists)
        {
            TempData["Success"] =
                "Отклик с этим резюме уже существует.";
            return RedirectToAction(nameof(Index));
        }
        
        if(!result.applicationnId.HasValue)
            return NotFound();
        TempData["Success"] = "Отклик отправлен.";
        return RedirectToAction(
                "Index",
                "Chat",
                new { id = result.applicationnId.Value });
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var applications = await _jobApplicationService.GetAllJobApplications();
        if (applications == null)
            return NotFound();
        return View(applications);
    }
}