﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Reminder_WPF.Models;
using Reminder_WPF.ViewModels;
using System.Windows.Data;

namespace Reminder_WPF.Views;


public partial class MainWindow : Window
{
    public MainWindow(MainWindowVM vM)
    {
        InitializeComponent();
        DataContext = vM;
        VM = vM;
    }

    public MainWindowVM VM { get; }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var lv = (ListView)sender;
        VM.SelectedItem = (Reminder)lv.SelectedItem;
    }

    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if(e.ChangedButton == System.Windows.Input.MouseButton.Left)
        {
            var lv = (ListView)sender;
            Reminder? selected = lv.SelectedItem as Reminder;
            if(selected != null)
            {
                //var win1 = new Window1(selected);       
                //win1.Closed += Win1_Closed;
                //win1.Show();

                var dlg = new TestDialog();
                if (dlg.ShowDialog() == true)
                {
                    VM.SelectedItem!.ReminderText = dlg.Reply;
                    CollectionViewSource.GetDefaultView(VM.Reminders).Refresh();
                }


            }
        }
    }

    private void Win1_Closed(object? sender, System.EventArgs e)
    {
        var win1 = sender as Window1;
        if(win1 != null)
        {
            var t = win1.Reminder.ReminderText;
            win1.Closed -= Win1_Closed;              
        }
    }
}
