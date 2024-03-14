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
    internal class APIReminderRepo : IDataRepo
    {
        private readonly ILogger<APIReminderRepo> _logger;
        private readonly IAPIManager _api;
        private string basePath;
        private HttpClient _client;

        public APIReminderRepo(HttpClient client, ILogger<APIReminderRepo> logger, IAPIManager api)
        {
            _logger = logger;
            _api = api;

            _client = client;
            _client.BaseAddress = new Uri(_api.BasePath);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _api.GetToken());
            
            _logger.LogInformation($"API BasePath = {basePath}");           
        }
        public async Task<List<Reminder>> GetRemindersAsync()
        {
            var r = await _client.GetAsync("Reminders");
            
            r.EnsureSuccessStatusCode();
            var data = await r.Content.ReadFromJsonAsync<List<Reminder>>();
            return data;
            
        }


        public async Task<Reminder?> GetReminderByIdAsync(int id)
        {
            var reminder = await _client.GetFromJsonAsync<Reminder?>($"reminders/{id}");
            return reminder;
        }


        
        
        
        
        
        
        
        
        
        /*============================================================================================*/


        public Task<Reminder> AddReminderAsync(Reminder item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteReminderAsync(Reminder item)
        {
            throw new NotImplementedException();
        }


    }

    public class APIManager : IAPIManager
    {
        private HttpClient _client;
        private string host;
        private int port;
        public string BasePath { get; internal set; }
        private string _currentToken = "";
        private DateTime TokenExpires = DateTime.Now.AddMinutes(-1);

        public APIManager(HttpClient client)
        {
            _client = client;
            host = AppSettings.Default.API_Host;
            port = AppSettings.Default.API_Port;
            BasePath = $"http://{host}:{port}/";
            
            _currentToken = AppSettings.Default.API_Token;
            TokenExpires = AppSettings.Default.API_TokenExpiration;
            
            _client.BaseAddress = new Uri(BasePath);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken() );
        }

        public string? GetToken()
        {
            if(TokenExpires > DateTime.Now)
            {
                return _currentToken;
            }

            var body = new LoginModel {UserName=AppSettings.Default.API_Username, password=AppSettings.Default.API_Password};
            var result = _client.PostAsJsonAsync<LoginModel>("Account/login", body).Result;
            if(result.IsSuccessStatusCode)
            {
                var t = result.Content.ReadFromJsonAsync<LoginResponse>().Result;
                if(t != null)
                {
                    AppSettings.Default.API_Token = t.token;
                    _currentToken = t.token;
                    AppSettings.Default.API_TokenExpiration = DateTime.Parse(t.expiration);
                    TokenExpires = DateTime.Parse(t.expiration);
                    return t.token;
                }
            }

            return null;
            
        }



    }
}
