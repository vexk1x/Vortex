using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Sol_s_RNG_Biome_Detector
{
    public partial class Form1 : Form
    {
        private static int TotalRare = 0;
        private static int TotalBiomes = 0;

        private static int SessionRare = 0;
        private static int SessionBiomes = 0;

        private static int TotalGlobals = 0;
        private static int SessionGlobals = 0;

        public static bool Start_Stop = false;

        private bool LoadingSettings = false;

        private readonly Stopwatch SessionTimer = new Stopwatch();
        public static readonly Stopwatch UntilAFK = new Stopwatch();
        public const uint AFKDelay = 900000; // 15 mins

        private int AccountRefreshVersion = 0;

        private readonly CheckBox[] BiomeCheckboxes;
        private readonly Label[] BiomeStats;

        private readonly Button[] SidebarButtons;

        private readonly PictureBox[] TabBoxes;
        private readonly Label[] TabLabels;

        private GUI Gui = new GUI();

        private Webhook Webhooks = new Webhook();

        private PrivateServer PrivateServers = new PrivateServer();

        private MatchAura MatchAura = new MatchAura();

        private const int HOTKEY_F1 = 1;
        private const int HOTKEY_F2 = 2;
        private const int HOTKEY_F3 = 3;

        private const int WM_HOTKEY = 0x0312;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        public Form1()
        {
            InitializeComponent();

            this.Size = new Size(739, 492);
            FormBorderStyle = FormBorderStyle.None;
            panelContent.MouseDown += titleBar_MouseDown;

           
            foreach (TabPage page in tabControl.TabPages)
            {
                page.MouseDown += titleBar_MouseDown;
            }

            CheckBox CYBERSPACE = new CheckBox();
            CYBERSPACE.Checked = true;
            CYBERSPACE.Text = "CYBERSPACE";

            CheckBox DREAMSPACE = new CheckBox();
            DREAMSPACE.Checked = true;
            DREAMSPACE.Text = "DREAMSPACE";

            CheckBox GLITCHED = new CheckBox();
            GLITCHED.Checked = true;
            GLITCHED.Text = "GLITCHED";


            BiomeCheckboxes =
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
                checkBox15,
                CYBERSPACE,
                DREAMSPACE,
                GLITCHED
            ];

            SidebarButtons =
            [
                button2,
                button4,
                button27,
                button5,
                button6,
                button7,
                button8,
                button37
            ];

            BiomeStats =
            [
                label51, // Normal - Null
                label52,
                label53,
                label54,
                label55,
                label56,
                label57,
                label58,
                label59,
                label60,
                label62, // Blazing Sun (Current Event Biome)
                label43, // Singularity - Glitched
                label44,
                label45,
                label46
            ];

            TabBoxes =
            [
                pictureBox14,
                pictureBox15,
                pictureBox16,
                pictureBox17,
                pictureBox18,
                pictureBox19,
                pictureBox20,
                pictureBox21
            ];

            TabLabels =
            [
                label28,
                label40,
                label42,
                label99,
            ];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings.Rename();

            LoadingSettings = true;

            Settings.Load();
            LoadSettings();
            Webhooks.Load(listBox1);
            PrivateServers.Load(listBox2);
            LoadStats();

            LoadingSettings = false;


            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.WordWrap = false;
            textBox1.Dock = DockStyle.Fill;
            button1.BringToFront();

            textBox9.Multiline = true;
            textBox9.AutoSize = false;
            textBox9.Height = 250;
            textBox9.ReadOnly = true;
            textBox9.WordWrap = false;
            textBox9.ScrollBars = ScrollBars.Vertical;

            Gui.DrawGui(this, tabControl, BiomeCheckboxes, BiomeStats, panelSidebar, panelContent, SidebarButtons, TabBoxes, TabLabels);

            RegisterHotKey(Handle, HOTKEY_F1, 0, (uint)Keys.F1);
            RegisterHotKey(Handle, HOTKEY_F2, 0, (uint)Keys.F2);
            RegisterHotKey(Handle, HOTKEY_F3, 0, (uint)Keys.F3);

            UpdateRunningStatus(false, pictureBox13);

            label17.ForeColor = ColorTranslator.FromHtml("#0e00ff");
            label19.ForeColor = ColorTranslator.FromHtml("#ff00bd");
            label20.ForeColor = ColorTranslator.FromHtml("#2a2a2a");

            listBox1.BackColor = textBox1.BackColor;
            listBox2.BackColor = textBox1.BackColor;

            listBox1.ForeColor = Color.White;
            listBox2.ForeColor = Color.White;

            linkLabel4.Hide(); // hidden until fiko publishes the tutorial video

            _ = MatchAura.initAuras();

            _ = UpdateAccountTextBox();
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
           foreach (CheckBox box in BiomeCheckboxes)
            {
                if (box.Text.ToUpper() != biome.ToUpper() || !box.Checked)
                    continue;

                if (Webhooks.Webhooks.Count <= 0 && string.IsNullOrEmpty(privateserverlink))
                    return;

                int color = GetBiomeStuff(biome.ToUpper(), checkBox22.Checked, checkBox21.Checked, out bool ping);

                string mention = "";

                if (checkBox23.Checked)
                {
                    if (string.IsNullOrWhiteSpace(textBox3.Text))
                        return;

                    mention = $"<@&{textBox3.Text}>";
                }
                else if (checkBox24.Checked)
                {
                    if (string.IsNullOrWhiteSpace(textBox4.Text))
                        return;

                    mention = $"<@{textBox4.Text}>";
                }
                else if (checkBox25.Checked)
                {
                    mention = "@everyone";
                }

                _ = Webhooks.PostToWebhooks(biome, mention, ping, privateserverlink, color, userid, checkBox31.Checked);

                PrintLogs("Sent Webhook!");

                _ = UpdateAccountTextBox();
                return;
            }
        }

        private void titleBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, IntPtr.Zero);
            }
        }

        private async void Start(bool start)
        {
            if (!Start_Stop && start)
            {
                Start_Stop = true;
                SessionTimer.Restart();
                UntilAFK.Restart();
                _ = Uptime(label41);

                SessionBiomes = 0;
                SessionGlobals = 0;

                PrintLogs("Started");

                if (Webhooks.Webhooks.Count > 0)
                    await Webhooks.StartStopWebhooks(true, TimeSpan.Zero, SessionBiomes, SessionRare);

                _ = BiomeDetector.Biomes(this, textBox1);
                UpdateRunningStatus(true, pictureBox13);
                return;
            }

            if (Start_Stop && !start)
            {
                Start_Stop = false;
                SessionTimer.Stop();

                TimeSpan sessionTime = SessionTimer.Elapsed;

                PrintLogs($"Stopped - Session Time: {FormatSessionTime(sessionTime)}");

                if (Webhooks.Webhooks.Count > 0)
                    await Webhooks.StartStopWebhooks(false, sessionTime, SessionBiomes, SessionRare);

                UpdateRunningStatus(false, pictureBox13);
                return;
            }
        }
        private async Task Uptime(Label uptime)
        {
            while (Start_Stop)
            {
                TimeSpan time = SessionTimer.Elapsed;
                string formatted = $"{(int)time.TotalDays:D2}:{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
                uptime.Text = $"Uptime: {formatted}";
                await Task.Delay(1000);
            }
        }

        private void UpdateRunningStatus(bool update, PictureBox statusPictureBox)
        {
            if (update)
            {
                statusPictureBox.Image = Properties.Resources.running;
                return;
            }

            statusPictureBox.Image = Properties.Resources.stopped;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private async Task UpdateAccountTextBox()
        {
            int version = Interlocked.Increment(ref AccountRefreshVersion);

            int count = PrivateServers.Servers.Count;

            string[] accounts = new string[count];
            string[] userIds = new string[count];

            for (int i = 0; i < count; i++)
            {
                userIds[i] = PrivateServers.Servers[i].UserId.Trim();
                accounts[i] = await Webhooks.GetUsername(userIds[i]);
            }

            if (version != AccountRefreshVersion)
                return;

            StringBuilder output = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                string biome = BiomeDetector.LastBiomebyUser(userIds[i]);

                output.AppendLine($"{accounts[i]}: {biome}");
            }

            if (version != AccountRefreshVersion)
                return;

            textBox9.Text = output.ToString();

            textBox9.SelectionStart = textBox9.Text.Length;

            textBox9.ScrollToCaret();
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

            if (!LoadingSettings)
                SaveSettings();
        }

        private void UpdateStats(bool rarebiome)
        {
            if (rarebiome)
            {
                TotalRare++;

                SessionRare++;
            }

            TotalBiomes++;
            SessionBiomes++;

            Settings.Data.TotalBiomes = TotalBiomes;
            Settings.Data.TotalRareBiomes = TotalRare;

            Settings.Save();
            LoadStats();
        }

        private void LoadStats()
        {
            label9.Text = TotalRare.ToString();
            label14.Text = SessionRare.ToString();
            label11.Text = TotalBiomes.ToString();
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

            label78.Text = Settings.Data.TotalBlazingSun.ToString(); // Blazing Sun -> Incinerator
            label16.Text = Settings.Data.TotalIncinerator.ToString(); 

            label3.Text = SessionGlobals.ToString();
            label4.Text = TotalGlobals.ToString();
        }

        private int GetBiomeStuff(string Biome, bool onlyrareping, bool treatsingasrare, out bool ping)
        {
            ping = onlyrareping ? false : true;

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
                case "INCINERATOR":
                    {
                        Settings.Data.TotalIncinerator++;
                        Settings.Save();
                        UpdateStats(false);
                        return 0xff0000;
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
            Settings.Data.BlazingSun = checkBox15.Checked;
            Settings.Data.Incinerator = checkBox12.Checked;

            Settings.Data.TreatSingularityAsRare = checkBox21.Checked;
            Settings.Data.OnlyPingForRare = checkBox22.Checked;

            Settings.Data.PingRole = checkBox23.Checked;
            Settings.Data.PingUserID = checkBox24.Checked;
            Settings.Data.PingEveryone = checkBox25.Checked;
            Settings.Data.DontPing = checkBox26.Checked;

            Settings.Data.PingRoleID = textBox3.Text;
            Settings.Data.PingUserIDValue = textBox4.Text;

            Settings.Data.IncludeUsername = checkBox31.Checked;

            Settings.Data.AuraNotifications = checkBox32.Checked;
            Settings.Data.MinAuraRarity = textBox7.Text;
            Settings.Data.AuraPingUserID = checkBox33.Checked;
            Settings.Data.AuraUserID = textBox8.Text;
            Settings.Data.TotalGlobalsRolled = TotalGlobals;

            Settings.Save();
        }

        private void SettingChanged(object sender, EventArgs e)
        {
            if (!LoadingSettings)
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
            checkBox15.Checked = Settings.Data.BlazingSun;
            checkBox12.Checked = Settings.Data.Incinerator;

            checkBox21.Checked = Settings.Data.TreatSingularityAsRare;
            checkBox22.Checked = Settings.Data.OnlyPingForRare;

            checkBox23.Checked = Settings.Data.PingRole;
            checkBox24.Checked = Settings.Data.PingUserID;
            checkBox25.Checked = Settings.Data.PingEveryone;
            checkBox26.Checked = Settings.Data.DontPing;

            textBox3.Text = Settings.Data.PingRoleID;
            textBox4.Text = Settings.Data.PingUserIDValue;

            TotalBiomes = Settings.Data.TotalBiomes;
            TotalRare = Settings.Data.TotalRareBiomes;

            label11.Text = TotalBiomes.ToString();
            label9.Text = TotalRare.ToString();

            checkBox31.Checked = Settings.Data.IncludeUsername;

            checkBox32.Checked = Settings.Data.AuraNotifications;
            textBox7.Text = Settings.Data.MinAuraRarity;
            checkBox33.Checked = Settings.Data.AuraPingUserID;
            textBox8.Text = Settings.Data.AuraUserID;

            TotalGlobals = Settings.Data.TotalGlobalsRolled;
            label4.Text = Settings.Data.TotalGlobalsRolled.ToString();

        }

        private string FormatSessionTime(TimeSpan time)
        {
            return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }

        private void activebutton(object sender, EventArgs e)
        {
            if (sender is Button selectedButton)
            {
                Gui.SetActiveButton(selectedButton, SidebarButtons);

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

                else if (selectedButton == button27)
                    tabControl.SelectedTab = tabPage15;

                else if (selectedButton == button37)
                    tabControl.SelectedTab = tabPage16;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string webhook = textBox2.Text.Trim();

            if (Webhooks.EditingIndex == -1)
            {
                if (Webhooks.Contains(webhook))
                {
                    MessageBox.Show("This webhook is already added.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Webhooks.Add(webhook);
            }
            else
            {
                Webhooks.Update(Webhooks.EditingIndex, webhook);
                Webhooks.EditingIndex = -1;
            }

            Webhooks.RefreshList(listBox1);

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
                MessageBox.Show("Select a webhook first.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Delete this webhook?", "Vortex", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            Webhooks.Remove(listBox1.SelectedIndex);
            Webhooks.RefreshList(listBox1);

            _ = UpdateAccountTextBox();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a webhook first.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Webhooks.EditingIndex = listBox1.SelectedIndex;

            textBox2.Text = Webhooks.Get(Webhooks.EditingIndex);
            button11.Text = "Save Changes";

            tabControl.SelectedTab = tabPage7;

            _ = UpdateAccountTextBox();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            PrivateServers.EditingIndex = -1;

            textBox5.Clear();
            textBox6.Clear();

            tabControl.SelectedTab = tabPage9;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            _ = PrivateServers.RefreshList(listBox2);
            tabControl.SelectedTab = tabPage10;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            PrivateServers.EditingIndex = -1;

            textBox5.Clear();
            textBox6.Clear();

            tabControl.SelectedTab = tabPage16;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage16;
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

            if (!PrivateServers.IsValidLink(link))
            {
                MessageBox.Show("Please enter a valid Roblox private server link.", "Invalid Private Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!PrivateServers.IsValidUserId(userId))
            {
                MessageBox.Show("Please enter a valid Roblox User ID.", "Invalid User ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int existingIndex = PrivateServers.FindUser(userId);

            if (existingIndex != -1 && existingIndex != PrivateServers.EditingIndex)
            {
                MessageBox.Show("This Roblox User ID already has a private server configured.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (PrivateServers.EditingIndex == -1)
            {
                PrivateServers.Add(userId, link);
            }
            else
            {
                PrivateServers.Update(PrivateServers.EditingIndex, userId, link);
                PrivateServers.EditingIndex = -1;
            }

            _ = PrivateServers.RefreshList(listBox2);

            textBox5.Clear();
            textBox6.Clear();

            tabControl.SelectedTab = tabPage10;

            _ = UpdateAccountTextBox();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select a private server first.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrivateServers.EditingIndex = listBox2.SelectedIndex;

            PrivateServer.Entry entry = PrivateServers.Get(PrivateServers.EditingIndex);

            textBox5.Text = entry.Link;
            textBox6.Text = entry.UserId;

            button19.Text = "Save Changes";

            tabControl.SelectedTab = tabPage9;

            _ = UpdateAccountTextBox();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select a private server first.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrivateServer.Entry entry = PrivateServers.Get(listBox2.SelectedIndex);


            DialogResult result = MessageBox.Show($"Delete the private server for User ID {entry.UserId}?", "Vortex", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            PrivateServers.Remove(listBox2.SelectedIndex);
            _ = PrivateServers.RefreshList(listBox2);

            _ = UpdateAccountTextBox();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public string GetPrivateServerForUser(string userId)
        {
            return PrivateServers.GetForUser(userId);
        }

        private void checkBox28_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox28.Checked)
                return;

            string path = RobloxHandle.FindHandleExe();

            if (string.IsNullOrWhiteSpace(path))
            {
                tabControl.SelectedTab = tabPage12;
                checkBox28.Checked = false;
            }
            else
            {
                if (!AskForAdmin())
                {
                    checkBox28.Checked = false;
                    return;
                }

                bool result = RobloxHandle.CloseSingletonEvent(path);

                if (!result)
                {
                    checkBox28.Checked = false;
                    PrintLogs("Couldn't close the Handle!");
                    DialogResult dresult = MessageBox.Show(RobloxHandle.LastError, "Failed to close Roblox Handle", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);

                    if (dresult == DialogResult.Retry)
                    {
                        PrintLogs("Retrying...");
                        bool retryresult = RobloxHandle.CloseSingletonEvent(path);

                        if (!retryresult)
                        {
                            PrintLogs("Couldn't close the Handle! x2");
                            MessageBox.Show(RobloxHandle.LastError, "Failed to close Roblox Handle (again), check the box again to retry (if u want)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            checkBox28.Checked = true;
                        }
                    }

                    if (dresult == DialogResult.Cancel)
                    {
                        checkBox28.Checked = false;
                        return;
                    }
                }
                else
                {
                    checkBox28.Checked = true;
                }
            }
        }

        private static bool AskForAdmin()
        {
            WindowsIdentity ntIdentity = WindowsIdentity.GetCurrent();

            WindowsPrincipal principal = new WindowsPrincipal(ntIdentity);

            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                return true;

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath,
                Verb = "runas",
                UseShellExecute = true
            };

            try
            {
                Process.Start(info);

                Application.Exit();

                return false;
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223)
                    return false;

                throw;
            }
            finally
            {
                ntIdentity.Dispose();
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
                if (Start_Stop && UntilAFK.ElapsedMilliseconds > AFKDelay)
                {
                    if (!checkBox29.Checked || !Start_Stop)
                        continue;

                    AntiAFK.SendActivity();
                    UntilAFK.Restart();
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
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
                MessageBox.Show("Select a webhook first.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _ = Webhooks.TestWebhook(Webhooks.Get(listBox1.SelectedIndex));
        }

        private void button25_Click_1(object sender, EventArgs e)
        {
            Start(!Start_Stop);
        }

        public async Task FoundNewAura(string aura, string userid, string biome)
        {
            if (!checkBox32.Checked)
                return;

            if (Webhooks.Webhooks.Count == 0)
                return;

            string temp = string.IsNullOrWhiteSpace(textBox7.Text) ? "0" : textBox7.Text;
            temp = temp.Replace(",", "");

            bool success = long.TryParse(temp, out long minrarity);

            if (!success)
                return;

            var result = await MatchAura.GetAura(aura, biome, minrarity);

            if (string.IsNullOrWhiteSpace(result.Name))
                return;

            if (result.Rarity == 0)
                return;

            if (result.Global)
                UpdateAuraStats();

            string stat = result.Rarity.ToString();
            bool native = result.bIsNative;

            await Webhooks.PostAuraToWebhooks(aura, userid, checkBox31.Checked, true, $"<@{textBox8.Text}>", stat, native, biome);

            PrintLogs($"Sent Aura Webhook: {aura}");

        }

        private void UpdateAuraStats()
        {
            TotalGlobals++;
            SessionGlobals++;

            Settings.Data.TotalGlobalsRolled = TotalGlobals;

            Settings.Save();
            LoadStats();
        }

        private void button25_Click_2(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Discord server
        {
            Process.Start(new ProcessStartInfo("https://www.discord.gg/XhBq3tmQcm") { UseShellExecute = true });
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Tutorial vid
        {
            // Process.Start(new ProcessStartInfo("...") { UseShellExecute = true });
        }

        private void pictureBox13_Click(object sender, EventArgs e) // Status picture
        {
            if (!Start_Stop)
                Start(true);
            else
                Start(false);
        }

        private void button9_Click_1(object sender, EventArgs e) // add w
        {
            tabControl.SelectedTab = tabPage7;
        }

        private void button16_Click_1(object sender, EventArgs e) // add a
        {
            tabControl.SelectedTab = tabPage9;
        }

        private void button10_Click_1(object sender, EventArgs e) // view w
        {
            tabControl.SelectedTab = tabPage8;
        }

        private void button17_Click_1(object sender, EventArgs e) // view a
        {
            tabControl.SelectedTab = tabPage10;
        }

        private void button9_Click_2(object sender, EventArgs e)
        {
            Webhooks.EditingIndex = -1;
            textBox2.Text = "";
            tabControl.SelectedTab = tabPage7;
        }

        private void button10_Click_2(object sender, EventArgs e)
        {
            Webhooks.RefreshList(listBox1);
            tabControl.SelectedTab = tabPage8;
        }
    }
}
