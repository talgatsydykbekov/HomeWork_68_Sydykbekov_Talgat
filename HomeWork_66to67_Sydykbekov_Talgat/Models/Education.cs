using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class Education
{
    public int Id { get; set; }
    public int ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
    [Required(ErrorMessage = "Введите название учебного заведения")]
    [StringLength(200)]
    public string InstitutionName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Введите специальность или название курса")]
    [StringLength(200)]
    public string Program { get; set; } = string.Empty;
    [Range(1900, 9999, ErrorMessage = "Введите корректный год")]
    public int StartYear { get; set; }
    [Range(1900, 9999, ErrorMessage = "Введите корректный год")]
    public int? EndYear { get; set; }
    public bool IsCourse { get; set; }
}