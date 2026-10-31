using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Vortex
{
    internal class Detector
    {
        private static readonly Dictionary<string, RobloxClient> Clients = new Dictionary<string, RobloxClient>();
        private static readonly HashSet<string> KnownFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static Dictionary<string, string> LastBiomeByUser = new Dictionary<string, string>();
        private static Dictionary<string, string> LastAuraByUser = new Dictionary<string, string>();

        private class RobloxClient
        {
            public string LogFile { get; set; } = "";
            public string UserId { get; set; } = "";
            public string Buffer { get; set; } = "";
            public long ReadPosition { get; set; } = 0;
            public DateTime LogWriteTime { get; set; }
        }

        public static async Task Biomes(CancellationToken t, Form1 form)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "logs");
            DateTime lastLogScan = DateTime.MinValue;

            Clients.Clear();
            KnownFiles.Clear();
            LastBiomeByUser.Clear();
            LastAuraByUser.Clear();

            Form1._PrintLogs("Detector Started.", Color.White);
            Form1._PrintLogs("Searching for Roblox clients.", Color.White);

            while (!t.IsCancellationRequested)
            {
                if ((DateTime.Now - lastLogScan).TotalSeconds >= 3)
                {
                    lastLogScan = DateTime.Now;

                    FindClients(path, form);
                }

                List<RobloxClient> currentClients = new List<RobloxClient>(Clients.Values);

                foreach (RobloxClient client in currentClients)
                {
                    try
                    {
                        if (!form.AccEnabled(client.UserId))
                        {
                            using (FileStream fstream = new FileStream(
                                client.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                            {
                                client.ReadPosition = fstream.Length;
                                client.Buffer = "";
                            }
                        }

                        string newData = ReadNewData(client);

                        if (!string.IsNullOrEmpty(newData))
                            ProcessNewData(client, newData, form);
                    }
                    catch
                    {
                        continue;
                    }
                }

                await Task.Delay(200);
            }
        }

        private static void FindClients(string path, Form1 form)
        {
            if (!Directory.Exists(path))
                return;

            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] files = directory.GetFiles("*.log");

            Array.Sort(files, CompareLogFiles);

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i];

                if (KnownFiles.Contains(file.FullName))
                    continue;

                if (!IsLogActive(file.FullName))
                    continue;

                try
                {
                    long position;
                    string log = ReadFullLog(file.FullName, out position);

                    if (!log.Contains("SetRichPresence"))
                        continue;

                    string userId = GetUserId(log);

                    if (string.IsNullOrWhiteSpace(userId))
                        continue;

                    KnownFiles.Add(file.FullName);

                    string currentBiome = GetLatestBiome(log);
                    string currentAura = GetLatestAura(log);

                    RobloxClient client = new RobloxClient()
                    {
                        LogFile = file.FullName,
                        UserId = userId,
                        ReadPosition = position,
                        LogWriteTime = file.LastWriteTime
                    };


                    if (Clients.ContainsKey(userId))
                    {
                        RobloxClient oldClient = Clients[userId];

                        if (file.LastWriteTime <= oldClient.LogWriteTime)
                            continue;

                        Clients[userId] = client;

                        Form1._PrintLogs($"Switched log for User ID {userId}: {file.Name}", Color.White);
                    }
                    else
                    {
                        Clients.Add(userId, client);

                        Form1._PrintLogs($"Found Roblox client | User ID: {userId}", Color.White);
                    }

                    if (!string.IsNullOrWhiteSpace(currentBiome))
                        ValidateBiome(client, currentBiome, form);


                    if (!string.IsNullOrWhiteSpace(currentAura))
                    {
                        if (!LastAuraByUser.ContainsKey(client.UserId))
                        {
                            LastAuraByUser[client.UserId] = currentAura;
                        }
                        else
                        {
                            ValidateAura(client, currentAura, form);
                        }
                    }


                }
                catch (Exception e)
                {
                    Form1._PrintLogs("Client Scan Error: " + e.Message, Color.DarkRed);
                }
            }
        }

        private static bool IsLogActive(string file)
        {
            try
            {
                FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);

                stream.Dispose();
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        private static int CompareLogFiles(FileInfo first, FileInfo second)
        {
            return second.LastWriteTime.CompareTo(first.LastWriteTime);
        }

        private static string GetUserId(string log)
        {
            string marker = "userid:";

            int start = log.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

            if (start == -1)
                return "";

            start += marker.Length;

            int end = log.IndexOf(",", start);

            if (end == -1)
                return "";

            return log.Substring(start, end - start).Trim();
        }

        private static string GetLatestBiome(string log)
        {
            string followup = "\"largeImage\":{\"hoverText\":\"";

            int start = log.LastIndexOf(followup, StringComparison.Ordinal);

            if (start == -1)
                return "";

            start += followup.Length;

            int end = log.IndexOf("\"", start, StringComparison.Ordinal);

            if (end == -1)
                return "";

            return log.Substring(start, end - start);
        }

        private static string ReadNewData(RobloxClient client)
        {
            FileStream stream = new FileStream(client.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

            if (stream.Length < client.ReadPosition)
            {
                client.ReadPosition = 0;
                client.Buffer = "";
            }

            if (stream.Length == client.ReadPosition)
            {
                client.LogWriteTime = File.GetLastWriteTime(client.LogFile);

                stream.Dispose();

                return "";
            }

            stream.Seek(client.ReadPosition, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true);

            string data = reader.ReadToEnd();

            client.ReadPosition = stream.Position;
            client.LogWriteTime = File.GetLastWriteTime(client.LogFile);

            stream.Dispose();
            reader.Dispose();

            return data;
        }

        private static void ProcessNewData(RobloxClient client, string newData, Form1 form)
        {
            string followup = "\"largeImage\":{\"hoverText\":\"";
            string data = client.Buffer + newData;

            if (data.IndexOf("Client:Disconnect", StringComparison.Ordinal) >= 0)
            {
                client.Buffer = "";

                form.DisableAccount(client.UserId);

                return;
            }

            client.Buffer = "";

            ProcessAuraData(client, data, form);

            int searchPosition = 0;

            while (searchPosition < data.Length)
            {
                int markerStart = data.IndexOf(followup, searchPosition, StringComparison.Ordinal);

                if (markerStart == -1)
                {
                    int keepLength = Math.Min(followup.Length - 1, data.Length);

                    if (keepLength > 0)
                        client.Buffer = data.Substring(data.Length - keepLength);

                    break;
                }

                int biomeStart = markerStart + followup.Length;
                int biomeEnd = data.IndexOf("\"", biomeStart, StringComparison.Ordinal);

                if (biomeEnd == -1)
                {
                    client.Buffer = data.Substring(markerStart);

                    break;
                }

                string biome = data.Substring(biomeStart, biomeEnd - biomeStart);

                ValidateBiome(client, biome, form);

                searchPosition = biomeEnd + 1;
            }
        }

        private static void ValidateBiome(RobloxClient client, string biome, Form1 form)
        {
            if (!form.AccEnabled(client.UserId))
                return;


            string PrivateServer = form.GetPrivateServerForUser(client.UserId);

            if (LastBiomeByUser.ContainsKey(client.UserId))
            {
                if (LastBiomeByUser[client.UserId] == biome)
                    return;
            }

            LastBiomeByUser[client.UserId] = biome;

            string username = RobloxAPI.GetUsernameEntry(client.UserId);


            Color PrintColor = ProcessData.GetColor(biome);

            Form1._PrintLogs($"New Biome Found: {biome} | Found by: {username}", PrintColor); // BIOME COLOR

            if (string.IsNullOrWhiteSpace(PrivateServer))
                Form1._PrintLogs($"No private server configured for User ID: {client.UserId}", Color.Red);

            _ = ProcessData.ProcessNewBiome(biome, PrivateServer, client.UserId, form);
        }

        private static string ReadFullLog(string file, out long position)
        {
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true);

            string text = reader.ReadToEnd();

            position = stream.Position;

            stream.Dispose();
            reader.Dispose();

            return text;
        }

        private static string GetLatestAura(string log)
        {
            string marker = "\"state\":\"Equipped \\\"";

            int start = log.LastIndexOf(marker, StringComparison.Ordinal);

            if (start == -1)
                return "";

            start += marker.Length;

            int end = log.IndexOf("\\\"", start, StringComparison.Ordinal);

            if (end == -1)
                return "";

            return log.Substring(start, end - start);
        }

        private static void ProcessAuraData(RobloxClient client, string data, Form1 form)
        {
            string marker = "\"state\":\"Equipped \\\"";
            int searchPosition = 0;

            while (searchPosition < data.Length)
            {
                int markerStart = data.IndexOf(marker, searchPosition, StringComparison.Ordinal);

                if (markerStart == -1)
                    break;

                int auraStart = markerStart + marker.Length;

                int auraEnd = data.IndexOf("\\\"", auraStart, StringComparison.Ordinal);

                if (auraEnd == -1)
                    break;

                string aura = data.Substring(auraStart, auraEnd - auraStart);

                ValidateAura(client, aura, form);

                searchPosition = auraEnd + 2;
            }
        }

        private static void ValidateAura(RobloxClient client, string aura, Form1 form)
        {
            if (string.IsNullOrWhiteSpace(aura))
                return;

            if (!form.AccEnabled(client.UserId))
                return;

            if (LastAuraByUser.TryGetValue(client.UserId, out string lastAura))
            {
                if (lastAura == aura)
                    return;

                if (lastAura is null)
                    return;
            }

            LastAuraByUser[client.UserId] = aura;

            string biome = LastBiomeByUser.ContainsKey(client.UserId) ? LastBiomeByUser[client.UserId] : "Normal";

            _ = ProcessData.ProcessNewAura(aura, client.UserId, biome, form);
        }

        public static string CurrentBiomeByUserA(string userId)
        {
            if (!Clients.TryGetValue(userId, out RobloxClient client))
                return "Inactive";

            if (LastBiomeByUser.TryGetValue(userId, out string biome))
                return biome;

            return "Loading";
        }
    }
}
