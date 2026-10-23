using HomeWork_66_Sydykbekov_Talgat.ViewModels;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IWorkExperienceService
{
    Task<(
            bool RequiresLogin,
            CreateWorkExperienceViewModel? Model)>
        GetCreateModelAsync(int resumeId);

    Task<(
            bool RequiresLogin,
            int? ResumeId,
            Dictionary<string, string> Errors)>
        CreateAsync(CreateWorkExperienceViewModel model);

    Task<(
            bool RequiresLogin,
            CreateWorkExperienceViewModel? Model)>
        GetEditModelAsync(int id);

    Task<(
            bool RequiresLogin,
            int? ResumeId,
            Dictionary<string, string> Errors)>
        EditAsync(int id, CreateWorkExperienceViewModel model);

    Task<(
            bool RequiresLogin,
            int? ResumeId)>
        DeleteAsync(int id);
}