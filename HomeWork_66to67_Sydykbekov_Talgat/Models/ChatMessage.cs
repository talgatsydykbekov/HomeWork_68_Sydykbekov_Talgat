using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;
    public int SenderId { get; set; }
    public MyUser Sender { get; set; } = null!;
    
    [Required(ErrorMessage = "Введите сообщение")]
    [StringLength(5000)]
    public string Text { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; }
}