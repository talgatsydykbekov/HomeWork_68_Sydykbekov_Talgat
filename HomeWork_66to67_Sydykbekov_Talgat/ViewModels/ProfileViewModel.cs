using HomeWork_66_Sydykbekov_Talgat.Models;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class ProfileViewModel
{
    public MyUser User { get; set; } = null!;

    public List<Resume> Resumes { get; set; } = new();
    public List<Vacancy> Vacancies { get; set; } = new();
}