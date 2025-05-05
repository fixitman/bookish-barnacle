using Microsoft.Extensions.Logging;
using Reminder_WPF.Models;
using Reminder_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
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
    private Timer RefreshTimer { get; set; }

    private Dictionary<int,int>SnoozeTimes = new Dictionary<int,int>();

    //public string Name => "ReminderManager";

    public ReminderManager(IDataRepo dataRepo, ReminderScheduler reminderScheduler, ILogger<ReminderManager> logger)
    {
        _logger = logger;
        logger.LogDebug("ReminderManager");
        DataRepo = dataRepo;
        RemScheduler = reminderScheduler;
        RefreshTimer = new Timer(
            (object? state) => { RefreshReminders(); _logger.LogDebug("refresh"); },
            null,
            (long)TimeSpan.FromMinutes(10).TotalMilliseconds,
            (long)TimeSpan.FromMinutes(10).TotalMilliseconds
        );

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
                if(r.ReminderTime < DateTime.Now && r.Recurrence == Reminder.RecurrenceType.None)
                {
                    await DataRepo.DeleteReminderAsync(r);
                }else
                {
                    await AddReminder(r);
                }
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

        //strip millis and ticks
        r.ReminderTime = new DateTime(item.ReminderTime.Year,item.ReminderTime.Month,item.ReminderTime.Day,
                                item.ReminderTime.Hour,item.ReminderTime.Minute, item.ReminderTime.Second);

        if (item.id == 0)
        {
            var result = await DataRepo.AddReminderAsync(r);
            if (result.Success && result.Value != null)
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
            ScheduleReminder(r);
            Application.Current.Dispatcher.Invoke(() =>
            {
                var delay = TimeSpan.FromMilliseconds(RemScheduler.FindNext(r) + 1);                
                var next = DateTime.Now + delay;
                //strip millis and ticks
                r.ReminderTime = new DateTime(next.Year, next.Month, next.Day,next.Hour,next.Minute,next.Second);
                Add(r);
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
        _ = Application.Current.Dispatcher.BeginInvoke(() =>
        {
            var dlg = new NotificationWindow(reminder?.ReminderText ?? "Empty");
            dlg.Owner = ((App)Application.Current).MainWindow;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            System.Media.SystemSounds.Exclamation.Play();
            dlg.SnoozeMinutes = SnoozeTimes.ContainsKey(reminder.id) ? SnoozeTimes[reminder.id] : 10;
            var result = dlg.ShowDialog();
            if (dlg.WasSnoozed)
            {

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
                if (reminder.Recurrence == Reminder.RecurrenceType.None)
                {
                    _ = RemoveReminder(reminder);
                }
                else
                {
                    RemScheduler.ScheduleNext(reminder);
                    var next = DateTime.Now + TimeSpan.FromMilliseconds(RemScheduler.FindNext(reminder) + 1);
                    reminder.ReminderTime = next;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
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
            var itemToDelete = this.First(r => r.id == item.id);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Remove(itemToDelete);
            }, null);
        }
    }

    public async Task UpdateReminder(Reminder item)
    {
        _logger.LogDebug("UpdateReminder");
        var current = this.First(r => r.id == item.id);
        if (current == null)
        {
            await AddReminder(item);
        }
        else if (item.Equals(current) == false)
        {   
            await RemoveReminder(item);
            item.id = 0;
            await AddReminder(item);
        }
    }

    private static void ShowError(string error)
    {
        MessageBox.Show(error, "Reminders - Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void RefreshReminders()
    {
        Application.Current.Dispatcher.Invoke(async() => {
            var result = await DataRepo.GetRemindersAsync();
            if (result.IsFailure)
            {
                return;
            }
            foreach (Reminder existing in this)
            {
                if (result.Value.FirstOrDefault(r => r.id == existing.id) == null)
                {
                    RemScheduler.DeleteReminder(existing.id);
                    this.Remove(existing);
                }
            }
            foreach (Reminder reminder in result.Value)
            {
                await UpdateReminder(reminder);
            }
        });
                    
    }

   




}
