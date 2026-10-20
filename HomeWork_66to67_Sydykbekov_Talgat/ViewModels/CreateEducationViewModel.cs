using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class CreateEducationViewModel
{
    public int ResumeId { get; set; }

    [Required(ErrorMessage = "Введите название учебного заведения")]
    [StringLength(200)]
    public string InstitutionName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите специальность или название курса")]
    [StringLength(200)]
    public string Program { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите год начала обучения")]
    [Range(1900, 9999)]
    public int? StartYear { get; set; }

    [Range(1900, 9999)]
    public int? EndYear { get; set; }

    public bool IsCourse { get; set; }
}