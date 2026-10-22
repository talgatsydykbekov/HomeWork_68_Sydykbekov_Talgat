using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
namespace HomeWork_66_Sydykbekov_Talgat.Services;
public class ChatService : IChatService 
{
    private readonly MyUserContext _context;
    public ChatService(MyUserContext context)
    {
        _context = context;
    }

    public async Task<ChatViewModel?> GetChatAsync(int applicationId, int userId)
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
    

    public async Task<bool> SendAsync(int applicationId, int userId, string text)
    {
        if(string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException("Сообщение не должно быть пустым",nameof(text));
        
        var hasAcces = await _context.JobApplications
            .AnyAsync(a => a.Id == applicationId && 
                           (a.Resume.ApplicantId == userId || a.Vacancy.EmployerId == userId));
        if (!hasAcces)
            return false;

        var message = new ChatMessage
        {
            JobApplicationId = applicationId,
            SenderId = userId,
            Text = text,
            SentAt = DateTime.UtcNow
        };
        await  _context.ChatMessages.AddAsync(message);
        await _context.SaveChangesAsync();

        return true;
    }
}
    