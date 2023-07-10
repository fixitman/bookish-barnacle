using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Reminder_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;
        

        public App()
        {
            _host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddScoped<MainWindowVM>();
                    services.AddScoped<IDataRepo, ReminderRepo>();
                })
                .Build();
            
            

        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            using(_host)
            {
                _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }
    }
}
