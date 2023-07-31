using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quartz;
using Reminder_WPF.Models;
using Reminder_WPF.Services;

namespace Reminder_WPF.ViewModels;

public partial class MainWindowVM : ObservableObject
{
    public ReminderManager Reminders { get; }
    public IScheduler Scheduler { get; }
    [ObservableProperty]
    public Reminder? selectedItem = null;     
    public bool CanDelete { get { return SelectedItem != null; } }

    

    public MainWindowVM(ReminderManager mgr, IScheduler scheduler)
    {
        Reminders = mgr;
        Scheduler = scheduler;       
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    public void DeleteSelected()
    {
        if (SelectedItem != null)
        {
            Reminders.RemoveReminder(SelectedItem);
        }
    }

    partial void OnSelectedItemChanged(Reminder? value)
    {
        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    internal void UpdateReminder(Reminder selected)
    {
        Reminders.UpdateReminder(selected);
    }

}
