using System.Globalization;
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

        public async Task PostToWebhook(string webhook, string Biome, string whatping, bool ping, string pslink, int color, string username)
        { 

            string biomeimage = await GetBiomeImage(Biome);

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            var embed = new
            {
                title = $"<t:{timestamp}:F> (<t:{timestamp}:R>)",
                description = $"## Biome Started - {Biome}\n## [Join Server]({pslink})\n\n{username}",
                color,

                thumbnail = new
                {
                    url = biomeimage
                },

                footer = new
                {
                    text = $"Vortex | https://github.com/vexk1x/Vortex/releases"
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

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            using HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }

        private async Task<string> GetBiomeImage(string biome)
        {
            string fileName = biome.ToUpper().Replace(" ", "_");

            string biomeUrl = $"https://raw.githubusercontent.com/vexk1x/Vortex/main/Biomes/{fileName}.png";
            string eventUrl = "https://raw.githubusercontent.com/vexk1x/Vortex/main/Biomes/EVENT.png";

            using HttpResponseMessage response = await client.GetAsync(biomeUrl);

            if (response.IsSuccessStatusCode)
                return biomeUrl;

            return eventUrl;
        }

        public async Task StartStopWebhook(string webhook, bool Started, TimeSpan sessionTime, int biomesfound, int rarebiomesfound)
        {

            string formattedTime = $"{(int)sessionTime.TotalHours:D2}:{sessionTime.Minutes:D2}:{sessionTime.Seconds:D2}";

            var embed = new
            {
                description = Started ? "## STARTED!" : $"## STOPPED!\nSession Time: **{formattedTime}**\nBiomes Found this Session: **{biomesfound}**\nRare Biomes found this Session: **{rarebiomesfound}**",
                color = Started ? 0x0da65c : 0x940202,

                footer = new
                {
                    text = $"Vortex | https://github.com/vexk1x/Vortex/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };

            var payload = new
            {
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            using HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

        }

        public async Task PostToWebhooks(string Biome, string whatping, bool ping, string pslink, int color, string userid, bool includeuser)
        {
            List<Task> tasks = new List<Task>();
            string username = "";
            string tempuser = "";

            if (includeuser)
            {
                tempuser = await GetUsername(userid);
                username = $"`Found by: {tempuser}`";
            }

            foreach (string webhook in Webhooks)
                tasks.Add(PostToWebhook(webhook, Biome, whatping, ping, pslink, color, username));

            await Task.WhenAll(tasks);
        }

        public async Task StartStopWebhooks(bool Started, TimeSpan sessionTime, int biomesfound, int rarebiomesfound)
        {
            List<Task> tasks = new List<Task>();

            foreach (string webhook in Webhooks)
                tasks.Add(StartStopWebhook(webhook, Started, sessionTime, biomesfound, rarebiomesfound));

            await Task.WhenAll(tasks);
        }

        public async Task TestWebhook(string webhook)
        {

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var embed = new
            {
                title = $"Vortex",
                description = $"Webhook Test!",
                color = 0xFFFFFF,
                footer = new
                {
                    text = $"Vortex | https://github.com/vexk1x/Vortex/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };


            var payload = new
            {
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            using HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetUsername(string userid)
        {
            string json = await client.GetStringAsync($"https://users.roblox.com/v1/users/{userid}");

            using JsonDocument document = JsonDocument.Parse(json);

            string? username = document.RootElement.GetProperty("name").GetString();

            if (string.IsNullOrWhiteSpace(username))
                return userid;

            return username;
        }

        public async Task PostAuraToWebhook(string webhook, string aura, string rolledby, bool ping, string DiscordUserId, string rarity, bool native, string frombiome)
        {

            var embed = new
            {
                title = $"**Aura Equipped - {aura}**",
                description = native ?  $"\n\n{rolledby}\n**1/{rarity}**\n**From: {frombiome} [NATIVE]**" : $"\n\n{rolledby}\n**1/{rarity}**\n**From: {frombiome}**",
                color = 0xFFFFFF,

                footer = new
                {
                    text = $"Vortex | https://github.com/vexk1x/Vortex/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };

            var payload = new
            {
                content = ping && !string.IsNullOrWhiteSpace(DiscordUserId) ? DiscordUserId : null,
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            using HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task PostAuraToWebhooks(string aura, string userid, bool includeuser, bool ping, string DiscordUserId, string rarity, bool native, string frombiome)
        {
            List<Task> tasks = new List<Task>();
            string username = "";
            string rolledby = "";

            if (includeuser)
                username = await GetUsername(userid);

            if (!string.IsNullOrWhiteSpace(username))
                rolledby = $"`Rolled by {username}`";

            if (!string.IsNullOrWhiteSpace(rarity))
                rarity = Int64.Parse(rarity).ToString("N0", CultureInfo.InvariantCulture);

            foreach (string webhook in Webhooks)
                tasks.Add(PostAuraToWebhook(webhook, aura, rolledby, ping, DiscordUserId, rarity, native, frombiome));

            await Task.WhenAll(tasks);
        }
    }
}