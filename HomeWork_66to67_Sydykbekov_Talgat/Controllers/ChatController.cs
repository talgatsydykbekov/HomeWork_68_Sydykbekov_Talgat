using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class ChatController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public ChatController(
        MyUserContext context,
        UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var model = await BuildChatModel(id, user.Id);

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

        // Проверяем доступ и при отправке сообщения.
        var chat = await BuildChatModel(id, user.Id);

        if (chat == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            chat.Text = model.Text ?? string.Empty;

            return View("Index", chat);
        }

        var message = new ChatMessage
        {
            JobApplicationId = chat.ApplicationId,
            SenderId = user.Id,
            Text = model.Text.Trim(),
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);

        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(Index),
            new { id = chat.ApplicationId });
    }

    private async Task<ChatViewModel?> BuildChatModel(
        int applicationId,
        int userId)
    {
        var application = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Vacancy)
            .Include(a => a.Resume)
            .FirstOrDefaultAsync(a =>
                a.Id == applicationId &&
                (a.Resume.ApplicantId == userId ||
                 a.Vacancy.EmployerId == userId));

        if (application == null)
            return null;

        var messages = await _context.ChatMessages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Where(m => m.JobApplicationId == application.Id)
            .OrderBy(m => m.SentAt)
            .ThenBy(m => m.Id)
            .ToListAsync();

        return new ChatViewModel
        {
            ApplicationId = application.Id,
            CurrentUserId = userId,
            VacancyTitle = application.Vacancy.Title,
            ResumeTitle = application.Resume.Title,
            Messages = messages
        };
    }
}