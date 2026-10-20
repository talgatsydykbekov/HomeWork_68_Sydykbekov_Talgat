using HomeWork_66_Sydykbekov_Talgat.Models;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class VacancyApplicationsViewModel
{
    public string VacancyTitle { get; set; } = string.Empty;
    public List<JobApplication> Applications { get; set; } = new();
}