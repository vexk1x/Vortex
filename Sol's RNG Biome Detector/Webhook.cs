using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Sol_s_RNG_Biome_Detector
{
    internal class Webhook
    {
        public List<string> Webhooks = new List<string>();

        public int EditingIndex = -1;

        private static readonly HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };


        public void Load(ListBox listBox)
        {
            Webhooks.Clear();
            Webhooks.AddRange(Settings.Data.Webhooks);

            RefreshList(listBox);
        }

        public void Save()
        {
            Settings.Data.Webhooks = new List<string>(Webhooks);
            Settings.Save();
        }

        public void RefreshList(ListBox listBox)
        {
            listBox.Items.Clear();

            for (int i = 0; i < Webhooks.Count; i++)
                listBox.Items.Add($"Webhook {i + 1} | {GetID(Webhooks[i])}");
        }

        public string GetID(string webhook)
        {
            try
            {
                Uri uri = new Uri(webhook);
                string[] parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3)
                    return parts[2];
            }
            catch
            {
            }

            return "Unknown";
        }

        public bool Contains(string webhook)
        {
            return Webhooks.Contains(webhook);
        }

        public void Add(string webhook)
        {
            Webhooks.Add(webhook);
            Save();
        }

        public void Update(int index, string webhook)
        {
            Webhooks[index] = webhook;
            Save();
        }

        public void Remove(int index)
        {
            Webhooks.RemoveAt(index);
            Save();
        }

        public string Get(int index)
        {
            return Webhooks[index];
        }

        public async Task PostToWebhook(string webhookURL, string Biome, string whatping, bool ping, string pslink, int color)
        {

            string biomeimage = await GetBiomeImage(Biome);

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            var embed = new
            {
                title = $"<t:{timestamp}:F> (<t:{timestamp}:R>)",
                description = $"## Biome Started - {Biome}\n## [Join Server]({pslink})\n\n\n[Download Bloom](https://github.com/vexk1x/Bloom/releases)",
                color,

                thumbnail = new
                {
                    url = biomeimage
                },

                footer = new
                {
                    text = $"Bloom | https://github.com/vexk1x/Bloom/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };


            var payload = new
            {
                content = ping && !string.IsNullOrWhiteSpace(whatping) ? whatping : null,
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhookURL.Contains("?") ? webhookURL + "&wait=true" : webhookURL + "?wait=true";

            using HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }

        private async Task<string> GetBiomeImage(string biome)
        {
            string fileName = biome.ToUpper().Replace(" ", "_");

            string biomeUrl = $"https://raw.githubusercontent.com/vexk1x/Bloom/main/Biomes/{fileName}.png";
            string eventUrl = "https://raw.githubusercontent.com/vexk1x/Bloom/main/Biomes/EVENT.png";

            using HttpResponseMessage response = await client.GetAsync(biomeUrl);

            if (response.IsSuccessStatusCode)
                return biomeUrl;

            return eventUrl;
        }

        public async Task StartStopWebhook(string webhookURL, bool Started, TimeSpan sessionTime, int biomesfound, int rarebiomesfound)
        {

            string formattedTime = $"{(int)sessionTime.TotalHours:D2}:{sessionTime.Minutes:D2}:{sessionTime.Seconds:D2}";

            var embed = new
            {
                description = Started ? "## STARTED!\n **[Download Bloom](https://github.com/vexk1x/Bloom/releases)**" : $"## STOPPED!\nSession Time: **{formattedTime}**\nBiomes Found this Session: **{biomesfound}**\nRare Biomes found this Session: **{rarebiomesfound}**\n\n **[Download Bloom](https://github.com/vexk1x/Bloom/releases)**",
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

        public async Task PostToWebhooks(IEnumerable<string> webhookURLs, string Biome, string whatping, bool ping, string pslink, int color)
        {
            List<Task> tasks = new List<Task>();

            foreach (string webhookURL in webhookURLs)
                tasks.Add(PostToWebhook(webhookURL, Biome, whatping, ping, pslink, color));

            await Task.WhenAll(tasks);
        }

        public async Task StartStopWebhooks(IEnumerable<string> webhookURLs, bool Started, TimeSpan sessionTime, int biomesfound, int rarebiomesfound)
        {
            List<Task> tasks = new List<Task>();

            foreach (string webhookURL in webhookURLs)
                tasks.Add(StartStopWebhook(webhookURL, Started, sessionTime, biomesfound, rarebiomesfound));

            await Task.WhenAll(tasks);
        }

        public async Task TestWebhook(string webhookURL)
        {

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var embed = new
            {
                title = $"Bloom",
                description = $"Webhook Test!",
                color = 0xFFFFFF,
                footer = new
                {
                    text = $"Bloom | https://github.com/vexk1x/Bloom/releases"
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
    }
}