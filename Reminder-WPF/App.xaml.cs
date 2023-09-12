using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Reminder_WPF.Services;
using Reminder_WPF.ViewModels;
using Reminder_WPF.Views;
using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace Reminder_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;
        private IScheduler? _scheduler;
        private string settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ConfigurationManager.AppSettings["Company"] ?? "Company",
                ConfigurationManager.AppSettings["AppName"] ?? "AppName",
                "settings.json");


        public App()
        {
            _host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowVM>();
                    services.AddSingleton<ReminderManager>();
                    services.AddSingleton<ISettings, Settings>();
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
            
            ISettings settings = _host.Services.GetRequiredService<ISettings>();            
            await settings.Load(settingsPath);
                        
            _scheduler = _host.Services.GetRequiredService<IScheduler>();
            await _scheduler.Start();

            _ = (TaskbarIcon)FindResource("TaskBarIcon");

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            if (!settings.HideMainWindowOnStartup)
            {
                MainWindow.Show();
            }
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            if (_scheduler != null)
            {
            await _scheduler.Shutdown();
            }

            ISettings s = _host.Services.GetRequiredService<ISettings>();
            await s.Save(settingsPath);

            using (_host)
            {
                await _host.StopAsync();
            }            
        }
    }
}
