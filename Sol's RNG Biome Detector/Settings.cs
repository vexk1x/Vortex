using System;
using System.Text.Json;
using System.IO;
using System.Windows.Forms.Design;

namespace Sol_s_RNG_Biome_Detector
{
    class Settings
    {
        public static SettingsData Data = new SettingsData();

        private static readonly string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vortex");
        private static readonly string file = Path.Combine(folder, "settings.json");

        private static readonly string oldfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bloom");
        private static readonly string oldfile = Path.Combine(oldfolder, "settings.json");


        public static void Rename()
        {
            try
            {
                if (!Directory.Exists(folder) && Directory.Exists(oldfolder))
                {
                    Directory.Move(oldfolder, folder);
                }
                if (!File.Exists(file) && File.Exists(oldfile))
                {
                    File.Move(oldfile, file);
                }
            }
            catch
            {
            }
        }

        public static void Save()
        {
            Directory.CreateDirectory(folder);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            string json = JsonSerializer.Serialize(Data, options);

            File.WriteAllText(file, json);
        }

        public static string GetPath()
        {
            return file;
        }
        
        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(folder);

                if (!File.Exists(file))
                {
                    Save();
                    return;
                }

                string json = File.ReadAllText(file);

                var loaded = JsonSerializer.Deserialize<SettingsData>(json);

                if (loaded != null)
                    Data = loaded;
            }
            catch
            {
                Data = new SettingsData();
            }
        }

        internal class SettingsData
        {
            public bool Normal { get; set; } = true;
            public bool Windy { get; set; } = true;
            public bool Snowy { get; set; } = true;
            public bool Rainy { get; set; } = true;
            public bool SandStorm { get; set; } = true;
            public bool Hell { get; set; } = true;
            public bool Starfall { get; set; } = true;
            public bool Heaven { get; set; } = true;
            public bool Corruption { get; set; } = true;
            public bool Null { get; set; } = true;
            public bool Singularity { get; set; } = true;
            public bool Cyberspace { get; set; } = true;
            public bool Dreamspace { get; set; } = true;
            public bool Glitched { get; set; } = true;
            public bool BlazingSun { get; set; } = true;
            public bool Incinerator { get; set; } = true;

            public bool TreatSingularityAsRare { get; set; } = false;
            public bool OnlyPingForRare { get; set; } = false;

            public bool PingRole { get; set; } = false;
            public bool PingUserID { get; set; } = false;
            public bool PingEveryone { get; set; } = false;
            public bool DontPing { get; set; } = true;

            public string PingRoleID { get; set; } = "";
            public string PingUserIDValue { get; set; } = "";

            public int TotalBiomes { get; set; } = 0;
            public int TotalRareBiomes { get; set; } = 0;

            public List<string> Webhooks { get; set; } = new List<string>();
            public List<PrivateServer.Entry> PrivateServers { get; set; } = new List<PrivateServer.Entry>();

            public int TotalNormal { get; set; } = 0;
            public int TotalWindy { get; set; } = 0;
            public int TotalSnowy { get; set; } = 0;
            public int TotalRainy { get; set; } = 0;
            public int TotalSandStorm { get; set; } = 0;
            public int TotalHell { get; set; } = 0;
            public int TotalStarfall { get; set; } = 0;
            public int TotalHeaven { get; set; } = 0;
            public int TotalCorruption { get; set; } = 0;
            public int TotalNull { get; set; } = 0;
            public int TotalSingularity { get; set; } = 0;
            public int TotalCyberspace { get; set; } = 0;
            public int TotalDreamspace { get; set; } = 0;
            public int TotalGlitched { get; set; } = 0;
            public int TotalBlazingSun { get; set; } = 0;
            public int TotalIncinerator { get; set; } = 0;

            public bool IncludeUsername { get; set; } = false;

            public bool AuraNotifications { get; set; } = false;
            public bool OnlyPingForGlobals { get; set; } = false;
            public bool AuraPingUserID { get; set; } = false;
            public string MinAuraRarity { get; set; } = "";
            public string AuraUserID { get; set; } = "";

            public int TotalGlobalsRolled {  get; set; } = 0;

        }
    }
}
