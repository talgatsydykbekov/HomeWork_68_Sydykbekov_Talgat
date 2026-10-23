using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class VacancyService : IVacancyService
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;

    public VacancyService(
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
    public async Task LoadCategoriesAsync(CreateVacancyViewModel model)
    {
        model.Categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.NameCategory)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            })
            .ToListAsync();
    }

    public async Task<CreateVacancyViewModel> GetCreateModelAsync()
    {
        var model = new CreateVacancyViewModel();
        await LoadCategoriesAsync(model);
        return model;
    }

    private async Task<Dictionary<string, string>> ValidateAsync(
        CreateVacancyViewModel model)
    {
        var errors = new Dictionary<string, string>();

        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(
            model, new ValidationContext(model), validationResults, true);

        foreach (var result in validationResults)
        {
            var members = result.MemberNames.ToArray();
            if (members.Length == 0)
                errors[string.Empty] = result.ErrorMessage ?? "Проверьте данные.";

            foreach (var member in members)
                errors[member] = result.ErrorMessage ?? "Проверьте данные.";
        }

        if (string.IsNullOrWhiteSpace(model.Title))
            errors[nameof(model.Title)] = "Введите название вакансии.";

        if (string.IsNullOrWhiteSpace(model.Description))
            errors[nameof(model.Description)] = "Введите описание вакансии.";

        if (!model.Salary.HasValue)
            errors[nameof(model.Salary)] = "Укажите зарплату.";

        if (!model.ExperienceFrom.HasValue)
            errors[nameof(model.ExperienceFrom)] = "Укажите минимальный опыт.";

        if (!model.ExperienceTo.HasValue)
            errors[nameof(model.ExperienceTo)] = "Укажите максимальный опыт.";

        if (model.ExperienceFrom.HasValue && model.ExperienceTo.HasValue &&
            model.ExperienceFrom.Value > model.ExperienceTo.Value)
        {
            errors[nameof(model.ExperienceTo)] =
                "Максимальный опыт не может быть меньше минимального.";
        }

        if (!model.CategoryId.HasValue)
        {
            errors[nameof(model.CategoryId)] = "Выберите категорию.";
        }
        else if (!await _context.Categories.AnyAsync(c => c.Id == model.CategoryId.Value))
        {
            errors[nameof(model.CategoryId)] = "Выбранная категория не существует.";
        }

        return errors;
    }

    public async Task<(bool RequiresLogin, int? VacancyId, Dictionary<string, string> Errors)>
        CreateAsync(CreateVacancyViewModel model)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return (true, null, new Dictionary<string, string>());

        var errors = await ValidateAsync(model);
        if (errors.Count > 0)
            return (false, null, errors);

        var now = DateTime.UtcNow;
        var vacancy = new Vacancy
        {
            Title = model.Title.Trim(),
            Salary = model.Salary!.Value,
            Description = model.Description.Trim(),
            ExperienceFrom = model.ExperienceFrom!.Value,
            ExperienceTo = model.ExperienceTo!.Value,
            CategoryId = model.CategoryId!.Value,
            EmployerId = user.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsPublished = false
        };

        _context.Vacancies.Add(vacancy);
        await _context.SaveChangesAsync();
        return (false, vacancy.Id, errors);
    }

    public async Task<(bool RequiresLogin, Vacancy? Vacancy)> GetDetailsAsync(int id)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return (true, null);

        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .Include(v => v.Category)
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == user.Id);

        return (false, vacancy);
    }

    public async Task<(bool RequiresLogin, CreateVacancyViewModel? Model)>
        GetEditModelAsync(int id)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return (true, null);

        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == user.Id);

        if (vacancy == null)
            return (false, null);

        var model = new CreateVacancyViewModel
        {
            Title = vacancy.Title,
            Salary = vacancy.Salary,
            Description = vacancy.Description,
            ExperienceFrom = vacancy.ExperienceFrom,
            ExperienceTo = vacancy.ExperienceTo,
            CategoryId = vacancy.CategoryId
        };

        await LoadCategoriesAsync(model);
        return (false, model);
    }

    public async Task<(bool RequiresLogin, int? VacancyId, Dictionary<string, string> Errors)>
        EditAsync(int id, CreateVacancyViewModel model)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return (true, null, new Dictionary<string, string>());

        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == user.Id);

        if (vacancy == null)
            return (false, null, new Dictionary<string, string>());

        var errors = await ValidateAsync(model);
        if (errors.Count > 0)
            return (false, vacancy.Id, errors);

        vacancy.Title = model.Title.Trim();
        vacancy.Salary = model.Salary!.Value;
        vacancy.Description = model.Description.Trim();
        vacancy.ExperienceFrom = model.ExperienceFrom!.Value;
        vacancy.ExperienceTo = model.ExperienceTo!.Value;
        vacancy.CategoryId = model.CategoryId!.Value;
        vacancy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (false, vacancy.Id, errors);
    }

    public async Task<(bool RequiresLogin, bool Updated)> RefreshAsync(int id)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return (true, false);

        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == user.Id);

        if (vacancy == null)
            return (false, false);

        vacancy.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (false, true);
    }

    public async Task<(bool RequiresLogin, bool Updated)>
        SetPublicationAsync(int id, bool isPublished)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return (true, false);

        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == user.Id);

        if (vacancy == null)
            return (false, false);

        if (isPublished && !vacancy.IsPublished)
            vacancy.PublishedAt = DateTime.UtcNow;

        vacancy.IsPublished = isPublished;
        await _context.SaveChangesAsync();
        return (false, true);
    }
}
