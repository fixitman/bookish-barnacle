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
        private IHost _host;
        private IScheduler? _scheduler;
        TBMenu TBMenu = new TBMenu();
       
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
                    services.AddScoped<IDataRepo, SQLiteReminderRepo>();
                })
                .Build();

        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {                        
            await _host.StartAsync();

            if (AppSettings.Default.UpdateSettings)
            {
                AppSettings.Default.Upgrade();
                AppSettings.Default.UpdateSettings = false;
                AppSettings.Default.Save();
            }

            _scheduler = _host.Services.GetRequiredService<IScheduler>();
            await _scheduler.Start();
                      

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            if (!AppSettings.Default.StartMinimized)
            {
                MainWindow.Show();
            }
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            AppSettings.Default.Save();
            
            if (_scheduler != null)
            {
            await _scheduler.Shutdown();
            }
            
            using (_host)
            {
                await _host.StopAsync();
            }            
        }
    }
}
