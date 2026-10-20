using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class Vacancy
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите название вакансии")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;
    [Range(0, 1_000_000_000,
        ErrorMessage = "Укажите зарплату от 0 до 1 миллиарда")]
    public decimal Salary { get; set; }
    [Required(ErrorMessage = "Добавьте описание вакансии")]
    [StringLength(10000)]
    public string Description { get; set; } = string.Empty;
    [Range(0, 100)]
    public int ExperienceFrom { get; set; }
    [Range(0, 100)]
    public int ExperienceTo { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public int EmployerId { get; set; }
    public MyUser Employer { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; } = false;
}