using Sol_s_RNG_Biome_Detector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

class BiomeDetector
{
    private static readonly Dictionary<string, RobloxClient> clients = new Dictionary<string, RobloxClient>();
    private static readonly HashSet<string> knownFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private static Dictionary<string, string> lastBiomeByUser = new Dictionary<string, string>();
    private static Dictionary<string, string> lastAuraByUser = new Dictionary<string, string>();

    private class RobloxClient
    {
        public string LogFile { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Buffer { get; set; } = "";
        public long ReadPosition { get; set; } = 0;
        public DateTime LogWriteTime { get; set; }
    }

    public static async Task Biomes(Form1 form)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "logs");
        DateTime lastLogScan = DateTime.MinValue;

        clients.Clear();
        knownFiles.Clear();
        lastBiomeByUser.Clear();
        lastAuraByUser.Clear();

        form.PrintLogs("Detector Started.");
        form.PrintLogs("Searching for Roblox clients.");

        while (Form1.Start_Stop)
        {
            try
            {
                if ((DateTime.Now - lastLogScan).TotalSeconds >= 3)
                {
                    lastLogScan = DateTime.Now;

                    FindClients(path, form);
                }

                List<RobloxClient> currentClients = new List<RobloxClient>(clients.Values);

                foreach (RobloxClient client in currentClients)
                {
                    try
                    {
                        string newData = ReadNewData(client);

                        if (!string.IsNullOrEmpty(newData))
                            ProcessNewData(client, newData, form);
                    }
                    catch (Exception e)
                    {
                        form.PrintLogs($"Log Error | User ID {client.UserId}: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                form.PrintLogs("Detector Error: " + e.Message);
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

            if (knownFiles.Contains(file.FullName))
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

                knownFiles.Add(file.FullName);

                string currentBiome = GetLatestBiome(log);
                string currentAura = GetLatestAura(log);

                RobloxClient client = new RobloxClient();

                client.LogFile = file.FullName;
                client.UserId = userId;
                client.ReadPosition = position;
                client.LogWriteTime = file.LastWriteTime;

                if (clients.ContainsKey(userId))
                {
                    RobloxClient oldClient = clients[userId];

                    if (file.LastWriteTime <= oldClient.LogWriteTime)
                        continue;

                    clients[userId] = client;

                    form.PrintLogs($"Switched log for User ID {userId}: {file.Name}");
                }
                else
                {
                    clients.Add(userId, client);

                    form.PrintLogs($"Found Roblox client | User ID: {userId}");
                }

                if (!string.IsNullOrWhiteSpace(currentBiome))
                    ValidateBiome(client, currentBiome, form);


                if (!string.IsNullOrWhiteSpace(currentAura))
                {
                    if (!lastAuraByUser.ContainsKey(client.UserId))
                    {
                        lastAuraByUser[client.UserId] = currentAura;
                    }
                    else
                    {
                        ValidateAura(client, currentAura, form);
                    }
                }


            }
            catch (Exception e)
            {
                form.PrintLogs("Client Scan Error: " + e.Message);
            }
        }
    }

    private static bool IsLogActive(string file)
    {
        try
        {
            using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);

            return false;
        }
        catch (IOException)
        {
            return true;
        }
        catch
        {
            return false;
        }
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
        using FileStream stream = new FileStream(client.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

        if (stream.Length < client.ReadPosition)
        {
            client.ReadPosition = 0;
            client.Buffer = "";
        }

        if (stream.Length == client.ReadPosition)
        {
            client.LogWriteTime = File.GetLastWriteTime(client.LogFile);

            return "";
        }

        stream.Seek(client.ReadPosition, SeekOrigin.Begin);

        using StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true);

        string data = reader.ReadToEnd();

        client.ReadPosition = stream.Position;
        client.LogWriteTime = File.GetLastWriteTime(client.LogFile);

        return data;
    }

    private static void ProcessNewData(RobloxClient client, string newData, Form1 form)
    {
        string followup = "\"largeImage\":{\"hoverText\":\"";
        string data = client.Buffer + newData;

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
        string privateServer = form.GetPrivateServerForUser(client.UserId);

        if (lastBiomeByUser.ContainsKey(client.UserId))
        {
            if (lastBiomeByUser[client.UserId] == biome)
                return;
        }

        lastBiomeByUser[client.UserId] = biome;

        form.PrintLogs($"New Biome Found: {biome} | User ID: {client.UserId}");

        if (string.IsNullOrWhiteSpace(privateServer))
            form.PrintLogs($"No private server configured for User ID: {client.UserId}");

        form.FoundNewBiome(biome, privateServer, client.UserId);
    }

    private static string ReadFullLog(string file, out long position)
    {
        using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true);

        string text = reader.ReadToEnd();

        position = stream.Position;

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

        if (lastAuraByUser.TryGetValue(client.UserId, out string lastAura))
        {
            if (lastAura == aura)
                return;
        }

        lastAuraByUser[client.UserId] = aura;

        form.FoundNewAura(aura, client.UserId);
    }
}