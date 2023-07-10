using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF;

public class MainWindowVM
{
    public ObservableCollection<Reminder> Reminders { get; }
    public MainWindowVM(IDataRepo data)
    {
        Reminders = new ObservableCollection<Reminder>();
        List<Reminder> list = data.GetReminders();
        foreach (Reminder reminder in list)
        {
            Reminders.Add(reminder);
            Console.WriteLine(reminder.ReminderText);
        }



    }
}
