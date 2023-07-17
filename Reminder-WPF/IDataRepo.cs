using System.Collections.Generic;

namespace Reminder_WPF
{
    public interface IDataRepo
    {
        Reminder AddReminder(Reminder item);
        bool DeleteReminder(Reminder item);
        List<Reminder> GetReminders();
    }
}