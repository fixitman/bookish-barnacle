using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reminder_WPF.Services;
using Reminder_WPF.ViewModels;
using Reminder_WPF.Views;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Reminder_WPF
{
    
    public partial class App : Application
    {
        private IHost _host; 
        public IServiceProvider Services { get => _host.Services; }

        TBMenu TBMenu = new TBMenu();
       
        public App()
        {
           var workingDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);     
           var logConfig = new ConfigurationBuilder()
                .SetBasePath(workingDir!)
                .AddJsonFile(Path.Combine(workingDir!,"appsettings.json"), optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(logConfig)
                .Enrich.FromLogContext()               
                .WriteTo.Debug()
                .WriteTo.File(new JsonFormatter(),Path.Combine(workingDir!,"logfile.json"))
                .CreateBootstrapLogger();
            
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowVM>();
                    services.AddSingleton<IReminderManager,ReminderManager>();
                    //services.AddSingleton<IScheduler, StdScheduler>((provider) =>
                    //{
                    //    return (StdScheduler) new StdSchedulerFactory().GetScheduler().Result;
                    //});
                    services.AddSingleton<ReminderScheduler,ReminderScheduler>();
                    //services.AddSingleton<IDataRepo, SQLiteReminderRepo>();
                    services.AddSingleton<IDataRepo, APIReminderRepo>();
                    //services.AddSingleton<IAPIManager, APIManager>();
                    services.AddHttpClient();
                })
                .UseSerilog((context, config) =>
                {
                    config.ReadFrom.Configuration(logConfig)
                        .Enrich.FromLogContext()
                        .WriteTo.Debug()
                        .WriteTo.File(new JsonFormatter(), Path.Combine(workingDir!, "logfile.json"));
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

                      

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            if (!AppSettings.Default.StartMinimized)
            {
                MainWindow.Show();
            }
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            AppSettings.Default.Save();
            
            
            
            using (_host)
            {
                await _host.StopAsync();
            }            
        }
    }
}
