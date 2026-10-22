using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryServices;

    public CategoryController(ICategoryService categoryservices)
    {
        _categoryServices = categoryservices;
    }
    [Authorize(Roles = "Admin")]
    public async  Task<IActionResult> Index()
    {
        var categories = await  _categoryServices.GetAllCategoryAsync();
        if (categories == null)
        {
            return NotFound();
        }
        return View(categories);
    }
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }
        await _categoryServices.CreateAsync(category);
        return RedirectToAction(nameof(Index));
    }
}