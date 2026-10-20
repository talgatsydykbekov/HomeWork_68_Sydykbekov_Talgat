using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class VacancyListViewModel
{
    public List<Vacancy> Vacancies { get; set; } = new();

    public int Page { get; set; }

    public int TotalPages { get; set; }
    
    public int? CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
    public string? Search { get; set; }
    public string Sort { get; set; } = "updated";
}