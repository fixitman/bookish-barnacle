using Reminder_WPF.Models;
using System.Collections.Generic;

namespace Reminder_WPF.Services
{
    public interface IDataRepo
    {
        Reminder AddReminder(Reminder item);
        bool DeleteReminder(Reminder item);
        List<Reminder> GetReminders();
    }
}