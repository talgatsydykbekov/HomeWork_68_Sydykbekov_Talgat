using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
public class EducationController : Controller
{
    private readonly IEducationService _educationService;
    public EducationController(IEducationService educationService)
    {
        _educationService = educationService;
    }
    [HttpGet]
    public async Task<IActionResult> Create(int resumeId)
    {
        var result = await _educationService.CreateView(resumeId);
        if (result == null)
        {
            return NotFound();
        }
        return View(result);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateEducationViewModel model)
    {
        var errors = _educationService.Validate(model);
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var newEducation = await _educationService.Create(model);
        
        return RedirectToAction(
            "Details",
            "Resume",
            new { id = model.ResumeId});
    }
}