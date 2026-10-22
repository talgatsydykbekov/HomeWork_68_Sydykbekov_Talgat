using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly UserManager<MyUser> _signInManager;
    private readonly ICategoryService _categoryServices;

    public CategoryController(UserManager<MyUser> usermanager,ICategoryService categoryservices)
    {
        _signInManager = usermanager;
        _categoryServices = categoryservices;
    }
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var categories = _categoryServices.GetAllCategoryAsync().Result;
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
    public IActionResult Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }
        _categoryServices.CreateAsync(category);
        return RedirectToAction(nameof(Index));
    }
}