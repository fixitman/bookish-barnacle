using System.Net.Http.Json;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://fixitmanmike2.ddns.net:80");
                string u = "mike";
                string p = "Penny1992";
                Login body = new Login { username = u, password = p };
                var result = client.PostAsJsonAsync<Login>("Account/login", body).Result;
                Console.WriteLine( result.Content.ReadFromJsonAsync<Response>().Result.token);
            }
            

            Console.ReadLine();
        }
    }

    internal class Response
    {
        public string token { get; set; }
        public string expiration { get; set; }
    }

    internal class Login
    {
        public string username { get; set; }
        public string password { get; set; }
    }



}
