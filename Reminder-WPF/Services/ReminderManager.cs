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
using Microsoft.Extensions.DependencyInjection;

namespace Reminder_WPF.Services;

public class ReminderManager : ObservableCollection<Reminder>, IReminderManager
{
    public static readonly string REMINDERTEXT = "reminderText";
    public static readonly string REMINDERID = "reminderID";

    private readonly ILogger _logger;    
    private IDataRepo LocalRepo { get; }
    private IDataRepo RemoteRepo { get; }
    
    private ReminderScheduler RemScheduler { get; }

    private DataSync dataSync { get; }
    private Timer RefreshTimer { get; set; }

    private Dictionary<string,int>SnoozeTimes = new Dictionary<string,int>();

    //public string Name => "ReminderManager";

    public ReminderManager(
        [FromKeyedServices("local")]IDataRepo localRepo, 
        [FromKeyedServices("remote")]IDataRepo remoteRepo,
        ReminderScheduler reminderScheduler, 
        ILogger<ReminderManager> logger
        )
    {

        _logger = logger;
        logger.LogDebug("ReminderManager");
        LocalRepo = localRepo;
        RemoteRepo = remoteRepo;
        dataSync = new DataSync(LocalRepo, RemoteRepo, logger);
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
        var result = await LocalRepo.GetRemindersAsync();
        if (result.IsSuccess && result.Value != null)
        {
            foreach (Reminder r in result.Value)
            {
                if(r.ReminderTime < DateTime.Now && r.Recurrence == Reminder.RecurrenceType.None)
                {
                    await LocalRepo.DeleteReminderAsync(r);
                }else
                {
                    r.ReminderTime = RemScheduler.GetNext(r);         
                    AddReminderLocal(r);
                }
            }
        }
        else if(result.Error != null)
        {
            ShowError(result.Error);
        }
    }

    public async Task AddReminder(Reminder item)
    {
        _logger.LogDebug("AddReminder");
        if (item == null) return;
        if(item.id == null || item.id == string.Empty) {
            item.id = Guid.NewGuid().ToString();
        }
        item.LastUpdated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        item.ReminderTime = RemScheduler.GetNext(item);
        var result = await LocalRepo.AddReminderAsync(item);
        if (result.IsSuccess && result.Value != null)
        {
            item = result.Value;
        }
        else if(result.Error != null)
        {
            ShowError(result.Error);
        }
        
        
        if (item != null)
        {
            ScheduleReminder(item);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Add(item);
            }, null);
            await dataSync.QueueChangeAsync(item, SyncOperation.Create);
            await dataSync.SyncAsync();
           
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
            dlg.SnoozeMinutes = SnoozeTimes.ContainsKey(reminder!.id) ? SnoozeTimes[reminder.id] : 10;
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

        var result = await LocalRepo.DeleteReminderAsync(item);
        if (result.IsFailure && result.Error != null)
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
            await dataSync.QueueChangeAsync(item, SyncOperation.Delete);
            await dataSync.SyncAsync();
        }
    }

    public async Task UpdateReminder(Reminder item)
    {
        _logger.LogDebug("UpdateReminder");
        var current = this.FirstOrDefault(r => r.id == item.id);
        if (current == null) {
            await AddReminder((Reminder)item);
            return;
        }
        
        if(item.Equals(current)) return;
        item.LastUpdated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        item.ReminderTime = RemScheduler.GetNext(item);
        
        var result = await LocalRepo.UpdateReminderAsync(item);
        if (result.IsSuccess && result.Value != null)
        {
            item = result.Value;
        }
        else if(result.Error != null)
        {
            ShowError(result.Error);
        }
        

        if (item != null)
        {
            ScheduleReminder(item);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Remove(current);
                Add(item.Clone());
            }, null);
            await dataSync.QueueChangeAsync(item, SyncOperation.Update);
            await dataSync.SyncAsync();
            
        }
        
    }
    
    private static void ShowError(string error)
    {
        MessageBox.Show(error, "Reminders - Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void RefreshReminders()
    {
        Application.Current.Dispatcher.Invoke(async() => {
            await dataSync.SyncAsync();
            var localResult = await LocalRepo.GetRemindersAsync();
            if (localResult.IsFailure )
            {
                return;
            }
            foreach (Reminder existing in this)
            {
                RemScheduler.DeleteReminder(existing.id);                
            }
            this.Clear();
            if(localResult.Value == null) return;
            foreach (Reminder reminder in localResult.Value)
            {
                AddReminderLocal(reminder);
            }
        });         
    }

    private void AddReminderLocal(Reminder reminder)
    {
        RemScheduler.ScheduleReminder(reminder, Callback);
        this.Add(reminder);
    }

    public async Task ImportReminders(List<Reminder> list)
    {
        foreach(Reminder reminder in list)
        {
            await dataSync.QueueChangeAsync(reminder, SyncOperation.Create);
        }
        RefreshReminders();
    }
}
