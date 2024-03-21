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
using System.Windows;
using System.Windows.Controls;

namespace Reminder_WPF.Services
{
    public class APIReminderRepo : IDataRepo
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<APIReminderRepo> _logger;
        private readonly ICredentialManager _credentialManager;

        public APIReminderRepo(IHttpClientFactory factory, ILogger<APIReminderRepo> logger, ICredentialManager credentialManager)
        {
            _factory = factory;
            _logger = logger;
            _credentialManager = credentialManager;
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
                var reminder = await result.Content.ReadFromJsonAsync<Reminder>();
                return Result.Ok(reminder);              
               
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await _credentialManager.GetToken());
            return client;
        }

        
    }
}
