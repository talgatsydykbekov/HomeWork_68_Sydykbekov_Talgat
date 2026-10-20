using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private MyUserContext _context;
    private readonly UserManager<MyUser> _signInManager;

    public CategoryController(MyUserContext context, UserManager<MyUser> usermanager)
    {
        _context = context;
        _signInManager = usermanager;
    }
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var categories = _context.Categories.ToList();
        if (categories == null)
        {
            return NotFound();
        }
        return View(categories.ToList());
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
        if (category == null)
        {
            return BadRequest();
        }
        _context.Categories.Add(category);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}