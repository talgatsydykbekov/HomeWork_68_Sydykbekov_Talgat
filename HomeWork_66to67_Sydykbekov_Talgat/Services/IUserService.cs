using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IUserService
{
    public Task<(ProfileViewModel,bool Login)> GetProfileAsync();
    public Task<(bool Edited,bool RequiresLogin,Dictionary<string,string> Errors)> EditProfileAsync(string? userName,string? email, string? phoneNumber);

    public Task<ProfileViewModel> BuildProfileModel();
}