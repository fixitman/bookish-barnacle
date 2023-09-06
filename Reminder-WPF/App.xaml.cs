using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Reminder_WPF.Services;
using Reminder_WPF.ViewModels;
using Reminder_WPF.Views;
using System.Windows;

namespace Reminder_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _host;
        private IScheduler _scheduler;
        private TaskbarIcon _taskbarIcon;

        public static IHost GetHost() { return _host; }

        public App()
        {
            _host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowVM>();
                    services.AddSingleton<ReminderManager>();
                    services.AddSingleton<IScheduler, StdScheduler>((provider) =>
                    {
                        return (StdScheduler) new StdSchedulerFactory().GetScheduler().Result;
                    });
                    //services.AddScoped<IDataRepo, TestReminderRepo>();
                    services.AddScoped<IDataRepo, SQLiteReminderRepo>();
                })
                .Build();

        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
            _scheduler = await new StdSchedulerFactory().GetScheduler();
            await _scheduler.Start();
            _taskbarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            await _scheduler.Shutdown();
            using(_host)
            {
                await _host.StopAsync();
            }
        }
    }
}
