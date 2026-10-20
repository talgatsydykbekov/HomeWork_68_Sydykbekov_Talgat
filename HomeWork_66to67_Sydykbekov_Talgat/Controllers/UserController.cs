using System.ComponentModel.DataAnnotations;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
[Authorize]
public class UserController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public UserController(
        MyUserContext context,
        UserManager<MyUser> userManager,IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var model = await BuildProfileModel(user);

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string? userName,
        string? email,
        string? phoneNumber)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        userName = userName?.Trim();
        email = email?.Trim();
        phoneNumber = phoneNumber?.Trim();

        if (string.IsNullOrWhiteSpace(userName))
        {
            ModelState.AddModelError(
                "",
                "Введите логин.");
        }

        if (string.IsNullOrWhiteSpace(email) ||
            !new EmailAddressAttribute().IsValid(email))
        {
            ModelState.AddModelError(
                "",
                "Введите корректный email.");
        }

        if (string.IsNullOrWhiteSpace(phoneNumber) ||
            !new PhoneAttribute().IsValid(phoneNumber))
        {
            ModelState.AddModelError(
                "",
                "Введите корректный телефон.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["OpenEditModal"] = true;

            return View(
                "Index",
                await BuildProfileModel(user));
        }

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

        if (result.Succeeded)
        {
            TempData["Success"] = "Профиль сохранён.";

            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(
                "",
                error.Description);
        }

        ViewData["OpenEditModal"] = true;

        return View(
            "Index",
            await BuildProfileModel(user));
    }

    
    private async Task<ProfileViewModel> BuildProfileModel(
        MyUser user)
    {
        var model = new ProfileViewModel
        {
            User = user
        };

        if (User.IsInRole("User"))
        {
            model.Resumes = await _context.Resumes
                .AsNoTracking()
                .Where(r => r.ApplicantId == user.Id)
                .OrderByDescending(r => r.UpdatedAt)
                .ToListAsync();
        }

        if (User.IsInRole("Employer"))
        {
            model.Vacancies = await _context.Vacancies
                .AsNoTracking()
                .Where(v => v.EmployerId == user.Id)
                .OrderByDescending(v => v.UpdatedAt)
                .ToListAsync();
        }

        return model;
    }
}