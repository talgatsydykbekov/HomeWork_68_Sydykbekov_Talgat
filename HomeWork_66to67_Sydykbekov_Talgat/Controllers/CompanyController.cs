using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
public class CompanyController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private ICompanyService _companyService;

    public CompanyController(UserManager<MyUser> userManager,ICompanyService companyService)
    {
        _userManager = userManager;
        _companyService = companyService;
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
        return View(await _companyService.Details(company));
    }
}