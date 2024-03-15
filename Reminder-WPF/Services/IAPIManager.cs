using System;

namespace Reminder_WPF.Services
{
    public interface IAPIManager
    {
        string BasePath { get; set; }
        string CurrentToken { get; set; }
        DateTime TokenExpiration { get; set; }

        string? GetToken();
        string? GetToken(string username, string password);
    }
}