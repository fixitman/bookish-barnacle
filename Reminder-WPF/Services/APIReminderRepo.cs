using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.Services
{
    internal class APIReminderRepo : IDataRepo
    {
        private readonly ILogger<APIReminderRepo> _logger;
        private readonly IAPIManager _api;
        private string APIBase = "";
        private string host;
        private int port;
        private string basePath;
        private HttpClient client;

        public APIReminderRepo(IConfiguration configuration, ILogger<APIReminderRepo> logger, IAPIManager api)
        {
            _logger = logger;
            _api = api;
            
            basePath = _api.BasePath;
            _logger.LogInformation($"API BasePath = {basePath}");
            client = new HttpClient()
            {
                BaseAddress = new Uri(basePath)                
            };
            var token = api.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "token");
        }

        public Task<Reminder> AddReminderAsync(Reminder item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteReminderAsync(Reminder item)
        {
            throw new NotImplementedException();
        }

        public async Task<Reminder?> GetReminderByIdAsync(int id)
        {
            var reminder = await client.GetFromJsonAsync<Reminder?>($"reminders/{id}");
            return reminder;
        }

        public async Task<List<Reminder>> GetRemindersAsync()
        {
            var reminders = await client.GetFromJsonAsync<List<Reminder>>("reminders");
            return reminders;                
            
        }
    }

    public class IAPIManager
    {
        private readonly IConfiguration _configuration;
        private string host;
        private int port;

        public string? BasePath { get; internal set; }
        private string _currentToken = "";
        private DateTime TokenExpires = DateTime.Now.AddMinutes(-1);

        public IAPIManager(IConfiguration configuration)
        {
            _configuration = configuration;
            var API = _configuration.GetSection("API");
            host = IPAddress.Parse(API.GetValue<string>("Host")!).ToString();
            port = API.GetValue<int>("Port")!;
            BasePath = $"http://{host}:{port}/";
        }

        public string GetToken()
        {
            return "token";
        }



    }
}
