using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class UserService : IUserService
{
    private readonly MyUserContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<MyUser> _userManager;
    private IResumeService _resumeServiceImplementation;
    private IUserService _userServiceImplementation;

    public UserService(IHttpContextAccessor httpContextAccessor,MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<(ProfileViewModel,bool Login)> GetProfileAsync()
    {
        var userId = GetUserId();
        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            return (null, true);
        }
        var model = await BuildProfileModel();
        return (model,true);
    }

    public async Task<(bool Edited, bool RequiresLogin, Dictionary<string, string> Errors)> EditProfileAsync(string? userName, string? email, string? phoneNumber)
    {
        var errors = new Dictionary<string, string>();
        var userId = GetUserId();
        
        if (userId == null)
            return (false,true,errors);
        
        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        
        if(user == null)
            return (false,true,errors);

        userName = userName?.Trim();
        email = email?.Trim();
        phoneNumber = phoneNumber?.Trim();
        
        
        if (string.IsNullOrWhiteSpace(userName))
        {
            errors[nameof(userName)] = "Введите логин.";
        }

        if (string.IsNullOrWhiteSpace(email) ||
            !new EmailAddressAttribute().IsValid(email))
        {
            errors[nameof(email)] = "Введите корректный email.";
        }

        if (string.IsNullOrWhiteSpace(phoneNumber) ||
            !new PhoneAttribute().IsValid(phoneNumber))
        {
            errors[nameof(phoneNumber)] = "Введите корректный телефон.";
        }

        if (errors.Count > 0)
            return (false, false, errors);
        
        user.UserName = userName!;

        if (user.Email != email)
        {
            user.Email = email!;
            user.EmailConfirmed = false;
        }

        if (user.PhoneNumber != phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            user.PhoneNumberConfirmed = false;
        }
        var result = await _userManager.UpdateAsync(user);
        foreach (var error in result.Errors)
        {
            errors[error.Code] = error.Description;
        }
        return (result.Succeeded, false, errors);
    }

    public async Task<ProfileViewModel> BuildProfileModel()
    {
        var userId = GetUserId();
        if (userId == null)
            return null;
        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
            return null;
        var model = new ProfileViewModel
        {
            User = user
        };
        if (await _userManager.IsInRoleAsync(user, "User"))
        {
            model.Resumes = await _context.Resumes
                .AsNoTracking()
                .Where(r => r.ApplicantId == user.Id)
                .OrderByDescending(r => r.UpdatedAt)
                .ToListAsync();
        }

        if (await _userManager.IsInRoleAsync(user, "Employer"))
        {
            model.Vacancies = await _context.Vacancies
                .AsNoTracking()
                .Where(v => v.EmployerId == user.Id)
                .OrderByDescending(v => v.UpdatedAt)
                .ToListAsync();
        }
        return model;
    }


    public int? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
    
    
}