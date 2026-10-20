using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class CompanyController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public CompanyController(MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var company = await _userManager.FindByIdAsync(id.ToString());

        if (company == null)
        {
            return NotFound();
        }
        var isEmployer = await _userManager.IsInRoleAsync(company, "Employer");
        if (!isEmployer)
        {
            return NotFound();
        }
        var vacancies = await _context.Vacancies
            .AsNoTracking()
            .Where(v => v.EmployerId == company.Id && v.IsPublished)
            .OrderByDescending(v => v.UpdatedAt)
            .ThenByDescending(v => v.Id)
            .ToListAsync();

        var model = new CompanyProfileViewModel()
        {
            CompanyName = company.UserName ?? "Компания",
            Vacancies = vacancies
        };
        return View(model);
    }
}