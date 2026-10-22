using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IEmployerApplicationService
{
    public Task<VacancyApplicationsViewModel> GetAllApplications(int vacancyId);
}