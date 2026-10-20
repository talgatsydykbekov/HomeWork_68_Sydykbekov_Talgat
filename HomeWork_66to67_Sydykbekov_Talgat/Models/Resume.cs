using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class Resume
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название должности")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 1_000_000_000,
        ErrorMessage = "Укажите зарплату от 0 до 1 миллиарда")]
    public decimal ExpectedSalary { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int ApplicantId { get; set; }

    public MyUser Applicant { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; } = false;
    
    [StringLength(100)]
    public string? Telegram { get; set; }

    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [StringLength(254)]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Введите корректный телефон")]
    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Введите полную ссылку на Facebook")]
    [StringLength(500)]
    public string? FacebookUrl { get; set; }

    [Url(ErrorMessage = "Введите полную ссылку на LinkedIn")]
    [StringLength(500)]
    public string? LinkedInUrl { get; set; }
    
    public ICollection<WorkExperience> WorkExperiences { get; set; }
        = new List<WorkExperience>();
    
    public ICollection<Education> Educations { get; set; }
        = new List<Education>();
}