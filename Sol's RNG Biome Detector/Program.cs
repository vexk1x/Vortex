using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Sol_s_RNG_Biome_Detector
{
    internal static class Program
    {
        private static readonly HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        [STAThread]

        static void Main()
        {

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

        }

        public static async Task PostToWebhook(string webhookURL, string Biome, string whatping, bool doping, string pslink, int color)
        {
            
            var embed = new
            {
                
                title = $"Biome Started - {Biome}",
                description = $"[Join Server]({pslink})\n\n\n[Download Bloom](https://github.com/vexk1x/Bloom/releases)",
                color,
                footer = new
                {
                    text = $"Bloom | {DateTime.Now:dd/MM/yyyy HH:mm}"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };

            
            var payload = new
            {
                content = doping && !string.IsNullOrWhiteSpace(whatping) ? whatping : null,
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhookURL.Contains("?") ? webhookURL + "&wait=true" : webhookURL + "?wait=true";

            using HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }

        public static async Task StartStopWebhook(string webhookURL, bool Started, TimeSpan sessionTime)
        {
            
            string formattedTime = $"{(int)sessionTime.TotalHours:D2}:{sessionTime.Minutes:D2}:{sessionTime.Seconds:D2}";

            var embed = new
            {
                title = Started ? $"STARTED!" : $"STOPPED!",
                description = Started ? "[Download Bloom](https://github.com/vexk1x/Bloom/releases)" : $"Session Time: **{formattedTime}**\n\n[Download Bloom](https://github.com/vexk1x/Bloom/releases)",
                color = Started ? 0x0da65c : 0x940202,
                footer = new
                {
                    text = $"Bloom | {DateTime.Now:dd/MM/yyyy HH:mm}"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };

            var payload = new
            {
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhookURL.Contains("?") ? webhookURL + "&wait=true" : webhookURL + "?wait=true";

            using HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

        }

        public static async Task PostToWebhooks(IEnumerable<string> webhookURLs, string Biome, string whatping, bool doping, string pslink, int color)
        {
            List<Task> tasks = new List<Task>();

            foreach (string webhookURL in webhookURLs)
                tasks.Add(PostToWebhook(webhookURL, Biome, whatping, doping, pslink, color));

            await Task.WhenAll(tasks);
        }

        public static async Task StartStopWebhooks(IEnumerable<string> webhookURLs, bool Started, TimeSpan sessionTime)
        {
            List<Task> tasks = new List<Task>();

            foreach (string webhookURL in webhookURLs)
                tasks.Add(StartStopWebhook(webhookURL, Started, sessionTime));

            await Task.WhenAll(tasks);
        }
    }
}
    
