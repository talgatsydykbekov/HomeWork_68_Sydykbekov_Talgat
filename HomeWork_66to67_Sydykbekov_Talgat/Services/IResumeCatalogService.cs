using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IResumeCatalogService
{
    public Task<ResumeListViewModel> GetAllResumeListAsync(int? categoryId, int page = 1);
    public Task<ResumeDetailsViewModel> GetResumeByIdAsyncDetails(int? id);
}