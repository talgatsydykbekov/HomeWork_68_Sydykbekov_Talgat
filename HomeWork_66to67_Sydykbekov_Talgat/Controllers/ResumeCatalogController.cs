using HomeWork_66_Sydykbekov_Talgat.Models;
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
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public ResumeCatalogController(MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? categoryId,
        int page = 1)
    {
        const int pageSize = 20;

        var query = _context.Resumes
            .AsNoTracking()
            .Where(r => r.IsPublished);

        if (categoryId.HasValue)
        {
            query = query.Where(r =>
                r.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync();

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        page = Math.Clamp(page, 1, totalPages);

        var resumes = await query
            .Include(r => r.Category)
            .OrderByDescending(r => r.UpdatedAt)
            .ThenByDescending(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.NameCategory)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            })
            .ToListAsync();

        var model = new ResumeListViewModel
        {
            Resumes = resumes,
            CategoryId = categoryId,
            Categories = categories,
            Page = page,
            TotalPages = totalPages
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }
        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.WorkExperiences)
            .Include(r => r.Educations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id && r.IsPublished);

        if (resume == null)
        {
            return NotFound();
        }
        var vacancies = await _context.Vacancies
            .AsNoTracking()
            .Where(v =>
                v.EmployerId == user.Id &&
                v.IsPublished)
            .OrderBy(v => v.Title)
            .Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Title
            })
            .ToListAsync();

        ViewData["VacancyOptions"] = vacancies;
        return View(resume);
    }
}