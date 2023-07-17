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

namespace Reminder_WPF;

public partial class MainWindowVM : ObservableObject
{
    public ObservableCollection<Reminder> Reminders { get; }
    public IScheduler Scheduler { get; }
    [ObservableProperty]
    public Reminder selectedItem; 
    public ICommand DeleteCommand { get;  }

    public MainWindowVM(ReminderManager mgr)
    {
        Reminders = mgr;       
        Scheduler = scheduler;
        DeleteCommand = new RelayCommand(Delete);
    }

    public bool CanDelete()
    {
        return SelectedItem != null;
    }

    public void Delete()
    {
        if (SelectedItem != null)
        {
            Reminders.RemoveReminder(SelectedItem);
        }
    }

    
}
