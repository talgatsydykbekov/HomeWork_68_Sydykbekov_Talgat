using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class CompanyService : ICompanyService
{
    private readonly MyUserContext _context;
    public CompanyService(MyUserContext context)
    {
        _context = context;
    }
    public async Task<CompanyProfileViewModel> Details(MyUser company)
    {
        var vacancies = await _context.Vacancies
            .AsNoTracking()
            .Where(v => v.EmployerId == company.Id && v.IsPublished)
            .OrderByDescending(v => v.UpdatedAt)
            .ThenByDescending(v => v.Id)
            .ToListAsync();

        var model = new CompanyProfileViewModel()
        {
            CompanyName = company.UserName ?? "Компания",
            Vacancies = vacancies
        };
        return model;
    }

}