using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class ResumeService : IResumeService
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;
    private IResumeService _resumeServiceImplementation;

    public ResumeService(IHttpContextAccessor httpContextAccessor,MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<List<SelectListItem>> LoadCategories(CreateResumeViewModel model)
    {
        model.Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.NameCategory)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            }).ToListAsync();
        return model.Categories;
    }
    public async Task<(bool RequiresLogin, bool Updated)> RefreshAsync(int id)
    {
        var user = GetUserId();

        if (user == null)
            return (true, false);

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Value);
        if (resume == null)
            return (false, false);

        resume.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return (false, true);
        
    }

    public async Task<(bool RequiresLogin, bool SetPublic)> SetPublication(int id, bool isPublished)
    {
        var user = GetUserId();
        if (user == null)
            return (true, false);

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Value);
        if (resume == null)
            return (false, false);

        resume.IsPublished = isPublished;
        await _context.SaveChangesAsync();
        return (false, true);
    }


    public async Task<CreateResumeViewModel> GetCreateResumeView()
    {
        var model = new CreateResumeViewModel();
        await LoadCategories(model);
        return model;
    }
    
    public async  Task<Dictionary<string, string>> CreateResume(CreateResumeViewModel model)
    {
        var errors = new Dictionary<string, string>();
        if (model.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == model.CategoryId.Value);
            if (!categoryExists)
            {
                errors[nameof(model.CategoryId)] =
                    "Выбранная категория не существует.";
            }
        }
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            errors[nameof(model.Title)] =
                "Выбранная категория не существует.";
        }

        var user = GetUserId();

        if (user == null)
            return null;

        var now = DateTime.UtcNow;

        var resume = new Resume
        {
            Title = model.Title.Trim(),
            ExpectedSalary = model.ExpectedSalary!.Value,
            CategoryId = model.CategoryId!.Value,
            ApplicantId = user.Value,

            Telegram = model.Telegram?.Trim(),
            Email = model.Email?.Trim(),
            PhoneNumber = model.PhoneNumber?.Trim(),
            FacebookUrl = model.FacebookUrl?.Trim(),
            LinkedInUrl = model.LinkedInUrl?.Trim(),

            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Resumes.AddAsync(resume);
        await _context.SaveChangesAsync();
        
        return errors;
    }

    public async Task<Resume> GetResumeDetails(int id)
    {
        var user = GetUserId();

        if (user == null)
            return null;

        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.WorkExperiences)
            .Include(r => r.Educations)
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Value);

        if (resume == null)
            return null;

        return resume;
    }

    public async Task<(CreateResumeViewModel, int? resumeId)> GetEditResumeView(int id)
    {
        var user = GetUserId();

        if (user == null)
            return (null,null);

        var resume = await _context.Resumes
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Value);

        if (resume == null)
            return (null,null);

        var model = new CreateResumeViewModel
        {
            Title = resume.Title,
            ExpectedSalary = resume.ExpectedSalary,
            CategoryId = resume.CategoryId,

            Telegram = resume.Telegram,
            Email = resume.Email,
            PhoneNumber = resume.PhoneNumber,
            FacebookUrl = resume.FacebookUrl,
            LinkedInUrl = resume.LinkedInUrl
        };

        await LoadCategories(model);
        return (model,resume.Id);

    }

    public async Task<(
        bool RequiresLogin,
        int? ResumeId,
        Dictionary<string, string> Errors)> EditAsync(
        int id, CreateResumeViewModel model)
    {
        var errors = new Dictionary<string, string>();

        var userId = GetUserId();

        if (userId == null)
            return (true, null, errors);

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == userId.Value);

        if (resume == null)
            return (false, null, errors);

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            errors[nameof(model.Title)] = "Введите должность.";
        }

        if (!model.CategoryId.HasValue)
        {
            errors[nameof(model.CategoryId)] = "Выберите категорию.";
        }
        else
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == model.CategoryId.Value);

            if (!categoryExists)
            {
                errors[nameof(model.CategoryId)] =
                    "Выбранная категория не существует.";
            }
        }

        if (!model.ExpectedSalary.HasValue)
        {
            errors[nameof(model.ExpectedSalary)] =
                "Укажите ожидаемую зарплату.";
        }

        if (errors.Count > 0)
            return (false, resume.Id, errors);

        resume.Title = model.Title.Trim();
        resume.ExpectedSalary = model.ExpectedSalary.Value;
        resume.CategoryId = model.CategoryId.Value;
        resume.UpdatedAt = DateTime.UtcNow;
        resume.Telegram = model.Telegram?.Trim();
        resume.Email = model.Email?.Trim();
        resume.PhoneNumber = model.PhoneNumber?.Trim();
        resume.FacebookUrl = model.FacebookUrl?.Trim();
        resume.LinkedInUrl = model.LinkedInUrl?.Trim();

        await _context.SaveChangesAsync();

        return (false, resume.Id, errors);
    }
    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}