using Quartz;
using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.Services;

public class ReminderManager : ObservableCollection<Reminder>
{
    private IDataRepo DataRepo { get; }
    private IScheduler Scheduler { get; }

    public ReminderManager(IDataRepo dataRepo, IScheduler scheduler)
    {
        DataRepo = dataRepo;
        Scheduler = scheduler;

        Task.Run(async () =>
        {
            foreach (Reminder r in await dataRepo.GetRemindersAsync())
            {
                await AddReminder(r);
            }

        });

    }

    public async Task  AddReminder(Reminder item)
    {
        if (item == null) return;
        Reminder r = item;
        if (item.id == 0)
        {
            r = await DataRepo.AddReminderAsync(item);
        }
        if (r != null)
        {
            Add(r);
            ScheduleReminder(r);
        }
    }

    private void ScheduleReminder(Reminder item)
    {
        var job = JobBuilder.Create<ReminderJob>()
            .WithIdentity(item.id.ToString())
            .UsingJobData("reminderText", item.ReminderText)
            .Build();

        var trigger = TriggerBuilder.Create();
        if(item.Recurrence == Reminder.RecurrenceType.Weekly)
        {
            var cs = $"0 {item.ReminderTime.Minute} {item.ReminderTime.Hour} ? * {item.RecurrenceData}";
            trigger.WithCronSchedule(cs);
        }
        else if (item.Recurrence == Reminder.RecurrenceType.Daily)
        {
            var cs = $"0 {item.ReminderTime.Minute} {item.ReminderTime.Hour} * * ?";
            trigger.WithCronSchedule(cs);
        }
        else
        {
            trigger.StartAt(item.ReminderTime);     
        }
       
       
        trigger.ForJob(job);
        
        Scheduler.ScheduleJob(job, trigger.Build());
    }

    public async Task RemoveReminder(Reminder item)
    {
        if (item == null) return;
        await DataRepo.DeleteReminderAsync(item);
        await Scheduler.DeleteJob(new JobKey(item.id.ToString()));
        var r = this.Where(r => r.id == item.id).First();
        Remove(r);
    }

    public async Task UpdateReminder(Reminder item)
    {
        if (item.id > 0)
        {
            var id = item.id;
            await RemoveReminder(item);
            item.id = 0;
            await AddReminder(item);
        }
    }


}
