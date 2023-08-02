using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quartz;
using Reminder_WPF.Models;
using Reminder_WPF.Services;
using Reminder_WPF.Views;
using System;
using System.Windows;

namespace Reminder_WPF.ViewModels;

public partial class MainWindowVM : ObservableObject
{
    public MainWindow? MainWindow { get; set; }
    public ReminderManager Reminders { get; }
    public IScheduler Scheduler { get; }
    [ObservableProperty]
    public Reminder? selectedItem = null;     
    public bool CanDelete { get { return SelectedItem != null; } }
    public bool CanAdd { get { return true; } }
    public bool CanEdit { get { return SelectedItem != null; } }

    public MainWindowVM(ReminderManager mgr, IScheduler scheduler)
    {
        Reminders = mgr;
        Scheduler = scheduler;
        MainWindow = null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    public void DeleteSelected()
    {
        if (SelectedItem != null)
        {
            Reminders.RemoveReminder(SelectedItem);
        }
    }

    [RelayCommand(CanExecute = nameof (CanAdd))]
    public void AddClicked()
    {
        var addDlg = new AddEditReminderDlg();
        addDlg.Title = "Add Reminder";
        addDlg.Owner = MainWindow;
        if(addDlg.ShowDialog() == true)
        {
            AddReminder(addDlg.Reminder);
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    public void EditClicked()
    {
        var addDlg = new AddEditReminderDlg(SelectedItem);
        addDlg.Title = "Edit Reminder";
        addDlg.Owner = MainWindow;
        if (addDlg.ShowDialog() == true)
        {
            UpdateReminder(addDlg.Reminder);
        }
    }

    partial void OnSelectedItemChanged(Reminder? value)
    {
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        EditClickedCommand.NotifyCanExecuteChanged();
    }

    public void UpdateReminder(Reminder r)
    {
        Reminders.UpdateReminder(r);
    }

    public void AddReminder(Reminder r)
    {
        Reminders.AddReminder(r);
    }

}
