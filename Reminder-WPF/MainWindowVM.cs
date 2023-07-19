using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Reminder_WPF;

public partial class MainWindowVM : ObservableObject
{
    public ReminderManager Reminders { get; }
    public IScheduler Scheduler { get; }

    [ObservableProperty]
    public Reminder? selectedItem = null; 

    

    public MainWindowVM(ReminderManager mgr, IScheduler scheduler)
    {
        Reminders = mgr;
        Scheduler = scheduler;
       
    }

    public bool CanDelete { get
        {
            return SelectedItem != null;
        }
    }

    [RelayCommand(CanExecute =nameof(CanDelete))]
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
}
