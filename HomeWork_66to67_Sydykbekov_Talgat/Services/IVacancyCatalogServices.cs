using HomeWork_66_Sydykbekov_Talgat.ViewModels;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IVacancyCatalogServices
{
    public Task<VacancyListViewModel> GetAllVacancies(int? categoryId, string? search, string sort = "updated",
        int page = 1);
    public Task<GetDetailsVacancyViewModel> GetDetailsVacancyDetails(int Id);
}