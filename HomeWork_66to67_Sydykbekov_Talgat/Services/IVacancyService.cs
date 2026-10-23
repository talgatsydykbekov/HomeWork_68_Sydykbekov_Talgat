using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IVacancyService
{
    Task<CreateVacancyViewModel> GetCreateModelAsync();
    Task LoadCategoriesAsync(CreateVacancyViewModel model);

    Task<(bool RequiresLogin, int? VacancyId, Dictionary<string, string> Errors)>
        CreateAsync(CreateVacancyViewModel model);

    Task<(bool RequiresLogin, Vacancy? Vacancy)> GetDetailsAsync(int id);

    Task<(bool RequiresLogin, CreateVacancyViewModel? Model)> GetEditModelAsync(int id);

    Task<(bool RequiresLogin, int? VacancyId, Dictionary<string, string> Errors)>
        EditAsync(int id, CreateVacancyViewModel model);

    Task<(bool RequiresLogin, bool Updated)> RefreshAsync(int id);
    Task<(bool RequiresLogin, bool Updated)> SetPublicationAsync(int id, bool isPublished);
}