using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class EducationService : IEducationService
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public EducationService(MyUserContext context, 
        IHttpContextAccessor httpContextAccessor,
        UserManager<MyUser> userManager)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }
    public async Task<CreateEducationViewModel> CreateView(int resumeId)
    {
        var user = GetUserId();
        if (user == null)
            return null;

        var ownsResume = await _context.Resumes
            .AnyAsync(r =>
                r.Id == resumeId &&
                r.ApplicantId == user);
        if (!ownsResume)
            return null;
        return  new CreateEducationViewModel
        {
            ResumeId = resumeId
        };
    }
    public async Task<bool> Create(CreateEducationViewModel model)
    {
        var user = GetUserId();
        if (user == null)
            return false;
        var User = await  _userManager.FindByIdAsync(user.Value.ToString());
        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == model.ResumeId &&
                r.ApplicantId == User.Id);

        if (resume == null)
            return false;
        Validate(model);
        var education = new Education
        {
            ResumeId = resume.Id,
            InstitutionName = model.InstitutionName.Trim(),
            Program = model.Program.Trim(),
            StartYear = model.StartYear!.Value,
            EndYear = model.EndYear,
            IsCourse = model.IsCourse
        };
        await _context.Educations.AddAsync(education);
        resume.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public Dictionary<string, string> Validate(CreateEducationViewModel model)
    {
        var errors = new Dictionary<string, string>();
        var currentYear = DateTime.UtcNow.Year;

        if (model.StartYear.HasValue &&
            model.StartYear.Value > currentYear)
        {
            errors[nameof(model.StartYear)] =
                "Год начала не может быть в будущем.";
        }

        if (model.EndYear.HasValue &&
            model.StartYear.HasValue &&
            model.EndYear.Value < model.StartYear.Value)
        {
            errors[nameof(model.EndYear)] =
                "Год окончания не может быть раньше года начала.";
        }

        return errors;
    }

    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

}