using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
[Authorize(Roles = "Employer")]
public class VacancyController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public VacancyController(
        MyUserContext context,
        UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateVacancyViewModel();

        await LoadCategories(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateVacancyViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (model.ExperienceFrom.HasValue &&
            model.ExperienceTo.HasValue &&
            model.ExperienceFrom.Value > model.ExperienceTo.Value)
        {
            ModelState.AddModelError(
                nameof(model.ExperienceTo),
                "Максимальный опыт не может быть меньше минимального.");
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

            return View(model);
        }

        var now = DateTime.UtcNow;

        var vacancy = new Vacancy
        {
            Title = model.Title.Trim(),
            Salary = model.Salary!.Value,
            Description = model.Description.Trim(),
            ExperienceFrom = model.ExperienceFrom!.Value,
            ExperienceTo = model.ExperienceTo!.Value,
            CategoryId = model.CategoryId!.Value,
            EmployerId = user.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsPublished = false
        };

        _context.Vacancies.Add(vacancy);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Вакансия создана.";

        return RedirectToAction("Index", "User");
    }

    private async Task LoadCategories(
        CreateVacancyViewModel model)
    {
        model.Categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.NameCategory)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            })
            .ToListAsync();
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .Include(v => v.Category)
            .FirstOrDefaultAsync(v =>
                v.Id == id &&
                v.EmployerId == user.Id);

        if (vacancy == null)
            return NotFound();

        return View(vacancy);
    }
    
    [HttpGet]
public async Task<IActionResult> Edit(int id)
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
        return Challenge();

    var vacancy = await _context.Vacancies
        .AsNoTracking()
        .FirstOrDefaultAsync(v =>
            v.Id == id &&
            v.EmployerId == user.Id);

    if (vacancy == null)
        return NotFound();

    var model = new CreateVacancyViewModel
    {
        Title = vacancy.Title,
        Salary = vacancy.Salary,
        Description = vacancy.Description,
        ExperienceFrom = vacancy.ExperienceFrom,
        ExperienceTo = vacancy.ExperienceTo,
        CategoryId = vacancy.CategoryId
    };

    await LoadCategories(model);

    ViewBag.VacancyId = vacancy.Id;

    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    int id,
    CreateVacancyViewModel model)
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
        return Challenge();

    var vacancy = await _context.Vacancies
        .FirstOrDefaultAsync(v =>
            v.Id == id &&
            v.EmployerId == user.Id);

    if (vacancy == null)
        return NotFound();

    if (model.ExperienceFrom.HasValue &&
        model.ExperienceTo.HasValue &&
        model.ExperienceFrom.Value > model.ExperienceTo.Value)
    {
        ModelState.AddModelError(
            nameof(model.ExperienceTo),
            "Максимальный опыт не может быть меньше минимального.");
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

        ViewBag.VacancyId = vacancy.Id;

        return View(model);
    }

    vacancy.Title = model.Title.Trim();
    vacancy.Salary = model.Salary!.Value;
    vacancy.Description = model.Description.Trim();
    vacancy.ExperienceFrom = model.ExperienceFrom!.Value;
    vacancy.ExperienceTo = model.ExperienceTo!.Value;
    vacancy.CategoryId = model.CategoryId!.Value;
    vacancy.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return RedirectToAction(
        nameof(Details),
        new { id = vacancy.Id });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Refresh(int id)
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
        return Challenge();

    var vacancy = await _context.Vacancies
        .FirstOrDefaultAsync(v =>
            v.Id == id &&
            v.EmployerId == user.Id);

    if (vacancy == null)
        return NotFound();

    vacancy.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    TempData["Success"] = "Дата обновления вакансии изменена.";

    return RedirectToAction("Index", "User");
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SetPublication(
    int id,
    bool isPublished)
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
        return Challenge();

    var vacancy = await _context.Vacancies
        .FirstOrDefaultAsync(v =>
            v.Id == id &&
            v.EmployerId == user.Id);

    if (vacancy == null)
        return NotFound();

    // Дату меняем только при переходе к публикации.
    if (isPublished && !vacancy.IsPublished)
    {
        vacancy.PublishedAt = DateTime.UtcNow;
    }

    vacancy.IsPublished = isPublished;

    await _context.SaveChangesAsync();

    TempData["Success"] = isPublished
        ? "Вакансия опубликована."
        : "Вакансия снята с публикации.";

    return RedirectToAction("Index", "User");
}
}