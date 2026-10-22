using HomeWork_66_Sydykbekov_Talgat.Models;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IJobApplicationService
{
    public Task<List<JobApplication>> GetAllJobApplications();
    public Task<(bool RequiresLogin,bool alreadyExists, int? applicationnId)> CreateJobApplication(int vacancyId, int resumeId);

}