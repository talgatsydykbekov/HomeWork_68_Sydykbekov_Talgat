using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.Services;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class ChatController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private readonly IChatService _chatService;

    public ChatController(
        UserManager<MyUser> userManager,IChatService chatService)
    {
        _userManager = userManager;
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var model = await _chatService.GetChatAsync(id,user.Id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(
        int id,
        SendMessageViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();
        var chat = await _chatService.GetChatAsync(id,user.Id);

        if (chat == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            chat.Text = model.Text ?? string.Empty;

            return View("Index", chat);
        }

        var sent = await _chatService.SendAsync(id,user.Id, model.Text);
        if(!sent) return NotFound();
        
        return RedirectToAction(
            nameof(Index),
            new { id = chat.ApplicationId });
    }
}