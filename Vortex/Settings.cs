using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Drawing;

namespace Vortex
{
    internal class Settings
    {
        public static SettingsData Data = new SettingsData();

        private static readonly string _Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vortex");
        private static readonly string _File = Path.Combine(_Folder, "settings.json");

        private static readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        private static readonly object @lock = new object(); // yes 

        public static void Save()
        {
            lock (@lock)
            {
                Directory.CreateDirectory(_Folder);

                string json = JsonSerializer.Serialize(Data, options);

                string tempfile = _File + "." + Guid.NewGuid().ToString() + ".tmp";

                try
                {
                    File.WriteAllText(tempfile, json);

                    if (File.Exists(_File))
                        File.Replace(tempfile, _File, _File + ".data");
                    else
                        File.Move(tempfile, _File);

                    Form1._PrintLogs("Saved your Settings!", Color.GhostWhite);
                }
                catch (Exception ex)
                {
                    Form1._PrintLogs($"Encountered an error while trying to Save your Settings! {ex.Message}", Color.DarkRed);
                }
                finally
                {
                    if (File.Exists(tempfile))
                        File.Delete(tempfile);
                }
            }
        }

        /* OLD
         * 
            Directory.CreateDirectory(_Folder);

            string json = JsonSerializer.Serialize(Data, options);

            File.WriteAllText(_File, json);
        */

        public static void Load()
        {
            string backup = _File + ".data";

            try
            {
                Directory.CreateDirectory(_Folder);

                if (!File.Exists(_File) && !File.Exists(backup))
                {
                    Save();
                    return;
                }

                string json = File.ReadAllText(_File);

                var loaded = JsonSerializer.Deserialize<SettingsData>(json);

                if (loaded != null)
                {
                    Data = loaded;
                    Form1._PrintLogs("Loaded Settings", Color.White);
                }
                    
            }
            catch (Exception ex)
            {
                Form1._PrintLogs($"Failed to Load Settings!, Retrying the Backup File.{ex.Message}", Color.DarkRed);

                try
                {

                    Directory.CreateDirectory(_Folder);

                    if (!File.Exists(backup)) // not even needed but ig it doesnt matter
                    {
                        Save();
                        return;
                    }

                    string json = File.ReadAllText(backup);

                    var loaded = JsonSerializer.Deserialize<SettingsData>(json);

                    if (loaded != null)
                    {
                        Data = loaded;
                        File.Copy(backup, _File, true);
                        Form1._PrintLogs("Loaded your settings and removed the corrupted save file.", Color.White);
                    }

                }
                catch (Exception ex2)
                {
                    Form1._PrintLogs($"Really couldn't load your settings, Sorry for the inconvenience. {ex2.Message}", Color.DarkRed);
                }
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
            public bool LostForest { get; set; } = true;
            public bool RadiantForest { get; set; } = true;

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
            public List<Accounts.Entry> Accounts { get; set; } = new List<Accounts.Entry>();

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
            public int TotalLostForest { get; set; } = 0;
            public int TotalRadiantForest { get; set; } = 0;

            public bool IncludeUsername { get; set; } = false;

            public bool AuraNotifications { get; set; } = false;
            public bool OnlyPingForGlobals { get; set; } = false;
            public bool AuraPingUserID { get; set; } = false;
            public long MinAuraRarity { get; set; } = 0;
            public long AuraUserID { get; set; } = 0;

            public int TotalGlobalsRolled { get; set; } = 0;

            public int FontIndex { get; set; } = 0;
            public int ThemeIndex { get; set; } = 0;

        }
    }
}
