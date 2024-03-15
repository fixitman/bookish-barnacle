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
       
        private HttpClient _client;

        public APIReminderRepo(HttpClient client, ILogger<APIReminderRepo> logger, IAPIManager api)
        {
            _logger = logger;
            _api = api;

            _client = client;

            _client.BaseAddress = new Uri(_api.BasePath);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _api.GetToken());
            
            _logger.LogInformation($"API BasePath = {_api.BasePath}");           
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
}
