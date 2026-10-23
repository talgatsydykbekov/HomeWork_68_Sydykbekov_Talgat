using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "User")]
public class WorkExperienceController : Controller
{
    private readonly IWorkExperienceService _workExperienceService;

    public WorkExperienceController(
        IWorkExperienceService workExperienceService)
    {
        _workExperienceService = workExperienceService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int resumeId)
    {
        var result = await _workExperienceService
            .GetCreateModelAsync(resumeId);

        if (result.RequiresLogin)
            return Challenge();

        if (result.Model == null)
            return NotFound();

        return View(result.Model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateWorkExperienceViewModel model)
    {
        var access = await _workExperienceService
            .GetCreateModelAsync(model.ResumeId);

        if (access.RequiresLogin)
            return Challenge();

        if (access.Model == null)
            return NotFound();
        if (!ModelState.IsValid)
            return View(model);

        var result = await _workExperienceService.CreateAsync(model);
        if (result.RequiresLogin)
            return Challenge();

        if (!result.ResumeId.HasValue)
            return NotFound();

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
        if (!ModelState.IsValid)
            return View(model);

        return RedirectToAction(
            "Details",
            "Resume",
            new { id = result.ResumeId.Value });
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _workExperienceService
            .GetEditModelAsync(id);

        if (result.RequiresLogin)
            return Challenge();

        if (result.Model == null)
            return NotFound();
        ViewBag.ExperienceId = id;
        return View(result.Model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateWorkExperienceViewModel model)
    {
        var access = await _workExperienceService
            .GetEditModelAsync(id);

        if (access.RequiresLogin)
            return Challenge();

        if (access.Model == null)
            return NotFound();

        model.ResumeId = access.Model.ResumeId;
        ModelState.Remove(nameof(model.ResumeId));

        if (!ModelState.IsValid)
        {
            ViewBag.ExperienceId = id;
            return View(model);
        }

        var result = await _workExperienceService.EditAsync(id, model);

        if (result.RequiresLogin)
            return Challenge();

        if (!result.ResumeId.HasValue)
            return NotFound();

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ExperienceId = id;
            return View(model);
        }

        return RedirectToAction(
            "Details",
            "Resume",
            new { id = result.ResumeId.Value });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _workExperienceService.DeleteAsync(id);

        if (result.RequiresLogin)
            return Challenge();

        if (!result.ResumeId.HasValue)
            return NotFound();

        return RedirectToAction(
            "Details",
            "Resume",
            new { id = result.ResumeId.Value });
    }
}