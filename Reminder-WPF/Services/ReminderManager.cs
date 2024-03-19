using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Reminder_WPF.Models;
using Reminder_WPF.Utilities;
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

    private readonly ILogger _logger;    
    private IDataRepo DataRepo { get; }
    private IScheduler Scheduler { get; }

    public string Name => "ReminderManager";

    public ReminderManager(IDataRepo dataRepo, IScheduler scheduler, ILogger<ReminderManager> logger)
    {
        _logger = logger;
        logger.LogDebug("ReminderManager");
        DataRepo = dataRepo;
        Scheduler = scheduler;
        Scheduler.ListenerManager.AddJobListener(this);
        Task.Run(async () =>
        {
            var result = await DataRepo.GetRemindersAsync();
            if (result.Success)
            {
                foreach (Reminder r in result.Value)
                {
                    await AddReminder(r);
                }
            }
            else
            {
                ShowError(result.Error);
            }

        });

    }


    public async Task AddReminder(Reminder item)
    {
        _logger.LogDebug("AddReminder");
        if (item == null) return;
        Reminder r = item;
        if (item.id == 0)
        {
            var result = await DataRepo.AddReminderAsync(item);
            if (result.Success)
            {
                r = result.Value;
            }
            else
            {
                ShowError(result.Error);
            }
        }
        if (r != null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Add(r);
                ScheduleReminder(r);
            }, null);
        }
        
    }

    public void ScheduleReminder(Reminder item)
    {
        _logger.LogDebug("ScheduleReminder");
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
        _logger.LogDebug("RemoveReminder");
        if (item == null) return;
        var result = await DataRepo.DeleteReminderAsync(item);
        if (result.IsFailure)
        {
            ShowError(result.Error);
        }
        else
        {
            await Scheduler.DeleteJob(new JobKey(item.id.ToString()));
            var r = this.Where(r => r.id == item.id).First();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Remove(r);
            }, null);
        }
    }

    public async Task UpdateReminder(Reminder item)
    {
        _logger.LogDebug("UpdateReminder");
        if (item.id > 0)
        {
            var id = item.id;
            await RemoveReminder(item);
            item.id = 0;
            await AddReminder(item);
        }
    }

    private static void ShowError(string error)
    {
        MessageBox.Show(error, "Reminders - Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
