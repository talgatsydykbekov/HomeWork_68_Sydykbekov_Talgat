using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IEducationService
{
    public Task<CreateEducationViewModel> CreateView(int resumeId);
    public Task<bool> Create(CreateEducationViewModel model);
    Dictionary<string, string> Validate(CreateEducationViewModel model);
}