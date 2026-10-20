using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "User")]
public class ResumeController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public ResumeController(MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateResumeViewModel();
        await LoadCategories(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateResumeViewModel model)
    {
        if (model.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == model.CategoryId.Value);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(model.CategoryId),"Выбранная категория не существует");
            }
        }
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(
                nameof(model.Title),
                "Введите должность.");
        }

        if (!ModelState.IsValid)
        {
            await LoadCategories(model);
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var now = DateTime.UtcNow;

        var resume = new Resume
        {
            Title = model.Title.Trim(),
            ExpectedSalary = model.ExpectedSalary!.Value,
            CategoryId = model.CategoryId!.Value,
            ApplicantId = user.Id,

            Telegram = model.Telegram?.Trim(),
            Email = model.Email?.Trim(),
            PhoneNumber = model.PhoneNumber?.Trim(),
            FacebookUrl = model.FacebookUrl?.Trim(),
            LinkedInUrl = model.LinkedInUrl?.Trim(),

            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Резюме создано.";

        return RedirectToAction("Index", "User");
    }
    
    private async Task LoadCategories(CreateResumeViewModel model)
    {
        model.Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.NameCategory)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            }).ToListAsync();
    }
    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.WorkExperiences)
            .Include(r => r.Educations)
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return NotFound();

        return View(resume);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var resume = await _context.Resumes
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return NotFound();

        var model = new CreateResumeViewModel
        {
            Title = resume.Title,
            ExpectedSalary = resume.ExpectedSalary,
            CategoryId = resume.CategoryId,

            Telegram = resume.Telegram,
            Email = resume.Email,
            PhoneNumber = resume.PhoneNumber,
            FacebookUrl = resume.FacebookUrl,
            LinkedInUrl = resume.LinkedInUrl
        };

        await LoadCategories(model);

        ViewBag.ResumeId = resume.Id;

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateResumeViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(
                nameof(model.Title),
                "Введите должность.");
        }

        if (model.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == model.CategoryId.Value);

            if (!categoryExists)
            {
                ModelState.AddModelError(
                    nameof(model.CategoryId),
                    "Выбранная категория не существует.");
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadCategories(model);

            ViewBag.ResumeId = resume.Id;

            return View(model);
        }

        resume.Title = model.Title.Trim();
        resume.ExpectedSalary = model.ExpectedSalary!.Value;
        resume.CategoryId = model.CategoryId!.Value;
        resume.UpdatedAt = DateTime.UtcNow;
        resume.Telegram = model.Telegram?.Trim();
        resume.Email = model.Email?.Trim();
        resume.PhoneNumber = model.PhoneNumber?.Trim();
        resume.FacebookUrl = model.FacebookUrl?.Trim();
        resume.LinkedInUrl = model.LinkedInUrl?.Trim();

        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(Details),
            new { id = resume.Id });
    }
    [Authorize(Roles = "User")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return NotFound();

        resume.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Дата обновления резюме изменена.";

        return RedirectToAction("Index", "User");
    }
    [Authorize(Roles = "User")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPublication(
        int id,
        bool isPublished)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return NotFound();

        resume.IsPublished = isPublished;

        await _context.SaveChangesAsync();

        TempData["Success"] = isPublished
            ? "Резюме опубликовано."
            : "Резюме снято с публикации.";

        return RedirectToAction("Index", "User");
    }
}