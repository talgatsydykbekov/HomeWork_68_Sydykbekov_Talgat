using System.ComponentModel.DataAnnotations;
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;
[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await _userService.BuildProfileModel();
        if (model == null)
            return Challenge();
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string? userName,
        string? email,
        string? phoneNumber)
    {
        if (!ModelState.IsValid)
        {
            var invalidProfile = await _userService.BuildProfileModel();

            if (invalidProfile == null)
                return Challenge();

            ViewData["OpenEditModal"] = true;

            return View("Index", invalidProfile);
        }
        var result = await _userService.EditProfileAsync(
            userName, email, phoneNumber);
        if(result.RequiresLogin)
            return Challenge();
        if (result.Edited)
        {
            TempData["Success"] = "Профиль сохранён.";

            return RedirectToAction(nameof(Index));
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Value);
        }
        var profile = await _userService.BuildProfileModel();

        if (profile == null)
            return Challenge();
        ViewData["OpenEditModal"] = true;
        return View("Index", profile);
    }
}