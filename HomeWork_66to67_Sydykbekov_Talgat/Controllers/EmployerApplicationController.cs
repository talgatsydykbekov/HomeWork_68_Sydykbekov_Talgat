using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "Employer")]
public class EmployerApplicationController : Controller
{
    private readonly IEmployerApplicationService _employerApplicationService;

    public EmployerApplicationController(IEmployerApplicationService employerApplicationService)
    {
        _employerApplicationService = employerApplicationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int vacancyId)
    {
        var model = await  _employerApplicationService.GetAllApplications(vacancyId);
        if (model == null)
            return NotFound();
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Resume(int id)
    {
        var result = await  _employerApplicationService.GetResumeById(id);
        if (result?.resume == null)
        {
            return NotFound();
        }
        return View(
            "~/Views/ResumeCatalog/Details.cshtml",
            result);
        // return View ("~/Views/ResumeCatalog/Details.cshtml",new ResumeDetailsViewModel
        // {
        //     Vacancies = null,
        //     resume = result.resume
        // });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(
        int resumeId,
        int vacancyId)
    {
        var result = await _employerApplicationService
            .StartAsync(resumeId, vacancyId);
        if(result.RequiresLogin)
            return RedirectToAction("Login", "Account");
        if (result.ApplicationId == null)
            return NotFound();
        return RedirectToAction(
            "Index",
            "Chat",
            new { id = result.ApplicationId });
    }
}