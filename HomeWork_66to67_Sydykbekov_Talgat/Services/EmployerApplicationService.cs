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

    public async Task<VacancyApplicationsViewModel> GetAllApplications(int vacancyId)
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
    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}