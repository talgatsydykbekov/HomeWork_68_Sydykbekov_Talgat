using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class RegisterViewModel
{
    [Required] public string NicknameOrNameCompany { get; set; }
    [Required] public string Email { get; set; }
    [Required][Range(2,3)]public int RoleId { get; set; }
    
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword")]
    public string PasswordConfirm { get; set; }

    [Phone] [Display(Name = "NumberTel")] public string? PhoneNumber { get; set; }
}