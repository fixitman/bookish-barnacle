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

    public MainWindowVM(IDataRepo data, IScheduler scheduler)
    {
        Scheduler = scheduler;
        Reminders = new ObservableCollection<Reminder>();
        List<Reminder> list = data.GetReminders();
        foreach (Reminder reminder in list)
        {
            Reminders.Add(reminder);
            Console.WriteLine(reminder.ReminderText);
            createScheduledJob(reminder);
        }            
    }

    private void createScheduledJob(Reminder reminder)
    {
        var myJob = JobBuilder.Create<ReminderJob>()
            .UsingJobData("reminderText", reminder.ReminderText)
            .Build();

        var trigger = TriggerBuilder.Create()
            .StartAt(reminder.ReminderTime)
            .ForJob(myJob)
            .Build();

        Scheduler.ScheduleJob(myJob, trigger);

    }
}
