using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "User")]
public class JobApplicationController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public JobApplicationController(
        MyUserContext context,
        UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int vacancyId,
        int resumeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();
        var vacancyExists = await _context.Vacancies
            .AnyAsync(v =>
                v.Id == vacancyId &&
                v.IsPublished);
        if (!vacancyExists)
            return NotFound();
        var ownsResume = await _context.Resumes
            .AnyAsync(r =>
                r.Id == resumeId &&
                r.ApplicantId == user.Id);
        if (!ownsResume)
            return NotFound();
        var alreadyExists = await _context.JobApplications
            .AnyAsync(a =>
                a.VacancyId == vacancyId &&
                a.ResumeId == resumeId);
        if (alreadyExists)
        {
            TempData["Success"] =
                "Отклик с этим резюме уже существует.";
            return RedirectToAction(nameof(Index));
        }

        var application = new JobApplication
        {
            VacancyId = vacancyId,
            ResumeId = resumeId,
            InitiatedByUserId = user.Id,
            CratedAt = DateTime.UtcNow
        };
        _context.JobApplications.Add(application);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var duplicateExists = await _context.JobApplications
                .AsNoTracking()
                .AnyAsync(a =>
                    a.VacancyId == vacancyId &&
                    a.ResumeId == resumeId);
            if (!duplicateExists)
                throw;
            TempData["Success"] =
                "Отклик с этим резюме уже существует.";
            return RedirectToAction("Index", "User");
        }
        TempData["Success"] = "Отклик отправлен.";
        return RedirectToAction(
            "Index",
            "Chat",
            new { id = application.Id });
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();
        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Resume)
            .Include(a => a.Vacancy)
            .ThenInclude(v => v.Employer)
            .Where(a => a.Resume.ApplicantId == user.Id)
            .OrderByDescending(a => a.CratedAt)
            .ThenByDescending(a => a.Id)
            .ToListAsync();

        return View(applications);
    }
}