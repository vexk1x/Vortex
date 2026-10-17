using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Vortex
{
    internal class RobloxAPI
    {
        private static List<string> RobloxUsernames = new List<string>();
        private static List<string> RobloxUserIds = new List<string>();

        public static async Task<string> GetUsername(string userid)
        {
            HttpClient client = new HttpClient();

            string username = "";

            try
            {
                string json = await client.GetStringAsync($"https://users.roblox.com/v1/users/{userid}");

                JsonDocument document = JsonDocument.Parse(json);

                username = document.RootElement.GetProperty("name").GetString();

                document.Dispose();
                client.Dispose();
            }
            catch
            {
                return userid;
            }

            AddUser(userid, username);

            return username;
        }

        private static void AddUser(string userid, string username)
        {
            if (!IsAdded(userid, RobloxUserIds) && !IsAdded(username, RobloxUsernames) && userid != "" && username != "")
            {
                RobloxUserIds.Add(userid);
                RobloxUsernames.Add(username);
            }
        }

        private static bool IsAdded(string Check, List<string> @List)
        {
            foreach (string entry in @List)
            {
                if (Check == entry)
                    return true;
            }
            return false;
        }

        public static string GetUsernameEntry(string userid)
        {
            int index = RobloxUserIds.IndexOf(userid);

            if (index != -1)
                return RobloxUsernames[index];

            return userid;
        }
    }
}
