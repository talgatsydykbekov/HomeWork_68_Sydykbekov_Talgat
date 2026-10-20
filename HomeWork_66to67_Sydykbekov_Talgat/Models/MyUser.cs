using Microsoft.AspNetCore.Identity;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class MyUser : IdentityUser<int>
{
    public string? AvatarUrl { get; set; }
}