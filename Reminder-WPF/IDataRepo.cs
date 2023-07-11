using System.Collections.Generic;

namespace Reminder_WPF
{
    public interface IDataRepo
    {
        Reminder AddReminder(Reminder item);
        List<Reminder> GetReminders();
    }
}