﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

public class ReminderManager : ObservableCollection<Reminder>, IReminderManager
{
    public static readonly string REMINDERTEXT = "reminderText";
    public static readonly string REMINDERID = "reminderID";

    private readonly ILogger _logger;    
    private IDataRepo DataRepo { get; }
    private ReminderScheduler RemScheduler { get; }

    private Dictionary<int,int>SnoozeTimes = new Dictionary<int,int>();

    public string Name => "ReminderManager";

    public ReminderManager(IDataRepo dataRepo, ReminderScheduler reminderScheduler, ILogger<ReminderManager> logger)
    {
        _logger = logger;
        logger.LogDebug("ReminderManager");
        DataRepo = dataRepo;
        RemScheduler = reminderScheduler;
        
        Task.Run(async () =>
        {
            await GetAllReminders();
        });

    }


    private async Task GetAllReminders()
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
        RemScheduler.ScheduleReminder(item, Callback);

    }

    private void Callback(object? state)
    {
        Reminder? reminder = (Reminder?)state;
        if (reminder == null) return;
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            var dlg = new NotificationWindow(reminder?.ReminderText ?? "Empty");
            dlg.Owner = ((App)Application.Current).MainWindow;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            System.Media.SystemSounds.Exclamation.Play();
            dlg.SnoozeMinutes = SnoozeTimes.ContainsKey(reminder.id) ? SnoozeTimes[reminder.id] : 10;
            var result = dlg.ShowDialog();
            if (dlg.WasSnoozed)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                RemScheduler.SnoozeReminder(reminder, dlg.SnoozeMinutes);
                if (SnoozeTimes.ContainsKey(reminder.id))
                {
                    SnoozeTimes[reminder.id] = dlg.SnoozeMinutes;
                }
                else
                {
                    SnoozeTimes.Add(reminder.id, dlg.SnoozeMinutes);
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {

                    await RemoveReminder(reminder);                   
#pragma warning restore CS8604 // Possible null reference argument.
                }, null);
            }
        });
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
            RemScheduler.DeleteReminder(item.id);
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

    public async Task RefreshReminders()
    {
        this.Clear();
        await GetAllReminders();        
    }


    

    
}
