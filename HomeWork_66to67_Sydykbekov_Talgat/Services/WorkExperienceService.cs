using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class WorkExperienceService : IWorkExperienceService
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;

    public WorkExperienceService(
        IHttpContextAccessor httpContextAccessor,
        MyUserContext context,
        UserManager<MyUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _userManager = userManager;
    }
    
    private async Task<MyUser?> GetCurrentUserAsync()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(value, out var userId))
            return null;

        return await _userManager.FindByIdAsync(userId.ToString());
    }

    private Dictionary<string, string> Validate(
        CreateWorkExperienceViewModel model)
    {
        var errors = new Dictionary<string, string>();
        
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            validationResults,
            validateAllProperties: true);

        foreach (var result in validationResults)
        {
            var fields = result.MemberNames.ToArray();

            if (fields.Length == 0)
            {
                errors[string.Empty] =
                    result.ErrorMessage ?? "Проверьте данные.";
            }

            foreach (var field in fields)
            {
                errors[field] =
                    result.ErrorMessage ?? "Проверьте данные.";
            }
        }

        if (string.IsNullOrWhiteSpace(model.CompanyName))
        {
            errors[nameof(model.CompanyName)] =
                "Введите название компании.";
        }

        if (string.IsNullOrWhiteSpace(model.Position))
        {
            errors[nameof(model.Position)] =
                "Введите должность.";
        }

        if (string.IsNullOrWhiteSpace(model.Responsibilities))
        {
            errors[nameof(model.Responsibilities)] =
                "Введите обязанности.";
        }

        var currentYear = DateTime.UtcNow.Year;

        if (!model.StartYear.HasValue)
        {
            errors[nameof(model.StartYear)] =
                "Укажите год начала.";
        }
        else if (model.StartYear.Value > currentYear)
        {
            errors[nameof(model.StartYear)] =
                "Год начала не может быть в будущем.";
        }

        if (model.EndYear.HasValue &&
            model.EndYear.Value > currentYear)
        {
            errors[nameof(model.EndYear)] =
                "Год окончания не может быть в будущем.";
        }

        if (model.StartYear.HasValue &&
            model.EndYear.HasValue &&
            model.EndYear.Value < model.StartYear.Value)
        {
            errors[nameof(model.EndYear)] =
                "Год окончания не может быть раньше года начала.";
        }

        return errors;
    }

    public async Task<(
        bool RequiresLogin,
        CreateWorkExperienceViewModel? Model)>
        GetCreateModelAsync(int resumeId)
    {
        var user = await GetCurrentUserAsync();

        if (user == null)
            return (true, null);

        var ownsResume = await _context.Resumes
            .AnyAsync(r =>
                r.Id == resumeId &&
                r.ApplicantId == user.Id);

        if (!ownsResume)
            return (false, null);

        return (false, new CreateWorkExperienceViewModel
        {
            ResumeId = resumeId
        });
    }

    public async Task<(
        bool RequiresLogin,
        int? ResumeId,
        Dictionary<string, string> Errors)>
        CreateAsync(CreateWorkExperienceViewModel model)
    {
        var errors = new Dictionary<string, string>();
        var user = await GetCurrentUserAsync();

        if (user == null)
            return (true, null, errors);

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == model.ResumeId &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return (false, null, errors);

        errors = Validate(model);

        if (errors.Count > 0)
            return (false, resume.Id, errors);

        var experience = new WorkExperience
        {
            ResumeId = resume.Id,
            CompanyName = model.CompanyName.Trim(),
            Position = model.Position.Trim(),
            Responsibilities = model.Responsibilities.Trim(),
            StartYear = model.StartYear!.Value,
            EndYear = model.EndYear
        };

        _context.WorkExperiences.Add(experience);

        resume.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (false, resume.Id, errors);
    }

    public async Task<(
        bool RequiresLogin,
        CreateWorkExperienceViewModel? Model)>
        GetEditModelAsync(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user == null)
            return (true, null);

        var experience = await _context.WorkExperiences
            .AsNoTracking()
            .FirstOrDefaultAsync(w =>
                w.Id == id &&
                w.Resume.ApplicantId == user.Id);

        if (experience == null)
            return (false, null);

        var model = new CreateWorkExperienceViewModel
        {
            ResumeId = experience.ResumeId,
            CompanyName = experience.CompanyName,
            Position = experience.Position,
            Responsibilities = experience.Responsibilities,
            StartYear = experience.StartYear,
            EndYear = experience.EndYear
        };

        return (false, model);
    }

    public async Task<(
        bool RequiresLogin,
        int? ResumeId,
        Dictionary<string, string> Errors)>
        EditAsync(int id, CreateWorkExperienceViewModel model)
    {
        var errors = new Dictionary<string, string>();
        var user = await GetCurrentUserAsync();

        if (user == null)
            return (true, null, errors);

        var experience = await _context.WorkExperiences
            .Include(w => w.Resume)
            .FirstOrDefaultAsync(w =>
                w.Id == id &&
                w.Resume.ApplicantId == user.Id);

        if (experience == null)
            return (false, null, errors);

        model.ResumeId = experience.ResumeId;

        errors = Validate(model);

        if (errors.Count > 0)
            return (false, experience.ResumeId, errors);

        experience.CompanyName = model.CompanyName.Trim();
        experience.Position = model.Position.Trim();
        experience.Responsibilities = model.Responsibilities.Trim();
        experience.StartYear = model.StartYear!.Value;
        experience.EndYear = model.EndYear;

        experience.Resume.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (false, experience.ResumeId, errors);
    }

    public async Task<(
        bool RequiresLogin,
        int? ResumeId)>
        DeleteAsync(int id)
    {
        var user = await GetCurrentUserAsync();

        if (user == null)
            return (true, null);

        var experience = await _context.WorkExperiences
            .Include(w => w.Resume)
            .FirstOrDefaultAsync(w =>
                w.Id == id &&
                w.Resume.ApplicantId == user.Id);

        if (experience == null)
            return (false, null);

        var resumeId = experience.ResumeId;

        experience.Resume.UpdatedAt = DateTime.UtcNow;

        _context.WorkExperiences.Remove(experience);

        await _context.SaveChangesAsync();

        return (false, resumeId);
    }
}