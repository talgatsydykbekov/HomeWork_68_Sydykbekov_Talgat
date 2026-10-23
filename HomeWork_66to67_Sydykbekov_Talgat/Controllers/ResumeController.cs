using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
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
    private readonly IResumeService _resumeService;

    public ResumeController(
        IResumeService resumeService)
    {
        _resumeService = resumeService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View( await _resumeService.GetCreateResumeView());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateResumeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var errors = await _resumeService.CreateResume(model);
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        TempData["Success"] = "Резюме создано.";
        return RedirectToAction("Index", "User");
    }
    
   
    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var resume = await _resumeService.GetResumeDetails(id);
        if (resume == null)
            return NotFound();
        return View(resume);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var resume = await _resumeService.GetEditResumeView(id);
        ViewBag.ResumeId = resume.resumeId;

        return View(resume.Item1);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateResumeViewModel model)
    {
        
        if (!ModelState.IsValid)
        {
            await _resumeService.LoadCategories(model);
            ViewBag.ResumeId = id;
            return View(model);
        }
        var result = await _resumeService.EditAsync(id, model);
        if(result.RequiresLogin)
            return Challenge();
        if(!result.ResumeId.HasValue)
            return NotFound();
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
        if (!ModelState.IsValid)
        {
            await _resumeService.LoadCategories(model);
            ViewBag.ResumeId = result.ResumeId.Value;
            return View(model);
        }
        return RedirectToAction(
            nameof(Details),
            new { id = result.ResumeId.Value });
    }
    
    
    [Authorize(Roles = "User")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh(int id)
    {
        var result = await _resumeService.RefreshAsync(id);

        if (result.RequiresLogin == null)
            return Challenge();
        if (!result.Updated)
            return NotFound();
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
       var result =await _resumeService.SetPublication(id, isPublished);

        if (result.RequiresLogin == null)
            return Challenge();
        if (!result.SetPublic)
            return NotFound();
        
        TempData["Success"] = isPublished
            ? "Резюме опубликовано."
            : "Резюме снято с публикации.";

        return RedirectToAction("Index", "User");
    }
}