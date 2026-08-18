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
        }

        public List<Entry> Servers = new List<Entry>();

        public int EditingIndex = -1;

        public void Load(ListBox listBox)
        {
            Servers.Clear();
            Servers.AddRange(Settings.Data.PrivateServers);

            RefreshList(listBox);
        }

        public void Save()
        {
            Settings.Data.PrivateServers = new List<Entry>(Servers);
            Settings.Save();
        }

        public void RefreshList(ListBox listBox)
        {
            listBox.Items.Clear();

            for (int i = 0; i < Servers.Count; i++)
                listBox.Items.Add($"User ID: {Servers[i].UserId} | Private Server configured");
        }

        public bool IsValidUserId(string userId)
        {
            return ulong.TryParse(userId, out ulong id) && id > 0;
        }

        public bool IsValidLink(string link)
        {
            Uri uri;

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
    }
}