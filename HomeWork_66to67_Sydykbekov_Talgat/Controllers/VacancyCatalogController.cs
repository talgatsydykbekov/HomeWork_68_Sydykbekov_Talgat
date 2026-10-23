using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class VacancyCatalogController : Controller
{
    private readonly IVacancyCatalogServices _vacancyCatalogServices;

    public VacancyCatalogController(IVacancyCatalogServices vacancyCatalogServices)
    {
        _vacancyCatalogServices = vacancyCatalogServices;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? categoryId,string? search,string sort = "updated",int page = 1)
    {
        var model = await _vacancyCatalogServices.GetAllVacancies(categoryId, search, sort);
        if (model == null)
            return NotFound();
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await  _vacancyCatalogServices.GetDetailsVacancyDetails(id);
        if (result == null) return NotFound();
        return View(result);
    }
}