using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class SendMessageViewModel
{
    [Required(ErrorMessage = "Введите сообщение")]
    [StringLength(5000, ErrorMessage = "Максимум 5000 символов")]
    public string Text { get; set; } = string.Empty;
}