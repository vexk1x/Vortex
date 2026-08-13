using Sol_s_RNG_Biome_Detector;
using System;
using System.Text;
using System.Threading.Tasks;

class BiomeDetector
{

    public static string lastValidBiome = "";

    public static async Task Biomes(Form1 form)
    {
        string path = $@"C:\Users\{Environment.UserName}\AppData\Local\Roblox\logs";
        string keyword = "SetRichPresence";
        string followup = "\"largeImage\":{\"hoverText\":\"";
        string activeFile = string.Empty;
        int attempts = 0;

        form.PrintLogs("Detector Started.");
        form.PrintLogs("Searching for active logs file.");

        while (activeFile == string.Empty && attempts < 10)
        {
            string[] files = Directory.GetFiles(path, "*.log").OrderByDescending(File.GetLastWriteTime).Take(3).ToArray();


            foreach (string file in files)
            {
                try
                {
                    form.PrintLogs("Checking: " + Path.GetFileName(file));

                    string text = ReadLog(file);

                    if (text.Contains(keyword))
                    {
                        activeFile = file;
                        break;
                    }
                }
                catch (Exception e)
                {
                    form.PrintLogs("Err: " + e.Message);
                }
            }

            attempts++;
            await Task.Delay(350);
            
        }

        if (activeFile == string.Empty)
        {
            form.PrintLogs("Err: No active log File found.");
            return;
        }
        else
        {
            form.PrintLogs("Found active log File.");
        }

        while (Form1.Start_Stop)
        {
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
}

