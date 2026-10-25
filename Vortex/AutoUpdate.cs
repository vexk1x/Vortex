using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Vortex
{
    internal class AutoUpdate
    {
        private const string ENDPOINT = "https://api.github.com/repos/vexk1x/Vortex/releases/latest";
        private static Version CURRENT_VERSION = null;
        private const string RELEASE_NAME = "Vortex.exe";
        private static Uri faddr;
        private static string _digest;

        private static readonly HttpClient client = new HttpClient();

        static AutoUpdate()
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Vortex-Updater");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
        }

        public static void __SetVersion(Version @Version)
        {
            if (Version != null)
                CURRENT_VERSION = Version;
        }

        public static async Task<bool> UpdateAvailable()
        {
            string json = await client.GetStringAsync(ENDPOINT);

            JObject rel = JObject.Parse(json);

            string tag = (string)rel["tag_name"];

            if (string.IsNullOrWhiteSpace(tag))
                throw new Exception("Release not found");

            if (tag.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                tag = tag.Substring(1);
            else if (tag.StartsWith("v-", StringComparison.OrdinalIgnoreCase))
                tag = tag.Substring(2);

            Version latest;

            if (!Version.TryParse(tag, out latest))
                throw new Exception("Invalid Release version");

            if (CURRENT_VERSION == null)
                throw new Exception("Current Version is Invalid ???");

            if (latest <= CURRENT_VERSION)
                return false;

            JArray assets = (JArray)rel["assets"];

            JObject asset = null;

            foreach (JObject item in assets)
            {
                if (string.Equals((string)item["name"], RELEASE_NAME, StringComparison.OrdinalIgnoreCase))
                {
                    asset = item;
                    break;
                }
            }

            if (asset == null)
                throw new Exception("No Latest release found");

            string url = (string)asset["browser_download_url"];
            string digest = (string)asset["digest"];

            if (string.IsNullOrEmpty(digest) || !digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
                throw new Exception("release has no sha 256 digest");

            Uri addr;

            if (
                !Uri.TryCreate(url, UriKind.Absolute, out addr) || 
                addr.Scheme != Uri.UriSchemeHttps 
                || !string.Equals(addr.Host, "github.com", StringComparison.OrdinalIgnoreCase)
                )
                throw new Exception("Invalid Download url");

            if (addr != null && digest != null)
            {
                faddr = addr;
                _digest = digest;
            }
            else
            {
                throw new Exception();
            }

            return true;
        }

        public static async Task DownloadUpdate()
        {
            string @this = Application.ExecutablePath;

            string goal = Path.Combine(Path.GetDirectoryName(@this), "Vortex.Update." + Guid.NewGuid().ToString() + ".exe");

            using (HttpResponseMessage response = await client.GetAsync(faddr, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (Stream inp = await response.Content.ReadAsStreamAsync())
                
                using (FileStream outp = File.Create(goal))
                {
                    await inp.CopyToAsync(outp);
                }
            }

            var hashresult = VerifyHash(goal, _digest.Substring(7));

            if (!hashresult)
            {
                throw new Exception("hash mismatch");
            }

            RunUpdate(goal, @this);
        }


        private static bool VerifyHash(string path, string exp)
        {
            using (SHA256 sha = SHA256.Create())

            using (FileStream fstream = File.OpenRead(path))
            {
                byte[] hash = sha.ComputeHash(fstream);

                string thishash = BitConverter.ToString(hash).Replace("-", "");

                return string.Equals(thishash, exp, StringComparison.OrdinalIgnoreCase);
            }
        }

        private static void RunUpdate(string goal, string @this)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = goal,
                Arguments = "--update " + Process.GetCurrentProcess().Id + " \"" + @this + "\"",
                UseShellExecute = true
            });
        }

        public static void ApplyUpdate(int pid, string @this)
        {
            try
            {
                using (Process process = Process.GetProcessById(pid))
                {
                    if (!process.WaitForExit(30000))
                        throw new TimeoutException("Vortex did not close.");
                }
            }
            catch (ArgumentException)
            {
            }

            string replacement = @this + "." + Guid.NewGuid().ToString() + ".new";
            string backup = @this + ".bak";

            try
            {
                File.Copy(Application.ExecutablePath, replacement);
                File.Replace(replacement, @this, backup);

                Process.Start(new ProcessStartInfo()
                {
                    FileName = @this,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
