using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "Employer")]
public class VacancyController : Controller
{
    private readonly IVacancyService _vacancyService;
    public VacancyController(IVacancyService vacancyService)
    {
        _vacancyService = vacancyService;
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await _vacancyService.GetCreateModelAsync());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVacancyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await _vacancyService.LoadCategoriesAsync(model);
            return View(model);
        }

        var result = await _vacancyService.CreateAsync(model);

        if (result.RequiresLogin)
            return Challenge();

        foreach (var error in result.Errors)
            ModelState.AddModelError(error.Key, error.Value);

        if (!ModelState.IsValid)
        {
            await _vacancyService.LoadCategoriesAsync(model);
            return View(model);
        }

        if (!result.VacancyId.HasValue)
            return NotFound();

        TempData["Success"] = "Вакансия создана.";
        return RedirectToAction("Index", "User");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _vacancyService.GetDetailsAsync(id);

        if (result.RequiresLogin)
            return Challenge();

        if (result.Vacancy == null)
            return NotFound();

        return View(result.Vacancy);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _vacancyService.GetEditModelAsync(id);

        if (result.RequiresLogin)
            return Challenge();

        if (result.Model == null)
            return NotFound();

        ViewBag.VacancyId = id;
        return View(result.Model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateVacancyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var access = await _vacancyService.GetEditModelAsync(id);

            if (access.RequiresLogin)
                return Challenge();

            if (access.Model == null)
                return NotFound();
            model.Categories = access.Model.Categories;
            ViewBag.VacancyId = id;
            return View(model);
        }

        var result = await _vacancyService.EditAsync(id, model);

        if (result.RequiresLogin)
            return Challenge();

        if (!result.VacancyId.HasValue)
            return NotFound();

        foreach (var error in result.Errors)
            ModelState.AddModelError(error.Key, error.Value);

        if (!ModelState.IsValid)
        {
            await _vacancyService.LoadCategoriesAsync(model);
            ViewBag.VacancyId = id;
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = result.VacancyId.Value });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh(int id)
    {
        var result = await _vacancyService.RefreshAsync(id);

        if (result.RequiresLogin)
            return Challenge();

        if (!result.Updated)
            return NotFound();

        TempData["Success"] = "Дата обновления вакансии изменена.";
        return RedirectToAction("Index", "User");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPublication(int id, bool isPublished)
    {
        var result = await _vacancyService.SetPublicationAsync(id, isPublished);

        if (result.RequiresLogin)
            return Challenge();

        if (!result.Updated)
            return NotFound();

        TempData["Success"] = isPublished
            ? "Вакансия опубликована."
            : "Вакансия снята с публикации.";

        return RedirectToAction("Index", "User");
    }
}