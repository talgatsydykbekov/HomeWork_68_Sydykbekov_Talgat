using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class WorkExperience
{
    public int Id { get; set; }

    public int ResumeId { get; set; }

    public Resume Resume { get; set; } = null!;

    [Required(ErrorMessage = "Введите название компании")]
    [StringLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите должность")]
    [StringLength(150)]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Опишите обязанности")]
    [StringLength(3000)]
    public string Responsibilities { get; set; } = string.Empty;

    [Range(1900, 9999, ErrorMessage = "Введите корректный год")]
    public int StartYear { get; set; }

    [Range(1900, 9999, ErrorMessage = "Введите корректный год")]
    public int? EndYear { get; set; }
}