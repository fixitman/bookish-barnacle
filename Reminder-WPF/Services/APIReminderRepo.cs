using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reminder_WPF.Models;
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
        
        
        public async Task<List<Reminder>> GetRemindersAsync()
        {
            using(var client = await GetClient())
            {

            }
            return new List<Reminder>();


            //using (var client = GetClient())
            //{
            //    var result = client.GetAsync("Reminders").Result;
            //    result.EnsureSuccessStatusCode();
            //    var data = await result.Content.ReadFromJsonAsync<List<Reminder>>();
            //    client.Dispose();
            //    return data;         
            //}



        }

        public async Task<Reminder?> GetReminderByIdAsync(int id)
        {
            using var client = await GetClient();
            var reminder = await client.GetFromJsonAsync<Reminder?>($"reminders/{id}");
            return reminder;
        }

        public async Task<Reminder?> AddReminderAsync(Reminder item)
        {
            using var client = await GetClient();
            var result = await client.PostAsJsonAsync<Reminder>($"reminders", item);
            if (result != null)
            {
                var reminder = await result.Content.ReadFromJsonAsync<Reminder>();
                return reminder;
            }
            return null;

        }

        public async Task<bool> DeleteReminderAsync(Reminder item)
        {
            using var client = await GetClient();
            var result = await client.DeleteAsync($"reminders/{item.id}");
            return result.IsSuccessStatusCode;
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
            //    return AppSettings.Default.API_Token;
            //}

            using (var client = _factory.CreateClient()) 
            {
                client.BaseAddress = new Uri($"http://{AppSettings.Default.API_Host}:{AppSettings.Default.API_Port}");
                var login = new LoginModel { UserName = AppSettings.Default.API_Username, password = AppSettings.Default.API_Password };
                var r = await client.PostAsJsonAsync<LoginModel>("Account/login", login);
                if (r.IsSuccessStatusCode)
                {
                    var token = await r.Content.ReadFromJsonAsync<LoginResponse>();
                    AppSettings.Default.API_Token = token.token;
                    AppSettings.Default.API_TokenExpiration = DateTime.Parse(token.expiration);
                    return token.token;
                }
                return null;
            }
        }
    }
}
