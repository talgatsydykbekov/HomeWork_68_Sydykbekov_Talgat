using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
[Authorize(Roles = "Employer")]
public class ResumeCatalogController : Controller
{
    private IResumeCatalogService _resumeCatalogService;

    public ResumeCatalogController(IResumeCatalogService resumeCatalogService)
    {
        _resumeCatalogService = resumeCatalogService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? categoryId)
    {
       var model = await _resumeCatalogService.GetAllResumeListAsync(categoryId);
       if(model == null)
           return NotFound();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        var result = await  _resumeCatalogService.GetResumeByIdAsyncDetails(id);
        if(result == null)
            return NotFound();
        return View(result);
    }
}