using System;
using System.Text.Json;

namespace Sol_s_RNG_Biome_Detector
{
    public static class RobloxAPI
    {
        public static async Task<string> GetUsername(string userid)
        {
            using HttpClient client = new HttpClient();

            string? username = "";

            try
            {
                string json = await client.GetStringAsync($"https://users.roblox.com/v1/users/{userid}");

                using JsonDocument document = JsonDocument.Parse(json);

                username = document.RootElement.GetProperty("name").GetString();
            }
            catch
            {
                return userid;
            }

            return username!;
        }
    }
}
