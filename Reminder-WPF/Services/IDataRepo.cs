using Reminder_WPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminder_WPF.Services
{
    public interface IDataRepo
    {
        
        Task<Reminder> AddReminderAsync(Reminder item);

        
        Task<bool> DeleteReminderAsync(Reminder item);

        
        Task<List<Reminder>> GetRemindersAsync();

        Task<Reminder?> GetReminderByIdAsync(int id);

    }
}