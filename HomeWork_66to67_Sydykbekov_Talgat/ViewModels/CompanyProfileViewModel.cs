using HomeWork_66_Sydykbekov_Talgat.Models;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class CompanyProfileViewModel
{
    public string CompanyName { get; set; } = string.Empty;

    public List<Vacancy> Vacancies { get; set; } = new();
}