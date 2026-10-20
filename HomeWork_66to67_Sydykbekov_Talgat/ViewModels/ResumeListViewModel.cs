using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class ResumeListViewModel
{
    public List<Resume> Resumes { get; set; } = new();
    public int? CategoryId { get; set; }
    public List<SelectListItem> Categories { get; set; } = new();
    public int Page { get; set; }
    public int TotalPages { get; set; }

}