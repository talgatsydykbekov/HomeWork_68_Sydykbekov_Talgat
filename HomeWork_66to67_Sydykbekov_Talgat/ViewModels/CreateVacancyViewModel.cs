using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class CreateVacancyViewModel
{
    [Required(ErrorMessage = "Введите название вакансии")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите зарплату")]
    [Range(0, 1_000_000_000)]
    public decimal? Salary { get; set; }

    [Required(ErrorMessage = "Добавьте описание вакансии")]
    [StringLength(10000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите минимальный опыт")]
    [Range(0, 100)]
    public int? ExperienceFrom { get; set; }

    [Required(ErrorMessage = "Укажите максимальный опыт")]
    [Range(0, 100)]
    public int? ExperienceTo { get; set; }

    [Required(ErrorMessage = "Выберите категорию")]
    public int? CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
}