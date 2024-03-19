using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reminder_WPF.Models;
using Reminder_WPF.Utilities;
using Reminder_WPF.Views;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Reminder_WPF.Services
{
    public class APIReminderRepo : IDataRepo
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<APIReminderRepo> _logger;
       
        public APIReminderRepo(IHttpClientFactory factory, ILogger<APIReminderRepo> logger)
        {
            _factory = factory;
            _logger = logger;                      
        }
        
        
        public async Task<Result<List<Reminder>>> GetRemindersAsync()
        {
            try
            {
                using (var client = await GetClient())
                {
                    var result = await client.GetAsync("Reminders");
                    result.EnsureSuccessStatusCode();
                    var data = await result.Content.ReadFromJsonAsync<List<Reminder>>();
                    return Result.Ok(data?? new List<Reminder>());
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogCritical($"Exception Thrown: {e.Message}");
                return Result.Fail<List<Reminder>>(e.Message);
            }

        }

        public async Task<Result<Reminder?>> GetReminderByIdAsync(int id)
        {
            try
            {
                using var client = await GetClient();
                var result = await client.GetAsync($"reminders/{id}");
                result.EnsureSuccessStatusCode();
                var reminder = await result.Content.ReadFromJsonAsync<Reminder?>();
                return Result.Ok(reminder);
            }
            catch (HttpRequestException e)
            {
                _logger.LogCritical($"Exception Thrown: {e.Message}");
                return Result.Fail<Reminder?>(e.Message);
            }
        }

        public async Task<Result<Reminder?>> AddReminderAsync(Reminder item)
        {
            try
            {
                using var client = await GetClient();
                var result = await client.PostAsJsonAsync<Reminder>($"reminders", item);
                result.EnsureSuccessStatusCode();
                if (result != null)
                {
                    var reminder = await result.Content.ReadFromJsonAsync<Reminder>();
                    return Result.Ok(reminder);
                }
                return Result.Ok<Reminder?>(null);
            }
            catch (HttpRequestException e)
            {
                _logger.LogCritical($"Exception Thrown: {e.Message}");
                return Result.Fail<Reminder?>(e.Message);
            }

        }

        public async Task<Result> DeleteReminderAsync(Reminder item)
        {
            try
            {
                using var client = await GetClient();
                var result = await client.DeleteAsync($"reminders/{item.id}");
                result.EnsureSuccessStatusCode();
                return Result.Ok();
            }
            catch (HttpRequestException e)
            {
                _logger.LogCritical($"Exception Thrown: {e.Message}");
                return Result.Fail(e.Message);
            }
        }

        private async Task<HttpClient> GetClient()
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri($"http://{AppSettings.Default.API_Host}:{AppSettings.Default.API_Port}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetToken());
            return client;
        }

        private async Task<string?> GetToken()
        {
            //if (AppSettings.Default.API_TokenExpiration > DateTime.Now)
            //{
            //    _logger.LogInformation("Token OK. No Update");
            //    return AppSettings.Default.API_Token;
            //}

            LoginModel credentials = GetCredentials();
            
            
            using (var client = _factory.CreateClient()) 
            {
                client.BaseAddress = new Uri($"http://{AppSettings.Default.API_Host}:{AppSettings.Default.API_Port}");
                var login = new LoginModel { UserName = AppSettings.Default.API_Username, password = AppSettings.Default.API_Password };
                var r = await client.PostAsJsonAsync<LoginModel>("Account/login", login);
                if (r.IsSuccessStatusCode)
                {
                    var token = await r.Content.ReadFromJsonAsync<LoginResponse>();
                    if(token != null)
                    {
                        AppSettings.Default.API_Token = token.token;
                        AppSettings.Default.API_TokenExpiration = DateTime.Parse(token.expiration);
                        AppSettings.Default.Save();
                        _logger.LogInformation($"New Tolen : {token.token}");
                        return token.token;
                    }
                }
                return null;
            }
        }

        private LoginModel GetCredentials()
        {
            LoginModel Credentials = new LoginModel();
            //if (AppSettings.Default.API_RememberCredentials)
            //{
            //    Credentials.UserName = AppSettings.Default.API_Username;
            //    Credentials.password = AppSettings.Default.API_Password;
            //    return Credentials;
            //}

            var dlg = new CredentialsDialog(Credentials);
            if(dlg.ShowDialog() == true)
            {
                return Credentials;
            }
            return new LoginModel();
        }
    }
}
