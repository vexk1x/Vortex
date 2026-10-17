using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Vortex
{
    internal class Webhook
    {
        public static List<string> Webhooks = new List<string>();

        public int EditingIndex = -1;

        private static readonly HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(5),
        };

        public void Load(ListBox l)
        {
            Webhooks.Clear();
            Webhooks.AddRange(Settings.Data.Webhooks);
            RefreshList(l);
        }

        private void RefreshList(ListBox l)
        {
            l.Items.Clear();

            for (int i = 0; i < Webhooks.Count; i++)
                l.Items.Add($"Webhook {i + 1} | {GetID(Webhooks[i])}");
        }

        private string GetID(string webhook)
        {
            try
            {
                Uri uri = new Uri(webhook);

                string[] parts = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3)
                    return parts[2];
            }
            catch
            {
            }
            return "Unknown";
        }

        public void Save()
        {
            Settings.Data.Webhooks = new List<string>(Webhooks);
            Settings.Save();
        }

        public void Add(string webhook)
        {
            Webhooks.Add(webhook);
            Save();
        }

        public void Remove(int idx)
        {
            Webhooks.RemoveAt(idx);
            Save();
        }

        public void Update(int idx, string webhook)
        {
            Webhooks[idx] = webhook;
            Save();
        }

        public string Get(int idx)
        {
            return Webhooks[idx];
        }

        private async Task<string> GetBiomeImageURL(string biome)
        {
            string filename = biome.ToUpper().Replace(" ", "_");

            string biomeurl = $"https://raw.githubusercontent.com/vexk1x/Vortex/main/Biomes/{filename}.png";

            HttpResponseMessage response = await client.GetAsync(biomeurl);

            if (response.IsSuccessStatusCode)
            {
                response.Dispose();
                return biomeurl;
            }
            else
            {
                response.Dispose();
                return "https://raw.githubusercontent.com/vexk1x/Vortex/main/Biomes/EVENT.png";
            }
        }

        private async Task _StartStopWebhook(string webhook, bool startorstop, TimeSpan SessionTime, int biomesfound, int rarebiomesfound) // session values
        {
            string fmt = $"{(int)SessionTime.TotalHours:D2}:{SessionTime.Minutes:D2}:{SessionTime.Seconds:D2}";

            var embed = new
            {
                description = startorstop ? $"## STARTED!\n\n> Active Webhooks: {Webhook.Webhooks.Count}" : $"## STOPPED!\nSession Time: **{fmt}**\nBiomes Found this Session: **{biomesfound}**\nRare Biomes found this Session: **{rarebiomesfound}**",
                color = (startorstop ? Color.Green : Color.Red).ToArgb() & 0xFFFFFF,

                footer = new
                {
                    text = $"{Form1._Version}  | https://github.com/vexk1x/Vortex/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };

            var payload = new
            {
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            content.Dispose();
            response.Dispose();
        }

        public async Task StartStopWebhook(bool startorstop, TimeSpan SessionTime, int biomesfound, int rarebiomesfound)
        {
            List<Task> tasks = new List<Task>();

            foreach (string webhook in Webhooks)
                tasks.Add(_StartStopWebhook(webhook, startorstop, SessionTime, biomesfound, rarebiomesfound));

            await Task.WhenAll(tasks);
        }

        public static async Task TestWebhook(string webhook)
        {
            var embed = new
            {
                title = $"Vortex",
                description = $"Webhook Test!",
                color = 0xFFFFFF,
                footer = new
                {
                    text = $"{Form1._Version}  | https://github.com/vexk1x/Vortex/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };


            var payload = new
            {
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            content.Dispose();
            response.Dispose();
        }

        private static async Task _PostAuraToWebhook(string webhook, string aura, string rarity, bool native, string frombiome, string rolledby, bool ping, string d_userid)
        {
            var embed = new
            {
                title = $"**Aura Equipped - {aura}**",
                description = native ? $"\n\n{rolledby}\n**1/{rarity}**\n**From: {frombiome} [NATIVE]**" : $"\n\n{rolledby}\n**1/{rarity}**\n**From: {frombiome}**",
                color = 0xFFFFFF,

                footer = new
                {
                    text = $"{Form1._Version}  | https://github.com/vexk1x/Vortex/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };

            var payload = new
            {
                content = ping && !string.IsNullOrWhiteSpace(d_userid) ? d_userid : null,
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            content.Dispose();
            response.Dispose();
        }

        public static async Task PostAuraToWebhooks(string aura, string rarity, bool native, string frombiome, bool ping, long d_userid, bool includeuser, string username)
        {
            List<Task> tasks = new List<Task>();
            string rolled_by = "";

            if (includeuser && !string.IsNullOrWhiteSpace(username))
                rolled_by = $"> Rolled by {username}";

            if (!string.IsNullOrWhiteSpace(rarity))
                rarity = Int64.Parse(rarity).ToString("N0", CultureInfo.InvariantCulture);

            foreach (string webhook in Webhooks)
                tasks.Add(_PostAuraToWebhook(webhook, aura, rarity, native, frombiome, rolled_by, ping, $"<@{d_userid.ToString()}>"));

            await Task.WhenAll(tasks);
        }

        private async Task _PostBiomeToWebhook(string webhook, string biome, string whatping, bool ping, string privateserverlink, Color color, string username)
        {
            string biomeimage = await GetBiomeImageURL(biome);

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var embed = new
            {
                title = $"<t:{timestamp}:F> (<t:{timestamp}:R>)",
                description = $"## Biome Started - {biome}\n## [Join Server]({privateserverlink})\n\n{username}",
                color = color.ToArgb() & 0xFFFFFF,

                thumbnail = new
                {
                    url = biomeimage
                },

                footer = new
                {
                    text = $"{Form1._Version} | https://github.com/vexk1x/Vortex/releases"
                },
                timestamp = DateTime.UtcNow.ToString("o")
            };


            var payload = new
            {
                content = ping && !string.IsNullOrWhiteSpace(whatping) ? whatping : null,
                embeds = new[] { embed }
            };

            string json = JsonSerializer.Serialize(payload);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = webhook.Contains("?") ? webhook + "&wait=true" : webhook + "?wait=true";

            HttpResponseMessage response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            content.Dispose();
            response.Dispose();
        }

        public async Task PostBiomeToWebhooks(string biome, string whatping, bool ping, string privateserverlink, Color color, string username, bool includeuser)
        {
            List<Task> tasks = new List<Task>();
            string found_by = "";

            if (includeuser && !string.IsNullOrWhiteSpace(username))
                found_by = $"> Found by: {username}";

            foreach (string webhook in Webhooks)
                tasks.Add(_PostBiomeToWebhook(webhook, biome, whatping, ping, privateserverlink, color, found_by));

            await Task.WhenAll(tasks);
        }
    }
}
