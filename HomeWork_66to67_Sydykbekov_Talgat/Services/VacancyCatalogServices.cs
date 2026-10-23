using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class VacancyCatalogServices : IVacancyCatalogServices
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;
    private IResumeService _resumeServiceImplementation;

    public VacancyCatalogServices(IHttpContextAccessor httpContextAccessor,MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<VacancyListViewModel> GetAllVacancies(int? categoryId, string? search, string sort = "updated", int page = 1)
    {
        const int pageSize = 20;
        search = search?.Trim();
        if(sort != "salaryAsk" && sort != "SalaryDesc")
            sort = "updated";
        var query = _context.Vacancies
            .AsNoTracking()
            .Where(v => v.IsPublished);
        if (categoryId.HasValue)
        {
            query = query.Where(v =>
                v.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(v => v.Title.Contains(search));
        }
        var totalCount = await query.CountAsync();

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        page = Math.Clamp(page, 1, totalPages);

        var sortedQuary = sort switch
        {
            "salaryAsk" => query.OrderBy(v => v.Salary)
                .ThenByDescending(v => v.Id),

            "salaryDesc" => query.OrderByDescending(v => v.Salary)
                .ThenByDescending(v => v.Id),
            _ => query.OrderByDescending(v => v.UpdatedAt)
                .ThenByDescending(v => v.Id)
        };

        var vacancies = await query
            .Include(v => v.Employer)
            .Include(v => v.Category)
            .OrderByDescending(v => v.UpdatedAt)
            .ThenByDescending(v => v.Id)
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
            }).ToListAsync();
        var model = new VacancyListViewModel
        {
            Vacancies = vacancies,
            Page = page,
            TotalPages = totalPages,
            Categories = categories,
            CategoryId = categoryId,
            Search = search
        };
        return model;
    }

    public async Task<GetDetailsVacancyViewModel> GetDetailsVacancyDetails(int id)
    {
        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .Include(v => v.Category)
            .Include(v => v.Employer)
            .FirstOrDefaultAsync(v =>
                v.Id == id &&
                v.IsPublished);

        if (vacancy == null)
            return null;

        var resumes = new List<SelectListItem>();

        if (_httpContextAccessor.HttpContext?
                .User.IsInRole("User") ?? false)
        {
            var user = GetUserId();
            if (user == null) return null;
            var User = _userManager.FindByIdAsync(user.Value.ToString()).Result;
            if(User == null) return null;
            
            resumes = await _context.Resumes
                .AsNoTracking()
                .Where(r => r.ApplicantId == User.Id)
                .OrderBy(r => r.Title)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Title
                }).ToListAsync();
        }
        var result = new GetDetailsVacancyViewModel
        {
            Resumes = resumes,
            Vacancy = vacancy,
        };
        return result;
    }
    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}