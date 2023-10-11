using Microsoft.Extensions.Logging;
﻿using Quartz;
using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.Services;

public class ReminderManager : ObservableCollection<Reminder>, IReminderManager, IJobListener
{
    private readonly ILogger logger;
    private IDataRepo DataRepo { get; }
    private IScheduler Scheduler { get; }

    public ReminderManager(IDataRepo dataRepo, IScheduler scheduler)
    public ReminderManager(IDataRepo dataRepo, IScheduler scheduler, ILogger<ReminderManager> logger)
    {
        this.logger = logger;
        logger.LogDebug("ReminderManager");
        DataRepo = dataRepo;
        Scheduler = scheduler;

        Task.Run(async () =>
        {
            foreach (Reminder r in await DataRepo.GetRemindersAsync())
            {
                await AddReminder(r);
            }

        });

    }

    public async Task  AddReminder(Reminder item)
    {
        logger.LogDebug("AddReminder");
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
        logger.LogDebug("ScheduleReminder");
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
        logger.LogDebug("RemoveReminder");
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
