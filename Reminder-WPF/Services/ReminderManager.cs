﻿using Microsoft.Extensions.Logging;
using Quartz;
using Reminder_WPF.Models;
using Reminder_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Reminder_WPF.Services;

public class ReminderManager : ObservableCollection<Reminder>, IReminderManager, IJobListener
{
    public static readonly string REMINDERTEXT = "reminderText";
    public static readonly string REMINDERID = "reminderID";


    private readonly ILogger logger;
    private readonly SynchronizationContext synchronizationContext;

    private IDataRepo DataRepo { get; }
    private IScheduler Scheduler { get; }


    public string Name => "ReminderManager";

    public ReminderManager(IDataRepo dataRepo, IScheduler scheduler, ILogger<ReminderManager> logger)
    {
        this.logger = logger;
        synchronizationContext = SynchronizationContext.Current!;
        logger.LogDebug("ReminderManager");
        DataRepo = dataRepo;
        Scheduler = scheduler;
        Scheduler.ListenerManager.AddJobListener(this);
        Task.Run(async () =>
        {
            foreach (Reminder r in await DataRepo.GetRemindersAsync())
            {
                await AddReminder(r);
            }

        });

    }

    public async Task AddReminder(Reminder item)
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

    public void ScheduleReminder(Reminder item)
    {
        logger.LogDebug("ScheduleReminder");
        var job = JobBuilder.Create<ReminderJob>()
            .WithIdentity(item.id.ToString())
            .UsingJobData(REMINDERTEXT, item.ReminderText)
            .UsingJobData(REMINDERID, item.id)
            .Build();

        var trigger = TriggerBuilder.Create();
        if (item.Recurrence == Reminder.RecurrenceType.Weekly)
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
        synchronizationContext.Post((state) =>
        {
            Remove(r);
        }, null);
    }

    public async Task UpdateReminder(Reminder item)
    {
        logger.LogDebug("UpdateReminder");
        if (item.id > 0)
        {
            var id = item.id;
            await RemoveReminder(item);
            item.id = 0;
            await AddReminder(item);
        }
    }



    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        var reminderId = context.JobDetail.JobDataMap.GetIntValue(REMINDERID);
        var reminderText = context.JobDetail.JobDataMap.GetString(REMINDERTEXT);
        Reminder? reminder = this.FirstOrDefault(r => r.id == reminderId);

        
        
        if (reminder != null && reminder.Recurrence == Reminder.RecurrenceType.None && reminderId != 0)
        {
            _ = RemoveReminder(reminder);
        }
        
       

        return Task.CompletedTask;

    }
}
