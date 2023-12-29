using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Reminder_WPF.Services;
using Reminder_WPF.Views;
using System;
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
        return Task.CompletedTask;
    }

}
