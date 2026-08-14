using Sol_s_RNG_Biome_Detector;
using System;
using System.Text;
using System.Threading.Tasks;

class BiomeDetector
{

    public static string lastValidBiome = "";

    public static async Task Biomes(Form1 form)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "logs");
        string followup = "\"largeImage\":{\"hoverText\":\"";

        string activeFile = string.Empty;
        DateTime lastLogScan = DateTime.MinValue;

        form.PrintLogs("Detector Started.");
        form.PrintLogs("Searching for active logs file.");

        while (Form1.Start_Stop)
        {
            try
            {
                if ((DateTime.Now - lastLogScan).TotalSeconds >= 3)
                {
                    lastLogScan = DateTime.Now;

                    string newActiveFile = FindActiveLog(path);

                    if (!string.IsNullOrEmpty(newActiveFile) && newActiveFile != activeFile)
                    {
                        activeFile = newActiveFile;
                        form.PrintLogs("Switched active log: " + Path.GetFileName(activeFile));
                    }
                }

                if (string.IsNullOrEmpty(activeFile))
                {
                    await Task.Delay(500);
                    continue;
                }

                string log = ReadLog(activeFile);

                int start = log.LastIndexOf(followup);

                if (start != -1)
                {
                    start += followup.Length;

                    int end = log.IndexOf("\"", start);

                    if (end != -1)
                    {
                        string biome = log.Substring(start, end - start);
                        ValidateBiome(biome, form);
                    }
                }
            }
            catch (Exception e)
            {
                form.PrintLogs("Detector Error: " + e.Message);
                activeFile = string.Empty;
                await Task.Delay(1000);
            }

            await Task.Delay(200);
        }
    }

    private static void ValidateBiome(string LastBiome, Form1 form)
    {
        if (lastValidBiome != LastBiome)
        {
            lastValidBiome = LastBiome;
            form.PrintLogs("New Biome Found: " + lastValidBiome);
            form.FoundNewBiome(lastValidBiome);
        }
    }

    private static string ReadLog(string file)
    {
        using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

        using StreamReader read = new StreamReader(stream);

        return read.ReadToEnd();
    }

    private static string FindActiveLog(string path)
    {
        string[] files = Directory.GetFiles(path, "*.log").OrderByDescending(File.GetLastWriteTime).Take(10).ToArray();

        foreach (string file in files)
        {
            try
            {
                string text = ReadLog(file);

                if (text.Contains("SetRichPresence"))
                    return file;
            }
            catch
            {
            }
        }
        return string.Empty;
    }
}

