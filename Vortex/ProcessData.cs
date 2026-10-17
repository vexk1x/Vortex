using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Vortex
{
    internal class ProcessData
    {
        private static readonly AuraMatcher.MatchAura matcher = new AuraMatcher.MatchAura();

        public static Color GetColor(string Biome)
        {
            string[] BiomeCopy = Form1.CurrentBiomes;

            Color[] ColorCopy = Form1.Colors;

            for (int i = 0; i < BiomeCopy.Length; i++)
            {
                if (BiomeCopy[i].ToUpper() == Biome.ToUpper())
                    return ColorCopy[i]; // index 1 in BiomeCopy corresponds to Normal, index 1 in ColorCopy corresponds to the Color for the biome normal. (better for logs)
            }

            return Color.White;
        }

        public static async Task LoadAuras()
        {
            try
            {
                await matcher.initAuras();
            }
            catch (Exception e)
            {
                Form1._PrintLogs($"Aura List Error: {e.Message}", Color.DarkRed);
            }
        }

        public static async Task ProcessNewBiome(string biome, string privateserver, string userid, Form1 form)
        {
            bool rare = form.UpdateBiomeStats(biome);

            if (!form.BiomeSelected(biome) || Webhook.Webhooks.Count == 0 || string.IsNullOrWhiteSpace(privateserver))
                return;

            bool ping = rare;

            string mention = "";

            if (ping)
            {
                if (Settings.Data.PingRole && !string.IsNullOrWhiteSpace(Settings.Data.PingRoleID))
                    mention = $"<@&{Settings.Data.PingRoleID}>";
                else if (Settings.Data.PingUserID && !string.IsNullOrWhiteSpace(Settings.Data.PingUserIDValue))
                    mention = $"<@{Settings.Data.PingUserIDValue}>";
                else if (Settings.Data.PingEveryone)
                    mention = "@everyone";
            }

            await form.Webhooks.PostBiomeToWebhooks(biome, mention, ping, privateserver, GetColor(biome), RobloxAPI.GetUsernameEntry(userid), Settings.Data.IncludeUsername);

            Form1._PrintLogs($"Sent Biome: {biome} to your Webhook", Color.White);
        }

        public static async Task ProcessNewAura(string aura, string userid, string biome, Form1 form)
        {
            var result = await matcher.GetAura(aura, biome, 0);

            if (string.IsNullOrWhiteSpace(result.Name))
                return;

            if (result.Global)
                form.UpdateAuraStats();

            if (!Form1.Auras_Notifications || result.Rarity < Form1.Auras_MinPingRarity || Webhook.Webhooks.Count == 0)
                return;

            bool ping = Settings.Data.AuraPingUserID && Form1.Auras_NotificationID > 0;

            await Webhook.PostAuraToWebhooks(result.Name, result.Rarity.ToString(), result.bIsNative, biome, ping, Form1.Auras_NotificationID, Settings.Data.IncludeUsername, RobloxAPI.GetUsernameEntry(userid));

            Form1._PrintLogs($"Sent Aura: {aura} to your Webhook", Color.White);
        }
    }
}
