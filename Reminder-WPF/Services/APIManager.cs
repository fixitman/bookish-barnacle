using Reminder_WPF.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Reminder_WPF.Services
{
    public class APIManager : IAPIManager
    {
        private HttpClient _client;
        private string host;
        private int port;
        public string BasePath { get; set; }
        public string CurrentToken { get; set; }
        public DateTime TokenExpiration { get; set; }

        public APIManager(HttpClient client)
        {
            _client = client;
            host = AppSettings.Default.API_Host;
            port = AppSettings.Default.API_Port;
            BasePath = $"http://{host}:{port}/";

            CurrentToken = AppSettings.Default.API_Token;
            TokenExpiration = AppSettings.Default.API_TokenExpiration;

            _client.BaseAddress = new Uri(BasePath);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken());

        }

        public string? GetToken(string username, string password)
        {
            if (TokenExpiration > DateTime.Now)
            {
                return CurrentToken;
            }

            var body = new LoginModel { UserName = username, password = password };
            var result = _client.PostAsJsonAsync<LoginModel>("Account/login", body).Result;
            if (result.IsSuccessStatusCode)
            {
                var t = result.Content.ReadFromJsonAsync<LoginResponse>().Result;
                if (t != null)
                {
                    AppSettings.Default.API_Token = t.token;
                    CurrentToken = t.token;
                    AppSettings.Default.API_TokenExpiration = DateTime.Parse(t.expiration);
                    TokenExpiration = DateTime.Parse(t.expiration);
                    return t.token;
                }
            }

            return null;

        }

        public string? GetToken()
        {
            var UserName = AppSettings.Default.API_Username;
            var password = AppSettings.Default.API_Password;
            return GetToken(UserName, password);
        }



    }
}
