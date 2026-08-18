using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Sol_s_RNG_Biome_Detector
{
    public partial class Form1 : Form
    {
        private static int totalRare = 0;
        private static int totalBiomes = 0;

        private static int SessionRare = 0;
        private static int SessionBiomes = 0;

        public static bool Start_Stop = false;

        private bool loadingSettings = false;

        private readonly Stopwatch sessionTimer = new Stopwatch();
        public static readonly Stopwatch untilAFK = new Stopwatch();
        public const uint AFKDelay = 900000; // 15 mins



        private readonly CheckBox[] biomeCheckboxes;

        private readonly Label[] biomeStats;

        private readonly Button[] sidebarButtons;

        private GUI gui = new GUI();

        private Webhook webhooks = new Webhook();

        private PrivateServer privateServers = new PrivateServer();


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_F1 = 1;
        private const int HOTKEY_F2 = 2;
        private const int HOTKEY_F3 = 3;

        private const int WM_HOTKEY = 0x0312;

        public Form1()
        {
            InitializeComponent();

            biomeCheckboxes =
            [
                checkBox1,
                checkBox2,
                checkBox3,
                checkBox4,
                checkBox5,
                checkBox6,
                checkBox7,
                checkBox8,
                checkBox9,
                checkBox10,
                checkBox11,
                checkBox12,
                checkBox13,
                checkBox14,
                checkBox15,
                checkBox16,
                checkBox17,
                checkBox18,
                checkBox19,
                checkBox20
            ];

            sidebarButtons =
            [
                button2,
                button4,
                button5,
                button6,
                button7,
                button8,
                button36
            ];

            biomeStats =
            [
                label51,
                label52,
                label53,
                label54,
                label55,
                label56,
                label57,
                label58,
                label59,
                label60, // Normal - Rainy
                label62,
                label63,
                label64,
                label65,
                label66,
                label67, // Blazing Sun - Aurora
                label43,
                label44,
                label45,
                label46, // Singularity - Glitched
            ];

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadingSettings = true;

            Settings.Load();
            LoadSettings();
            webhooks.Load(listBox1);
            privateServers.Load(listBox2);
            LoadStats();

            loadingSettings = false;


            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.WordWrap = false;
            textBox1.Dock = DockStyle.Fill;
            button1.BringToFront();

            gui.ApplyStyle(this, tabControl, biomeCheckboxes, biomeStats, panelSidebar, panelContent);
            gui.StyleSidebar(panelSidebar, sidebarButtons);

            RegisterHotKey(Handle, HOTKEY_F1, 0, (uint)Keys.F1);
            RegisterHotKey(Handle, HOTKEY_F2, 0, (uint)Keys.F2);
            RegisterHotKey(Handle, HOTKEY_F3, 0, (uint)Keys.F3);

            panelStatus.Size = new Size(14, 14);
            panelStatus.BackColor = Color.Red;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, HOTKEY_F1);
            UnregisterHotKey(Handle, HOTKEY_F2);
            UnregisterHotKey(Handle, HOTKEY_F3);

            base.OnFormClosing(e);
        }

        public void PrintLogs(string log)
        {
            textBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {log}{Environment.NewLine}");
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        public void FoundNewBiome(string biome, string privateserverlink, string userid)
        {
            foreach (CheckBox box in biomeCheckboxes)
            {
                if (box.Text.ToUpper() == biome.ToUpper())
                {
                    if (!box.Checked)
                        return;

                    if (webhooks.Webhooks.Count > 0 && !string.IsNullOrWhiteSpace(privateserverlink))
                    {
                        int color = GetBiomeStuff(biome.ToUpper(), checkBox22.Checked, checkBox21.Checked, out bool ping);

                        if (checkBox23.Checked)
                        {
                            if (textBox3.Text != string.Empty)
                            {
                                PrintLogs("Sent Webhook!");
                                webhooks.PostToWebhooks(webhooks.Webhooks, biome, $"<@&{textBox3.Text}>", ping, privateserverlink, color);
                            }
                        }
                        if (checkBox24.Checked)
                        {
                            if (textBox4.Text != string.Empty)
                            {
                                PrintLogs("Sent Webhook!");
                                webhooks.PostToWebhooks(webhooks.Webhooks, biome, $"<@{textBox4.Text}>", ping, privateserverlink, color);
                            }
                        }
                        if (checkBox25.Checked)
                        {
                            PrintLogs("Sent Webhook!");
                            webhooks.PostToWebhooks(webhooks.Webhooks, biome, "@everyone", ping, privateserverlink, color);

                        }
                        if (checkBox26.Checked)
                        {
                            PrintLogs("Sent Webhook!");
                            webhooks.PostToWebhooks(webhooks.Webhooks, biome, string.Empty, ping, privateserverlink, color);

                        }
                    }
                }
            }
            return;
        }

        private async void Start(bool start)
        {
            if (!Start_Stop && start)
            {
                Start_Stop = true;
                sessionTimer.Restart();
                untilAFK.Restart();

                SessionBiomes = 0;
                PrintLogs("Started");

                if (webhooks.Webhooks.Count > 0)
                {
                    try
                    {
                        await webhooks.StartStopWebhooks(webhooks.Webhooks, true, TimeSpan.Zero, SessionBiomes, SessionRare);
                    }
                    catch (Exception ex)
                    {
                        PrintLogs("Webhook Error: " + ex.Message);
                    }
                }
                Task Biomes = BiomeDetector.Biomes(this);
                UpdatePanelStatus(true);
                return;
            }

            if (Start_Stop && !start)
            {
                Start_Stop = false;
                sessionTimer.Stop();

                TimeSpan sessionTime = sessionTimer.Elapsed;

                PrintLogs($"Stopped - Session Time: {FormatSessionTime(sessionTime)}");

                if (webhooks.Webhooks.Count > 0)
                {
                    try
                    {
                        await webhooks.StartStopWebhooks(webhooks.Webhooks, false, sessionTime, SessionBiomes, SessionRare);
                    }
                    catch (Exception ex)
                    {
                        PrintLogs("Webhook Error: " + ex.Message);
                    }
                }
                UpdatePanelStatus(false);
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private void PingcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox selected = (CheckBox)sender;

            if (!selected.Checked)
            {
                return;
            }

            if (selected != checkBox23)
                checkBox23.Checked = false;

            if (selected != checkBox24)
                checkBox24.Checked = false;

            if (selected != checkBox25)
                checkBox25.Checked = false;

            if (selected != checkBox26)
                checkBox26.Checked = false;

            if (!loadingSettings)
                SaveSettings();

        }

        private void UpdateStats(bool rarebiome)
        {
            if (rarebiome)
            {
                totalRare++;

                SessionRare++;
            }

            totalBiomes++;
            SessionBiomes++;

            Settings.Data.TotalBiomes = totalBiomes;
            Settings.Data.TotalRareBiomes = totalRare;

            Settings.Save();
            LoadStats();
        }

        private void LoadStats()
        {
            label9.Text = totalRare.ToString();
            label14.Text = SessionRare.ToString();
            label11.Text = totalBiomes.ToString();
            label15.Text = SessionBiomes.ToString();

            label68.Text = Settings.Data.TotalNormal.ToString(); // Normal -> Null
            label69.Text = Settings.Data.TotalWindy.ToString();
            label70.Text = Settings.Data.TotalSnowy.ToString();
            label71.Text = Settings.Data.TotalRainy.ToString();
            label72.Text = Settings.Data.TotalSandStorm.ToString();
            label73.Text = Settings.Data.TotalHell.ToString();
            label74.Text = Settings.Data.TotalStarfall.ToString();
            label75.Text = Settings.Data.TotalHeaven.ToString();
            label76.Text = Settings.Data.TotalCorruption.ToString();
            label77.Text = Settings.Data.TotalNull.ToString();

            label47.Text = Settings.Data.TotalSingularity.ToString(); // Singularity -> Glitched
            label48.Text = Settings.Data.TotalCyberspace.ToString();
            label49.Text = Settings.Data.TotalDreamspace.ToString();
            label50.Text = Settings.Data.TotalGlitched.ToString();

            label78.Text = Settings.Data.TotalBlazingSun.ToString(); // Blazing Sun -> Aurora
            label79.Text = Settings.Data.TotalEggland.ToString();
            label80.Text = Settings.Data.TotalPumpkinMoon.ToString();
            label81.Text = Settings.Data.TotalBloodRain.ToString();
            label82.Text = Settings.Data.TotalGraveyard.ToString();
            label83.Text = Settings.Data.TotalAurora.ToString();

        }

        private int GetBiomeStuff(string Biome, bool onlyrareping, bool treatsingasrare, out bool ping)
        {
            if (onlyrareping)
                ping = false;
            else
                ping = true;


            switch (Biome)
            {
                case "NORMAL":
                    {
                        Settings.Data.TotalNormal++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x4e4e4e;
                    }


                case "WINDY":
                    {
                        
                        Settings.Data.TotalWindy++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0xc2f2ff;
                    }

                case "SNOWY":
                    {
                        Settings.Data.TotalSnowy++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0xb6cbd1;
                    }

                case "RAINY":
                    {
                        
                        Settings.Data.TotalRainy++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x0000ff;
                    }

                case "SAND STORM":
                    {
                        Settings.Data.TotalSandStorm++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0xffbb00;
                    }

                case "HELL":
                    {
                        Settings.Data.TotalHell++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x770a0a;
                    }

                case "STARFALL":
                    {
                       
                        Settings.Data.TotalStarfall++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x3b3abc;
                    }

                case "HEAVEN":
                    {
                     
                        Settings.Data.TotalHeaven++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0xf4fb01;
                    }

                case "CORRUPTION":
                    {
                        
                        Settings.Data.TotalCorruption++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x310387;
                    }

                case "NULL":
                    {
                        
                        Settings.Data.TotalNull++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x000000;
                    }

                case "SINGULARITY":
                    {
                        Settings.Data.TotalSingularity++;
                        Settings.Save();
                        UpdateStats(true);

                        if (treatsingasrare)
                            ping = true;

                        return 0xbf6c00;
                    }

                case "CYBERSPACE":
                    {
                        
                        Settings.Data.TotalCyberspace++;
                        Settings.Save();
                        UpdateStats(true);
                        ping = true;
                        return 0x08043f;
                    }

                case "DREAMSPACE":
                    {
                        
                        Settings.Data.TotalDreamspace++;
                        Settings.Save();
                        UpdateStats(true);
                        ping = true;
                        return 0xe500ff;
                    }

                case "GLITCHED":
                    {
                        
                        Settings.Data.TotalGlitched++;
                        Settings.Save();
                        UpdateStats(true);
                        ping = true;
                        return 0x212121;
                    }

                case "BLAZING SUN":
                    {
                        
                        Settings.Data.TotalBlazingSun++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0xfaff00;
                    }

                case "EGGLAND":
                    {
                        
                        Settings.Data.TotalEggland++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x9fff9a;
                    }

                case "PUMPKIN MOON":
                    {
                        
                        Settings.Data.TotalPumpkinMoon++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x996505;
                    }

                case "BLOOD RAIN":
                    {
                        
                        Settings.Data.TotalBloodRain++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x3e0000;
                    }

                case "GRAVEYARD":
                    {
                        
                        Settings.Data.TotalGraveyard++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0xc1ecff;
                    }

                case "AURORA":
                    {
                        
                        Settings.Data.TotalAurora++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0x8d7dc7;
                    }

                default:
                    return 0xFFFFFF;
            }
        }

        private void SaveSettings()
        {
            Settings.Data.Normal = checkBox1.Checked;
            Settings.Data.Windy = checkBox2.Checked;
            Settings.Data.Snowy = checkBox3.Checked;
            Settings.Data.Rainy = checkBox4.Checked;
            Settings.Data.SandStorm = checkBox5.Checked;
            Settings.Data.Hell = checkBox6.Checked;
            Settings.Data.Starfall = checkBox7.Checked;
            Settings.Data.Heaven = checkBox8.Checked;
            Settings.Data.Corruption = checkBox9.Checked;
            Settings.Data.Null = checkBox10.Checked;
            Settings.Data.Singularity = checkBox11.Checked;
            Settings.Data.Cyberspace = checkBox12.Checked;
            Settings.Data.Dreamspace = checkBox13.Checked;
            Settings.Data.Glitched = checkBox14.Checked;
            Settings.Data.BlazingSun = checkBox15.Checked;
            Settings.Data.Eggland = checkBox16.Checked;
            Settings.Data.PumpkinMoon = checkBox17.Checked;
            Settings.Data.BloodRain = checkBox18.Checked;
            Settings.Data.Graveyard = checkBox19.Checked;
            Settings.Data.Aurora = checkBox20.Checked;

            Settings.Data.TreatSingularityAsRare = checkBox21.Checked;
            Settings.Data.OnlyPingForRare = checkBox22.Checked;

            Settings.Data.PingRole = checkBox23.Checked;
            Settings.Data.PingUserID = checkBox24.Checked;
            Settings.Data.PingEveryone = checkBox25.Checked;
            Settings.Data.DontPing = checkBox26.Checked;

            Settings.Data.PingRoleID = textBox3.Text;
            Settings.Data.PingUserIDValue = textBox4.Text;



            Settings.Save();
        }

        private void SettingChanged(object sender, EventArgs e)
        {
            if (loadingSettings)
                return;

            SaveSettings();
        }

        private void LoadSettings()
        {
            checkBox1.Checked = Settings.Data.Normal;
            checkBox2.Checked = Settings.Data.Windy;
            checkBox3.Checked = Settings.Data.Snowy;
            checkBox4.Checked = Settings.Data.Rainy;
            checkBox5.Checked = Settings.Data.SandStorm;
            checkBox6.Checked = Settings.Data.Hell;
            checkBox7.Checked = Settings.Data.Starfall;
            checkBox8.Checked = Settings.Data.Heaven;
            checkBox9.Checked = Settings.Data.Corruption;
            checkBox10.Checked = Settings.Data.Null;
            checkBox11.Checked = Settings.Data.Singularity;
            checkBox12.Checked = Settings.Data.Cyberspace;
            checkBox13.Checked = Settings.Data.Dreamspace;
            checkBox14.Checked = Settings.Data.Glitched;
            checkBox15.Checked = Settings.Data.BlazingSun;
            checkBox16.Checked = Settings.Data.Eggland;
            checkBox17.Checked = Settings.Data.PumpkinMoon;
            checkBox18.Checked = Settings.Data.BloodRain;
            checkBox19.Checked = Settings.Data.Graveyard;
            checkBox20.Checked = Settings.Data.Aurora;

            checkBox21.Checked = Settings.Data.TreatSingularityAsRare;
            checkBox22.Checked = Settings.Data.OnlyPingForRare;

            checkBox23.Checked = Settings.Data.PingRole;
            checkBox24.Checked = Settings.Data.PingUserID;
            checkBox25.Checked = Settings.Data.PingEveryone;
            checkBox26.Checked = Settings.Data.DontPing;

            textBox3.Text = Settings.Data.PingRoleID;
            textBox4.Text = Settings.Data.PingUserIDValue;

            totalBiomes = Settings.Data.TotalBiomes;
            totalRare = Settings.Data.TotalRareBiomes;

            label11.Text = totalBiomes.ToString();
            label9.Text = totalRare.ToString();
        }

        private string FormatSessionTime(TimeSpan time)
        {
            return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }



        private void activebutton(object sender, EventArgs e)
        {
            if (sender is Button selectedButton)
            {
                gui.SetActiveButton(selectedButton, sidebarButtons);

                if (selectedButton == button2)
                    tabControl.SelectedTab = tabPage1;

                else if (selectedButton == button4)
                    tabControl.SelectedTab = tabPage2;

                else if (selectedButton == button5)
                    tabControl.SelectedTab = tabPage3;

                else if (selectedButton == button6)
                    tabControl.SelectedTab = tabPage4;

                else if (selectedButton == button7)
                    tabControl.SelectedTab = tabPage5;

                else if (selectedButton == button8)
                    tabControl.SelectedTab = tabPage6;

                else if (selectedButton == button36)
                    tabControl.SelectedTab = tabPage13;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            webhooks.EditingIndex = -1;
            textBox2.Text = "";
            tabControl.SelectedTab = tabPage7;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            webhooks.RefreshList(listBox1);
            tabControl.SelectedTab = tabPage8;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string webhook = textBox2.Text.Trim();

            if (webhooks.EditingIndex == -1)
            {
                if (webhooks.Contains(webhook))
                {
                    MessageBox.Show("This webhook is already added.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                webhooks.Add(webhook);
            }
            else
            {
                webhooks.Update(webhooks.EditingIndex, webhook);
                webhooks.EditingIndex = -1;
            }

            webhooks.RefreshList(listBox1);

            textBox2.Text = "";

            tabControl.SelectedTab = tabPage8;
        }


        private void button14_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage3;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage3;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a webhook first.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Delete this webhook?", "Bloom", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            webhooks.Remove(listBox1.SelectedIndex);
            webhooks.RefreshList(listBox1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a webhook first.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            webhooks.EditingIndex = listBox1.SelectedIndex;

            textBox2.Text = webhooks.Get(webhooks.EditingIndex);
            button11.Text = "Save Changes";

            tabControl.SelectedTab = tabPage7;
        }


        private void button16_Click(object sender, EventArgs e)
        {
            privateServers.EditingIndex = -1;

            textBox5.Clear();
            textBox6.Clear();

            tabControl.SelectedTab = tabPage9;
        }


        private void button17_Click(object sender, EventArgs e)
        {
            privateServers.RefreshList(listBox2);
            tabControl.SelectedTab = tabPage10;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            privateServers.EditingIndex = -1;

            textBox5.Clear();
            textBox6.Clear();

            tabControl.SelectedTab = tabPage3;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage3;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage11;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage9;
        }



        private void button19_Click(object sender, EventArgs e)
        {
            string link = textBox5.Text.Trim();
            string userId = textBox6.Text.Trim();

            if (!privateServers.IsValidLink(link))
            {
                MessageBox.Show("Please enter a valid Roblox private server link.", "Invalid Private Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!privateServers.IsValidUserId(userId))
            {
                MessageBox.Show("Please enter a valid Roblox User ID.", "Invalid User ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int existingIndex = privateServers.FindUser(userId);

            if (existingIndex != -1 && existingIndex != privateServers.EditingIndex)
            {
                MessageBox.Show("This Roblox User ID already has a private server configured.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (privateServers.EditingIndex == -1)
            {
                privateServers.Add(userId, link);
            }
            else
            {
                privateServers.Update(privateServers.EditingIndex, userId, link);
                privateServers.EditingIndex = -1;
            }

            privateServers.RefreshList(listBox2);

            textBox5.Clear();
            textBox6.Clear();

            tabControl.SelectedTab = tabPage10;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select a private server first.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            privateServers.EditingIndex = listBox2.SelectedIndex;

            PrivateServer.Entry entry = privateServers.Get(privateServers.EditingIndex);

            textBox5.Text = entry.Link;
            textBox6.Text = entry.UserId;

            button19.Text = "Save Changes";

            tabControl.SelectedTab = tabPage9;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select a private server first.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrivateServer.Entry entry = privateServers.Get(listBox2.SelectedIndex);

            DialogResult result = MessageBox.Show($"Delete the private server for User ID {entry.UserId}?", "Bloom", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            privateServers.Remove(listBox2.SelectedIndex);
            privateServers.RefreshList(listBox2);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public string GetPrivateServerForUser(string userId)
        {
            return privateServers.GetForUser(userId);
        }

        private void checkBox28_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox28.Checked)
            {
                DialogResult dresult = MessageBox.Show("Enabling this modifies handles of the Roblox client and may result in a ban (Use at your own risk)!", "Bloom", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (dresult == DialogResult.OK)
                {

                    string path = RobloxHandle.FindHandleExe();

                    if (string.IsNullOrWhiteSpace(path))
                    {
                        tabControl.SelectedTab = tabPage12;
                        checkBox28.Checked = false;
                    }
                    else
                    {

                        bool admin = AskForAdmin();

                        if (!admin)
                        {
                            MessageBox.Show("This program requires admin perms in order to interact with the Roblox Handle", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            checkBox28.Checked = false;
                            return;
                        }

                        bool result = RobloxHandle.CloseSingletonEvent(path);

                        if (!result)
                        {
                            checkBox28.Checked = false;
                            PrintLogs("Couldn't close the Handle!");
                            MessageBox.Show(RobloxHandle.LastError, "Failed to close Roblox Handle", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                if (dresult == DialogResult.Cancel)
                {
                    checkBox28.Checked = false;
                    return;
                }
            }
        }

        private static bool AskForAdmin()
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = Environment.ProcessPath;
            info.Verb = "runas";
            info.UseShellExecute = true;

            try
            {
                Process.Start(info);
                return true;
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223)
                    return false;

                throw;
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage1;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://learn.microsoft.com/en-us/sysinternals/downloads/handle") { UseShellExecute = true });
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://download.sysinternals.com/files/Handle.zip") { UseShellExecute = true });
        }

        private async void checkBox29_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox29.Checked)
                return;

            while (checkBox29.Checked)
            {
                if (Start_Stop && untilAFK.ElapsedMilliseconds > AFKDelay)
                {
                    if (!checkBox29.Checked || !Start_Stop)
                        continue;

                    AntiAFK.SendActivity();
                    untilAFK.Restart();
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        private async void GetUsedItems(object sender, EventArgs e)
        {

        }

        private void button34_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage13;
        }


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();

                if (id == HOTKEY_F1)
                {
                    Start(true);
                }

                if (id == HOTKEY_F2)
                {
                    Start(false);
                }

                if (id == HOTKEY_F3)
                {
                    Environment.Exit(0);
                }
            }

            base.WndProc(ref m);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a webhook first.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            webhooks.TestWebhook(webhooks.Get(listBox1.SelectedIndex));
        }

        private void UpdatePanelStatus(bool running)
        {
            panelStatus.BackColor = running ? Color.Green : Color.Red;
        }
    }
}
