using System;
using System.Diagnostics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sol_s_RNG_Biome_Detector
{
    internal class UpdateSelf // Written at 4:10 am
    {
        public async Task<(bool updateavailable, string latesturl, string latestversion, string sha)> CheckForUpdates(string currentver)
        { 
            using HttpClient client = new HttpClient();

            string url = "https://raw.githubusercontent.com/vexk1x/Vortex/main/Updater/latest.json";
            string json;

            try
            {
                json = await client.GetStringAsync(url);
            }
            catch
            {
                return (false, "", "", "");
            }

            using JsonDocument document = JsonDocument.Parse(json);

            string? latesturl = document.RootElement.GetProperty("latest").GetString();
            string? latestversion = document.RootElement.GetProperty("version").GetString();
            string? sha = document.RootElement.GetProperty("sha256").GetString();

            if (string.IsNullOrEmpty(latesturl) || string.IsNullOrEmpty(latestversion)|| string.IsNullOrEmpty(sha))
                return (false, "", "", "");

            int icurrentver = int.Parse(currentver.Replace(".", ""));
            int ilatestver = int.Parse(latestversion.Replace(".", ""));

            if (icurrentver >= ilatestver) // bad check since versions like 1.0.0.1 would be considered newer than 1.0.1 (doesnt matter since i use x.x.x)
                return (false, "", "", "");

            return (true, latesturl, latestversion, sha);
        }

        public async Task<(bool success, string path)> DownloadUpdate(string latesturl, string sha)
        {
            using HttpClient client = new HttpClient();

            string dversions = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vortex", "Versions");


            string vortexfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vortex", "Versions");

            string latestverfolder = "Vortex " + " - " + sha;

            string path = Path.Combine(vortexfolder, latestverfolder);


            try
            {
                if (!Directory.Exists(dversions))
                {
                    Directory.CreateDirectory(dversions);
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                
                if (!File.Exists(Path.Combine(path, "Vortex.exe")))
                {
                    using Stream stream = await client.GetStreamAsync(latesturl);

                    using FileStream fstream = File.Create(Path.Combine(path, "Vortex.exe"));

                    await stream.CopyToAsync(fstream);
                }
            }
            catch
            {
                return (false, path);
            }

            return (true, path);
        }

        public async Task WipeOldDownloads()
        {
            string allfolders = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vortex", "Versions");

			if (!Directory.Exists(allfolders))
				return;

            foreach (string folder in Directory.GetDirectories(allfolders))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch
                {
                }
            }
        }
        
        public async Task RunUpdate(string path)
        {
			try 
			{
            	string exepath = Path.Combine(path, "Vortex.exe");

            	Process.Start(new ProcessStartInfo { FileName = exepath, UseShellExecute = true});
			}

			catch {return;}
        }
    }
}
