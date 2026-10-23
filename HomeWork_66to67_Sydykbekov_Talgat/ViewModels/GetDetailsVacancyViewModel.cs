using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class GetDetailsVacancyViewModel
{
    public Vacancy Vacancy { get; set; }
    public List<SelectListItem> Resumes { get; set; }
}