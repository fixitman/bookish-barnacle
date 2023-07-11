using Quartz;
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
    public IScheduler Scheduler { get; }

    public MainWindowVM(ReminderManager mgr)
    {
        Reminders = mgr;       
    }

    
}
