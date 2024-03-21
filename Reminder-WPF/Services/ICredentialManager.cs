using System.Threading.Tasks;

namespace Reminder_WPF.Services
{
    public interface ICredentialManager
    {
        Task<string?> GetToken();

        void LogOut();
    }
}