using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF;

public class ReminderManager : ObservableCollection<Reminder>
{
    private IDataRepo DataRepo { get; }
    private IScheduler Scheduler { get; }

    public ReminderManager(IDataRepo dataRepo, IScheduler scheduler)
    {
        DataRepo = dataRepo;
        Scheduler = scheduler;       
        foreach (Reminder r in dataRepo.GetReminders())
        {
            AddReminder(r);
        }
    }

    public void AddReminder(Reminder item)
    {
        if (item == null) return;
        Reminder r = item;
        if (item.id == 0)
        {
            r = DataRepo.AddReminder(item);
        }
        if (r != null)
        {
            base.Add(r);
            ScheduleReminder(r);
        }
    }

    private void ScheduleReminder(Reminder item)
    {
        var job = JobBuilder.Create<ReminderJob>()
            .WithIdentity(item.id.ToString())
            .UsingJobData("reminderText", item.ReminderText)
            .Build();

        var trigger = TriggerBuilder.Create()
            .StartAt(item.ReminderTime)
            .ForJob(job)
            .Build();

        Scheduler.ScheduleJob(job, trigger);        
    }

    public void RemoveReminder(Reminder item)
    {
        if(item == null) return;
        DataRepo.DeleteReminder(item);
        Scheduler.DeleteJob(new JobKey(item.id.ToString()));
        Remove(item);
    }

    
}
