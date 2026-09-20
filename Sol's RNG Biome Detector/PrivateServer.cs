using System.Text.Json;
using System.Windows.Forms;

namespace Sol_s_RNG_Biome_Detector
{
    internal class PrivateServer
    {
        public class Entry
        {
            public string UserId { get; set; } = "";
            public string Link { get; set; } = "";
            public string Username { get; set; } = "";
            public bool Enabled { get; set; } = true;
        }

        public List<Entry> Servers = new List<Entry>();

        public int EditingIndex = -1;

        public async Task Load(ListBox listBox)
        {
            Servers.Clear();
            Servers.AddRange(Settings.Data.PrivateServers);

            await RefreshList(listBox);
        }

        public void Save()
        {
            Settings.Data.PrivateServers = new List<Entry>(Servers);
            Settings.Save();
        }

        public async Task RefreshList(ListBox listBox)
        {
            listBox.Items.Clear();

            for (int i = 0; i < Servers.Count; i++)
            {
                Servers[i].Username = await RobloxAPI.GetUsername(Servers[i].UserId);

                string status = Servers[i].Enabled ? "" : "(Disabled)";

                listBox.Items.Add($"User: {Servers[i].Username}{status} | Private Server configured");
            }
        }

        public bool IsValidUserId(string userId)
        {
            return ulong.TryParse(userId, out ulong id) && id > 0;
        }

        public bool IsValidLink(string link)
        {
            Uri? uri;

            if (!Uri.TryCreate(link, UriKind.Absolute, out uri))
                return false;

            return uri.Host.Equals("roblox.com", StringComparison.OrdinalIgnoreCase) || uri.Host.EndsWith(".roblox.com", StringComparison.OrdinalIgnoreCase);
        }

        public int FindUser(string userId)
        {
            for (int i = 0; i < Servers.Count; i++)
            {
                if (Servers[i].UserId == userId)
                    return i;
            }

            return -1;
        }

        public void Add(string userId, string link)
        {
            Servers.Add(new Entry
            {
                UserId = userId,
                Link = link
            });

            Save();
        }

        public void Update(int index, string userId, string link)
        {
            if (Servers[index].UserId != userId)
                Servers[index].Username = "";

            Servers[index].UserId = userId;
            Servers[index].Link = link;

            Save();
        }

        public void Remove(int index)
        {
            Servers.RemoveAt(index);

            Save();
        }

        public Entry Get(int index)
        {
            return Servers[index];
        }

        public string GetForUser(string userId)
        {
            foreach (Entry entry in Servers)
            {
                if (entry.UserId == userId)
                    return entry.Link;
            }

            return "";
        }

        public string GetUsername(string userid)
        {
            foreach (Entry entry in Servers)
            {
                if (entry.UserId != userid)
                    continue;

                if (!string.IsNullOrWhiteSpace(entry.Username))
                    return entry.Username;

                return userid;
            }

            return userid;
        }
    }
}