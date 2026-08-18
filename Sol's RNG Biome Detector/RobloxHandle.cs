using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Sol_s_RNG_Biome_Detector
{
    internal class RobloxHandle
    {
        public static string LastError = "";

        public static string FindHandleExe()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "handle64.exe");

            if (File.Exists(path))
                return path;

            string[] files = Directory.GetFiles(AppContext.BaseDirectory, "handle64.exe", SearchOption.AllDirectories);

            if (files.Length > 0)
                return files[0];

            return "";
        }

        private static IntPtr GetSingletonEventHandle(uint pid, string handleExePath)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = handleExePath;
            info.Arguments = $"-accepteula -a -p {pid}";
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            using Process process = Process.Start(info);

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                LastError = output + Environment.NewLine + error;
                return IntPtr.Zero;
            }

            Match match = Regex.Match(output, @"([0-9A-Fa-f]+):\s+Event\s+.*ROBLOX_singletonEvent", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                LastError = "ROBLOX_singletonEvent was not found." + Environment.NewLine + Environment.NewLine + output;
                return IntPtr.Zero;
            }

            long handle = Convert.ToInt64(match.Groups[1].Value, 16);

            return new IntPtr(handle);
        }

        public static bool CloseSingletonEvent(string handleExePath)
        {
            LastError = "";

            uint pid = GetProcessID();

            if (pid == 0)
            {
                LastError = "RobloxPlayerBeta was not found.";
                return false;
            }

            if (handleExePath == "")
            {
                LastError = "handle64.exe was not found.";
                return false;
            }

            IntPtr handle = GetSingletonEventHandle(pid, handleExePath);

            if (handle == IntPtr.Zero)
                return false;

            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = handleExePath;
            info.Arguments = $"-accepteula -c {handle.ToInt64():X} -p {pid} -y";
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            using Process process = Process.Start(info);

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                LastError = output + Environment.NewLine + error;
                return false;
            }

            return true;
        }

        private static uint GetProcessID()
        {
            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            if (processes.Length == 0)
                return 0;

            return (uint)processes[0].Id;
        }
    }
}