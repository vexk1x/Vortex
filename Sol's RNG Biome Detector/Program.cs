using System;
using System.Text;
using System.Text.Json;
using System.Net.Http;

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
                description = $"[Join Server]({pslink})",
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
    }

}
    
