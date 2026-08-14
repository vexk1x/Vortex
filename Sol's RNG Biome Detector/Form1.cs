using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        private readonly Color Accent = Color.FromArgb(123, 97, 255);

        private readonly CheckBox[] biomeCheckboxes;

        private readonly Button[] sidebarButtons;

        private GUI gui = new GUI();

        private readonly List<string> webhooks = new List<string>();

        private int editingWebhookIndex = -1;

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
                button8
            ];

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadingSettings = true;

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();

                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            LoadSettings();
            LoadWebhooks();
            loadingSettings = false;


            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.WordWrap = false;
            textBox1.Dock = DockStyle.Fill;
            button1.BringToFront();


            gui.ApplyStyle(this, button3, tabControl, biomeCheckboxes, panelSidebar, panelContent);
            gui.StyleSidebar(panelSidebar, sidebarButtons);
        }

        public void PrintLogs(string log)
        {
            textBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {log}{Environment.NewLine}");
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        public void FoundNewBiome(string biome)
        {
            bool countstats = true;
            bool checkedonce = false;

            if (Properties.Settings.Default.lastBiome == biome && !checkedonce)
            {
                countstats = false;
                checkedonce = true;
            }

            foreach (CheckBox box in biomeCheckboxes)
            {
                if (box.Text.ToUpper() == biome.ToUpper())
                {
                    if (!box.Checked)
                        return;

                    bool rarebiome = false;
                    bool doping = false;

                    if (checkBox22.Checked)
                    {
                        switch (biome.ToUpper())
                        {
                            case "CYBERSPACE":
                            case "DREAMSPACE":
                            case "GLITCHED":
                                rarebiome = true;
                                doping = true;
                                if (countstats)
                                    UpdateStats(true);

                                break;
                            case "SINGULARITY":
                                if (checkBox21.Checked)
                                {
                                    rarebiome = true;
                                    doping = true;
                                    if (countstats)
                                        UpdateStats(true);
                                }
                                else
                                {
                                    rarebiome = false;
                                    doping = false;
                                    if (countstats)
                                        UpdateStats(true);
                                }
                                break;

                            default:
                                if (countstats)
                                    UpdateStats(false);
                                break;
                        }
                    }
                    else
                    {
                        doping = true;

                        switch (biome.ToUpper())
                        {
                            case "CYBERSPACE":
                            case "DREAMSPACE":
                            case "GLITCHED":

                                if (countstats)
                                    UpdateStats(true);
                                rarebiome = true;

                                break;

                            case "SINGULARITY":
                                if (checkBox21.Checked)
                                {

                                    if (countstats)
                                        UpdateStats(true);

                                    rarebiome = true;
                                }
                                else
                                {

                                    if (countstats)
                                        UpdateStats(true);
                                    rarebiome = false;
                                }
                                break;

                            default:
                                if (countstats)
                                    UpdateStats(false);
                                break;
                        }
                    }

                    if (webhooks.Count > 0 && !string.IsNullOrWhiteSpace(textBox5.Text))
                    {

                        int color = GetColor(biome.ToUpper());

                        if (checkBox23.Checked)
                        {
                            if (textBox3.Text != string.Empty)
                            {
                                PrintLogs("Sent Webhook!");
                                Program.PostToWebhooks(webhooks, biome, $"<@&{textBox3.Text}>", doping, textBox5.Text, color);
                            }
                        }
                        if (checkBox24.Checked)
                        {
                            if (textBox4.Text != string.Empty)
                            {
                                PrintLogs("Sent Webhook!");
                                Program.PostToWebhooks(webhooks, biome, $"<@{textBox4.Text}>", doping, textBox5.Text, color);
                            }
                        }
                        if (checkBox25.Checked)
                        {
                            PrintLogs("Sent Webhook!");
                            Program.PostToWebhooks(webhooks, biome, "@everyone", doping, textBox5.Text, color);

                        }
                        if (checkBox26.Checked)
                        {
                            PrintLogs("Sent Webhook!");
                            Program.PostToWebhooks(webhooks, biome, string.Empty, doping, textBox5.Text, color);

                        }
                    }

                    doping = false;
                    rarebiome = false;
                    countstats = true;
                }
            }
            return;
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
                label9.Text = totalRare.ToString();

                SessionRare++;
                label14.Text = SessionRare.ToString();
            }

            totalBiomes++;
            SessionBiomes++;

            label11.Text = totalBiomes.ToString();
            label15.Text = SessionBiomes.ToString();

            Properties.Settings.Default.totalbiomes = totalBiomes;
            Properties.Settings.Default.totalrarebiomes = totalRare;
        }

        private int GetColor(string Biome)
        {
            switch (Biome)
            {
                case "NORMAL":
                    return 0x4e4e4e;

                case "WINDY":
                    return 0xc2f2ff;

                case "SNOWY":
                    return 0xb6cbd1;

                case "RAINY":
                    return 0x0000ff;

                case "SAND STORM":
                    return 0xffbb00;

                case "HELL":
                    return 0x770a0a;

                case "STARFALL":
                    return 0x3b3abc;

                case "HEAVEN":
                    return 0xf4fb01;

                case "CORRUPTION":
                    return 0x310387;

                case "NULL":
                    return 0x000000;

                case "SINGULARITY":
                    return 0xbf6c00;

                case "CYBERSPACE":
                    return 0x08043f;

                case "DREAMSPACE":
                    return 0xe500ff;

                case "GLITCHED":
                    return 0x212121;

                case "BLAZING SUN":
                    return 0xfaff00;

                case "EGGLAND":
                    return 0x9fff9a;

                case "PUMPKIN MOON":
                    return 0x996505;

                case "BLOOD RAIN":
                    return 0x3e0000;

                case "GRAVEYARD":
                    return 0xc1ecff;

                case "AURORA":
                    return 0x8d7dc7;


                default:
                    return 0xFFFFFF;
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Normal = checkBox1.Checked;
            Properties.Settings.Default.Windy = checkBox2.Checked;
            Properties.Settings.Default.Snowy = checkBox3.Checked;
            Properties.Settings.Default.Rainy = checkBox4.Checked;
            Properties.Settings.Default.Sand_Storm = checkBox5.Checked;
            Properties.Settings.Default.Hell = checkBox6.Checked;
            Properties.Settings.Default.Starfall = checkBox7.Checked;
            Properties.Settings.Default.Heaven = checkBox8.Checked;
            Properties.Settings.Default.Corruption = checkBox9.Checked;
            Properties.Settings.Default.Null = checkBox10.Checked;
            Properties.Settings.Default.Singularity = checkBox11.Checked;
            Properties.Settings.Default.Cyberspace = checkBox12.Checked;
            Properties.Settings.Default.Dreamspace = checkBox13.Checked;
            Properties.Settings.Default.Glitched = checkBox14.Checked;
            Properties.Settings.Default.Blazing_Sun = checkBox15.Checked;
            Properties.Settings.Default.Eggland = checkBox16.Checked;
            Properties.Settings.Default.Pumpkin_Moon = checkBox17.Checked;
            Properties.Settings.Default.Blood_Rain = checkBox18.Checked;
            Properties.Settings.Default.Graveyard = checkBox19.Checked;
            Properties.Settings.Default.Aurora = checkBox20.Checked;
            Properties.Settings.Default.treatsingasrare = checkBox21.Checked;
            Properties.Settings.Default.onlypingforrare = checkBox22.Checked;
            Properties.Settings.Default.pingrole = checkBox23.Checked;
            Properties.Settings.Default.pinguserid = checkBox24.Checked;
            Properties.Settings.Default.pingeveryone = checkBox25.Checked;
            Properties.Settings.Default.dontping = checkBox26.Checked;

            Properties.Settings.Default.sPingRole = textBox3.Text;
            Properties.Settings.Default.sPingUserID = textBox4.Text;

            Properties.Settings.Default.privateserver = textBox5.Text;

            Properties.Settings.Default.lastBiome = BiomeDetector.lastValidBiome;

            Properties.Settings.Default.Save();

        }

        private void SettingChanged(object sender, EventArgs e)
        {
            if (loadingSettings)
                return;

            SaveSettings();
        }

        private void LoadSettings()
        {
            checkBox1.Checked = Properties.Settings.Default.Normal;
            checkBox2.Checked = Properties.Settings.Default.Windy;
            checkBox3.Checked = Properties.Settings.Default.Snowy;
            checkBox4.Checked = Properties.Settings.Default.Rainy;
            checkBox5.Checked = Properties.Settings.Default.Sand_Storm;
            checkBox6.Checked = Properties.Settings.Default.Hell;
            checkBox7.Checked = Properties.Settings.Default.Starfall;
            checkBox8.Checked = Properties.Settings.Default.Heaven;
            checkBox9.Checked = Properties.Settings.Default.Corruption;
            checkBox10.Checked = Properties.Settings.Default.Null;
            checkBox11.Checked = Properties.Settings.Default.Singularity;
            checkBox12.Checked = Properties.Settings.Default.Cyberspace;
            checkBox13.Checked = Properties.Settings.Default.Dreamspace;
            checkBox14.Checked = Properties.Settings.Default.Glitched;
            checkBox15.Checked = Properties.Settings.Default.Blazing_Sun;
            checkBox16.Checked = Properties.Settings.Default.Eggland;
            checkBox17.Checked = Properties.Settings.Default.Pumpkin_Moon;
            checkBox18.Checked = Properties.Settings.Default.Blood_Rain;
            checkBox19.Checked = Properties.Settings.Default.Graveyard;
            checkBox20.Checked = Properties.Settings.Default.Aurora;

            checkBox21.Checked = Properties.Settings.Default.treatsingasrare;
            checkBox22.Checked = Properties.Settings.Default.onlypingforrare;

            checkBox23.Checked = Properties.Settings.Default.pingrole;
            checkBox24.Checked = Properties.Settings.Default.pinguserid;
            checkBox25.Checked = Properties.Settings.Default.pingeveryone;
            checkBox26.Checked = Properties.Settings.Default.dontping;

            textBox3.Text = Properties.Settings.Default.sPingRole;
            textBox4.Text = Properties.Settings.Default.sPingUserID;

            textBox5.Text = Properties.Settings.Default.privateserver;

            totalBiomes = Properties.Settings.Default.totalbiomes;
            totalRare = Properties.Settings.Default.totalrarebiomes;

            label11.Text = totalBiomes.ToString();
            label9.Text = totalRare.ToString();


        }

        private string FormatSessionTime(TimeSpan time)
        {
            return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (!Start_Stop)
            {
                Start_Stop = true;
                sessionTimer.Restart();

                button3.Text = "Stop";
                button3.ForeColor = Color.White;
                button3.BackColor = Color.FromArgb(220, 60, 70);

                PrintLogs("Started");

                BiomeDetector.lastValidBiome = "";
                Task Biomes = BiomeDetector.Biomes(this);

                if (webhooks.Count > 0)
                {
                    try
                    {
                        await Program.StartStopWebhooks(webhooks, true, TimeSpan.Zero);
                    }
                    catch (Exception ex)
                    {
                        PrintLogs("Webhook Error: " + ex.Message);
                    }
                }

                return;
            }

            if (Start_Stop)
            {
                Start_Stop = false;
                sessionTimer.Stop();

                TimeSpan sessionTime = sessionTimer.Elapsed;

                button3.Text = "Start";
                button3.ForeColor = Color.White;
                button3.BackColor = Accent;

                PrintLogs($"Stopped - Session Time: {FormatSessionTime(sessionTime)}");

                if (webhooks.Count > 0)
                {
                    try
                    {
                        await Program.StartStopWebhooks(webhooks, false, sessionTime);
                    }
                    catch (Exception ex)
                    {
                        PrintLogs("Webhook Error: " + ex.Message);
                    }
                }

                return;
            }
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
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            editingWebhookIndex = -1;
            textBox2.Text = "";
            button11.Text = "Add Webhook";
            tabControl.SelectedTab = tabPage7;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            RefreshWebhookList();
            tabControl.SelectedTab = tabPage8;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string webhook = textBox2.Text.Trim();

            if (!IsValidWebhook(webhook))
            {
                MessageBox.Show("Please enter a valid Discord webhook URL.", "Invalid Webhook", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (editingWebhookIndex == -1)
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
                webhooks[editingWebhookIndex] = webhook;
                editingWebhookIndex = -1;
            }

            SaveWebhooks();
            RefreshWebhookList();

            textBox2.Text = "";
            button11.Text = "Add Webhook";

            tabControl.SelectedTab = tabPage8;
        }

        private bool IsValidWebhook(string webhook)
        {
            if (!Uri.TryCreate(webhook, UriKind.Absolute, out Uri? uri))
                return false;

            bool discordHost = uri.Host.Equals("discord.com", StringComparison.OrdinalIgnoreCase) || uri.Host.EndsWith(".discord.com", StringComparison.OrdinalIgnoreCase);
            bool webhookPath = uri.AbsolutePath.StartsWith("/api/webhooks/", StringComparison.OrdinalIgnoreCase);

            return discordHost && webhookPath;
        }

        private void LoadWebhooks()
        {
            webhooks.Clear();

            try
            {
                List<string>? savedWebhooks = JsonSerializer.Deserialize<List<string>>(Properties.Settings.Default.webhooks);

                if (savedWebhooks != null)
                    webhooks.AddRange(savedWebhooks);
            }
            catch
            {
                Properties.Settings.Default.webhooks = "[]";
                Properties.Settings.Default.Save();
            }

            if (webhooks.Count == 0 && !string.IsNullOrWhiteSpace(Properties.Settings.Default.webhook))
            {
                webhooks.Add(Properties.Settings.Default.webhook);
                SaveWebhooks();
            }

            RefreshWebhookList();
        }

        private void SaveWebhooks()
        {
            Properties.Settings.Default.webhooks = JsonSerializer.Serialize(webhooks);
            Properties.Settings.Default.Save();
        }

        private void RefreshWebhookList()
        {
            listBox1.Items.Clear();

            for (int i = 0; i < webhooks.Count; i++)
                listBox1.Items.Add($"Webhook {i + 1} | {GetWebhookID(webhooks[i])}");
        }

        private string GetWebhookID(string webhook)
        {
            try
            {
                Uri uri = new Uri(webhook);
                string[] parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3)
                    return parts[2];
            }
            catch
            {
            }

            return "Unknown";
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

            webhooks.RemoveAt(listBox1.SelectedIndex);

            SaveWebhooks();
            RefreshWebhookList();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a webhook first.", "Bloom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            editingWebhookIndex = listBox1.SelectedIndex;

            textBox2.Text = webhooks[editingWebhookIndex];
            button11.Text = "Save Changes";

            tabControl.SelectedTab = tabPage7;
        }
    }
}
