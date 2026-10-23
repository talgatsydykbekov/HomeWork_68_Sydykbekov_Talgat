using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class ResumeCatalogService : IResumeCatalogService
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;
    private IResumeCatalogService _resumeCatalogServiceImplementation;

    public ResumeCatalogService(IHttpContextAccessor httpContextAccessor,MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    

    public async Task<ResumeListViewModel> GetAllResumeListAsync(int? categoryId, int page = 1)
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

        var model= new ResumeListViewModel
        {
            Resumes = resumes,
            CategoryId = categoryId,
            Categories = categories,
            Page = page,
            TotalPages = totalPages
        };
        return model;
    }

    public async Task<ResumeDetailsViewModel> GetResumeByIdAsyncDetails(int? id)
    {
        var user = GetUserId();
        if (user == null)
            return null;
        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.WorkExperiences)
            .Include(r => r.Educations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id && r.IsPublished);

        if (resume == null)
            return null;
        
        var vacancies = await _context.Vacancies
            .AsNoTracking()
            .Where(v =>
                v.EmployerId == user.Value &&
                v.IsPublished)
            .OrderBy(v => v.Title)
            .Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Title
            })
            .ToListAsync();
        return new ResumeDetailsViewModel
        {
            resume = resume,
            Vacancies = vacancies,
        };
    }
    
    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}