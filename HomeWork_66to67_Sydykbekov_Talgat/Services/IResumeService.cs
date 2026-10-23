using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IResumeService
{
    public Task<CreateResumeViewModel> GetCreateResumeView();
    public Task<Dictionary<string, string>> CreateResume(CreateResumeViewModel model);
    public Task<Resume> GetResumeDetails(int id);
    public Task<(CreateResumeViewModel, int? resumeId)> GetEditResumeView(int id);

    public Task<(
        bool RequiresLogin,
        int? ResumeId,
        Dictionary<string, string> Errors)> EditAsync(
        int id, CreateResumeViewModel model);
    public Task<List<SelectListItem>> LoadCategories(CreateResumeViewModel model);
    public Task<(bool RequiresLogin, bool Updated)> RefreshAsync(int id);
    public Task<(bool RequiresLogin, bool SetPublic)> SetPublication(int id, bool isPublished);

}