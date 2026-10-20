namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class JobApplication
{
    public int Id { get; set; }
    public int VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; } = null!;
    public int ResumeId { get; set; }
    public Resume? Resume { get; set; } = null!;
    public int InitiatedByUserId { get; set; }
    public MyUser? InitiatedByUser { get; set; } = null!;
    
    public DateTime CratedAt { get; set; }

}