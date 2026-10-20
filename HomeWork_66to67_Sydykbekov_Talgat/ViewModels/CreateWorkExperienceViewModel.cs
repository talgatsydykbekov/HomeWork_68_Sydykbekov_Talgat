using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class CreateWorkExperienceViewModel
{
    public int ResumeId { get; set; }

    [Required(ErrorMessage = "Введите название компании")]
    [StringLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите должность")]
    [StringLength(150)]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Опишите обязанности")]
    [StringLength(3000)]
    public string Responsibilities { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите год начала работы")]
    [Range(1900, 9999)]
    public int? StartYear { get; set; }

    [Range(1900, 9999)]
    public int? EndYear { get; set; }
}