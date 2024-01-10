using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Reminder_WPF.Services;
using Reminder_WPF.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Reminder_WPF.Models;

public class ReminderJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        //var reminderText = context.JobDetail.JobDataMap.GetString("reminderText");
        //MessageBox.Show(
        //    reminderText,
        //    "Reminder",
        //    MessageBoxButton.OK,
        //    MessageBoxImage.Information,
        //    MessageBoxResult.OK,
        //    MessageBoxOptions.DefaultDesktopOnly
        //    );

        var reminderId = context.JobDetail.JobDataMap.GetIntValue(ReminderManager.REMINDERID);
        var reminderText = context.JobDetail.JobDataMap.GetString(ReminderManager.REMINDERTEXT);
        IReminderManager reminders = ((App)Application.Current).Services.GetRequiredService<IReminderManager>();

        if (string.IsNullOrEmpty(reminderText) == false)
        {

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var dlg = new NotificationWindow(reminderText ?? "Empty");
                dlg.Owner = ((App)Application.Current).MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;                

                var result = dlg.ShowDialog();
                if (dlg.WasSnoozed)
                {
                    Reminder newReminder = new Reminder
                    {
                        id = 10000 + Random.Shared.Next(int.MaxValue - 10000),
                        Recurrence = Reminder.RecurrenceType.None,
                        ReminderText = reminderText ?? "Empty",
                        ReminderTime = DateTime.Now.AddMinutes(10)
                    };

                    reminders.ScheduleReminder(newReminder);
                }
            }); 
            
            
        }


        return Task.CompletedTask;
    }

}
