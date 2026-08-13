using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Sol_s_RNG_Biome_Detector
{
    internal static class Program
    {
        

        [STAThread]

        static void Main()
        {

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

        }

        public static async Task PostToWebhook(string webhookURL, string Biome, string whatping, bool doping, string pslink, int color)
        {
            using HttpClient client = new HttpClient();
            
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

            await client.PostAsync(webhookURL, content);
        }

        public static async Task StartStopWebhook(string webhookURL, bool Started, TimeSpan sessionTime)
        {
            using HttpClient client = new HttpClient();
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

            await client.PostAsync(webhookURL, content);

        }
    }
}
    
