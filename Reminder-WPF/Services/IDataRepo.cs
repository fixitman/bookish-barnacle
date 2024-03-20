using Reminder_WPF.Models;
using Reminder_WPF.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminder_WPF.Services
{
    public interface IDataRepo
    {
        
        Task<Result<Reminder?>> AddReminderAsync(Reminder item);

        
        Task<Result> DeleteReminderAsync(Reminder item);

        
        Task<Result<List<Reminder>>> GetRemindersAsync();

        Task<Result<Reminder?>> GetReminderByIdAsync(int id);

    }
}