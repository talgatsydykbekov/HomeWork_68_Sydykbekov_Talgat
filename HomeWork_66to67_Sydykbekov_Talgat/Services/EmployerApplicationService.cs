using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class EmployerApplicationService : IEmployerApplicationService
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;

    public EmployerApplicationService(IHttpContextAccessor httpContextAccessor,MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<VacancyApplicationsViewModel?> GetAllApplications(int vacancyId)
    {
        var user = GetUserId();

        if (user == null)
            return null;
        var User = await  _userManager.FindByIdAsync(user.Value.ToString());

        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .FirstOrDefaultAsync(v =>
                v.Id == vacancyId &&
                v.EmployerId == User.Id);

        if (vacancy == null)
            return null;

        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Resume)
            .ThenInclude(r => r.Applicant)
            .Where(a => a.VacancyId == vacancy.Id)
            .OrderByDescending(a => a.CratedAt)
            .ThenByDescending(a => a.Id)
            .ToListAsync();

        return  new VacancyApplicationsViewModel
        {
            VacancyTitle = vacancy.Title,
            Applications = applications
        };
    }

    public async Task<ResumeDetailsViewModel?> GetResumeById(int applicatiobId)
    {
        var user = GetUserId();

        if (user == null)
            return null;

        var application = await _context.JobApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.Id == applicatiobId &&
                a.Vacancy.EmployerId == user);

        if (application == null)
            return null;

        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.WorkExperiences)
            .Include(r => r.Educations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r =>
                r.Id == application.ResumeId);

        if (resume == null)
            return null;
        return new ResumeDetailsViewModel
        {
            resume = resume,
            Vacancies = null
        };
    }

    public async  Task<(bool RequiresLogin, int? ApplicationId)> StartAsync(int resumeId, int vacancyId)
    {
        var user = GetUserId();

        if (user == null)
            return (true, null);
        var resumeExists = await _context.Resumes
            .AnyAsync(r =>
                r.Id == resumeId &&
                r.IsPublished);
        if (!resumeExists)
            return (false, null);
        var ownsVacancy = await _context.Vacancies
            .AnyAsync(v =>
                v.Id == vacancyId &&
                v.EmployerId == user.Value &&
                v.IsPublished);
        if (!ownsVacancy)
            return (false, null);

        var existingId = await _context.JobApplications
            .Where(a =>
                a.ResumeId == resumeId &&
                a.VacancyId == vacancyId)
            .Select(a => (int?)a.Id)
            .FirstOrDefaultAsync();

        if (existingId.HasValue)
        {
            return (false, existingId.Value);
        }

        var application = new JobApplication
        {
            ResumeId = resumeId,
            VacancyId = vacancyId,
            InitiatedByUserId = user.Value,
            CratedAt = DateTime.UtcNow
        };
        _context.JobApplications.Add(application);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var duplicateId = await _context.JobApplications
                .AsNoTracking()
                .Where(a =>
                    a.ResumeId == resumeId &&
                    a.VacancyId == vacancyId)
                .Select(a => (int?)a.Id)
                .FirstOrDefaultAsync();
            if (!duplicateId.HasValue)
                throw;
            return (false,duplicateId);
        }
        return (false, application.Id);
    }

    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}