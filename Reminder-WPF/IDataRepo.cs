using System.Collections.Generic;

namespace Reminder_WPF
{
    public interface IDataRepo
    {
        List<Reminder> GetReminders();
    }
}