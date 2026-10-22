using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface IChatService
{
    public Task<ChatViewModel?> GetChatAsync(int applicationId, int userId);
    public Task<bool> SendAsync(int applicationId, int userId, string text);
}