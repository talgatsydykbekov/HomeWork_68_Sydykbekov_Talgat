using HomeWork_66_Sydykbekov_Talgat.Models;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class ChatViewModel
{
    public int ApplicationId { get; set; }

    public int CurrentUserId { get; set; }

    public string VacancyTitle { get; set; } = string.Empty;

    public string ResumeTitle { get; set; } = string.Empty;

    public List<ChatMessage> Messages { get; set; } = new();

    public string Text { get; set; } = string.Empty;
}