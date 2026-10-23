using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class ResumeDetailsViewModel
{
    public List<SelectListItem> Vacancies { get; set; } = new();
    public Resume resume { get; set; }
}