using System.Data;
using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;

    public JobApplicationService(IHttpContextAccessor httpContextAccessor,MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<List<JobApplication>> GetAllJobApplications()
    {
        var user = GetUserId();
        if (user == null)
            return null;
        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Resume)
            .Include(a => a.Vacancy)
            .ThenInclude(v => v.Employer)
            .Where(a => a.Resume.ApplicantId == user.Value)
            .OrderByDescending(a => a.CratedAt)
            .ThenByDescending(a => a.Id)
            .ToListAsync();
        return applications;
    }

    public async Task<(bool RequiresLogin,bool alreadyExists,int? applicationnId)> CreateJobApplication(int vacancyId, int resumeId)
    {
        var user = GetUserId();
        if (user == null)
            return (true, false,null);
        
        var vacancyExists = await _context.Vacancies
            .AnyAsync(v =>
                v.Id == vacancyId &&
                v.IsPublished);
        if (!vacancyExists)
            return (false,false,null);
        var ownsResume = await _context.Resumes
            .AnyAsync(r =>
                r.Id == resumeId &&
                r.ApplicantId == user.Value);
        if (!ownsResume)
            return (false,false,null);
        var alreadyExists = await _context.JobApplications
            .AnyAsync(a =>
                a.VacancyId == vacancyId &&
                a.ResumeId == resumeId);
        if (alreadyExists)
        {
            return (false,true,null);
        }

        var application = new JobApplication
        {
            VacancyId = vacancyId,
            ResumeId = resumeId,
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
            var duplicateExists = await _context.JobApplications
                .AsNoTracking()
                .AnyAsync(a =>
                    a.VacancyId == vacancyId &&
                    a.ResumeId == resumeId);
            if (!duplicateExists)
                throw;
            return (false,true,null);
        }
        return (false,false,application.Id);
    }


    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}