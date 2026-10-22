using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IEmployerApplicationService
{
    public Task<VacancyApplicationsViewModel> GetAllApplications(int vacancyId);
    public Task<Resume> GetResumeById(int applicatiobId);
    public Task<(bool RequiresLogin, int? ApplicationId)> StartAsync(int resumeId, int vacancyId);
}