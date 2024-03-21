using Reminder_WPF.Models;
using Reminder_WPF;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Windows;
using Reminder_WPF.Views;

namespace Reminder_WPF.Services
{
    public class CredentialManager : ICredentialManager
    {
        private readonly ILogger _logger;

        public CredentialManager(ILogger<CredentialManager> logger) {
            _logger = logger;
        }

        public async Task<string?> GetToken()
        {
            if (AppSettings.Default.API_TokenExpiration > DateTime.Now)
            {
                _logger.LogInformation("Token OK. No Update");
                return AppSettings.Default.API_Token;
            }

            //bool authenticating = true;
            while (true)
            {
                CredentialsModel? credentials = GetCredentials();
                if (credentials == null)
                {
                    return null;
                }

                using (var client = _factory.CreateClient())
                {
                    client.BaseAddress = new Uri($"http://{AppSettings.Default.API_Host}:{AppSettings.Default.API_Port}");
                    var login = new LoginModel { UserName = credentials.Username, password = credentials.Password };
                    var r = await client.PostAsJsonAsync<LoginModel>("Account/login", login);
                    if (r.IsSuccessStatusCode)
                    {
                        var token = await r.Content.ReadFromJsonAsync<LoginResponse>();
                        if (token != null)
                        {
                            AppSettings.Default.API_Token = token.token;
                            AppSettings.Default.API_TokenExpiration = DateTime.Parse(token.expiration);
                            AppSettings.Default.Save();
                            _logger.LogInformation($"New Tolen : {token.token}");
                            //authenticating = false;
                            return token.token;
                        }
                    }
                }
            }
            return null;
        }

        public void LogOut()
        {
            AppSettings.Default.API_Password = "";
            AppSettings.Default.API_Token = "";
            AppSettings.Default.API_TokenExpiration = DateTime.Now.AddMinutes(-1);
        }

        private CredentialsModel? GetCredentials()
        {
            CredentialsModel Credentials = new CredentialsModel()
            {
                Username = AppSettings.Default.API_Username,
                Password = "",
                Save = false
            };

            if (AppSettings.Default.API_RememberCredentials)
            {
                Credentials.Password = AppSettings.Default.API_Password;
                Credentials.Save = true;
                return Credentials;
            }


            bool cancelled = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dlg = new CredentialsDialog(Credentials);
                dlg.Owner = Application.Current.MainWindow;
                if (dlg.ShowDialog() == true)
                {
                    Credentials = dlg.Credentials;
                    if (Credentials.Save == true)
                    {
                        AppSettings.Default.API_Username = Credentials.Username;
                        AppSettings.Default.API_Password = Credentials.Password;
                        AppSettings.Default.API_RememberCredentials = true;
                        AppSettings.Default.Save();
                    }
                    else
                    {
                        AppSettings.Default.API_Password = "";
                        AppSettings.Default.API_RememberCredentials = false;
                        AppSettings.Default.Save();
                    }
                }
                else
                {
                    cancelled = true;
                }
            });
            if (cancelled)
            {
                return null;
            }
            return Credentials;

        }

    }
}



