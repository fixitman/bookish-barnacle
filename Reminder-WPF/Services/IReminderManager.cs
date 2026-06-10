using Reminder_WPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminder_WPF.Services
{
    public interface IReminderManager
    {
        void RefreshReminders();
        Task AddReminder(Reminder item);
        Task RemoveReminder(Reminder item);
        Task UpdateReminder(Reminder item);
        void ScheduleReminder(Reminder item);
        Task ImportReminders(List<Reminder> list);
    }
}