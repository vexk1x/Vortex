using System.Runtime.CompilerServices;
using System.Diagnostics;

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

        private Panel darkTabHeaderFill;

        private readonly Color DarkBackground = Color.FromArgb(17, 19, 24);
        private readonly Color DarkPanel = Color.FromArgb(25, 28, 35);
        private readonly Color DarkInput = Color.FromArgb(32, 36, 45);
        private readonly Color DarkBorder = Color.FromArgb(42, 47, 58);

        private readonly Color MainText = Color.FromArgb(241, 243, 245);
        private readonly Color SecondaryText = Color.FromArgb(156, 163, 175);

        private readonly Color Accent = Color.FromArgb(123, 97, 255);




        private readonly Stopwatch sessionTimer = new Stopwatch();
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            tabPage1.Text = "General";
            tabPage2.Text = "Biomes";
            tabPage3.Text = "Webhook";
            tabPage4.Text = "Info";
            tabPage5.Text = "Logs";
            tabPage6.Text = "Stats";

            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.WordWrap = false;
            textBox1.Dock = DockStyle.Fill;


            loadingSettings = true;

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();

                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            LoadSettings();
            loadingSettings = false;

            applystyle();

        }

        public void PrintLogs(string log)
        {
            textBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {log} {Environment.NewLine}");
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
            CheckBox[] biomeCheckboxes =
            {
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
            };

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

                    if (textBox2.Text != string.Empty && textBox5.Text != string.Empty)
                    {

                        int color = GetColor(biome.ToUpper());

                        if (checkBox23.Checked)
                        {
                            if (textBox3.Text != string.Empty)
                            {
                                PrintLogs("Sent Webhook!");
                                Program.PostToWebhook(textBox2.Text, biome, textBox3.Text, doping, textBox5.Text, color);
                            }
                        }
                        if (checkBox24.Checked)
                        {
                            if (textBox4.Text != string.Empty)
                            {
                                PrintLogs("Sent Webhook!");
                                Program.PostToWebhook(textBox2.Text, biome, "<@" + textBox4.Text + ">", doping, textBox5.Text, color);
                            }
                        }
                        if (checkBox25.Checked)
                        {
                            PrintLogs("Sent Webhook!");
                            Program.PostToWebhook(textBox2.Text, biome, "@everyone", doping, textBox5.Text, color);

                        }
                        if (checkBox26.Checked)
                        {
                            PrintLogs("Sent Webhook!");
                            Program.PostToWebhook(textBox2.Text, biome, string.Empty, doping, textBox5.Text, color);

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

            Properties.Settings.Default.webhook = textBox2.Text;
            Properties.Settings.Default.privateserver = textBox5.Text;

            Properties.Settings.Default.lastBiome = BiomeDetector.lastValidBiome;

            Properties.Settings.Default.darkMode = checkBox27.Checked;



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

            textBox2.Text = Properties.Settings.Default.webhook;
            textBox5.Text = Properties.Settings.Default.privateserver;

            totalBiomes = Properties.Settings.Default.totalbiomes;
            totalRare = Properties.Settings.Default.totalrarebiomes;

            label11.Text = totalBiomes.ToString();
            label9.Text = totalRare.ToString();

            checkBox27.Checked = Properties.Settings.Default.darkMode;


        }

        private void checkBox27_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox27.Checked)
            {
                ApplyDarkTheme(this);
                EnableDarkTabs();
            }
            else
            {
                ApplyLightTheme(this);
                DisableDarkTabs();
            }

            SettingChanged(sender, e);
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

                Task Biomes = BiomeDetector.Biomes(this);

                PrintLogs("Started");

                if (!string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    await Program.StartStopWebhook(textBox2.Text, true, TimeSpan.Zero);
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

                if (!string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    await Program.StartStopWebhook(textBox2.Text, false, sessionTime);
                }

                return;
            }
        }

        private void applystyle()
        {
            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 0;

            button3.BackColor = Accent;
            button3.ForeColor = Color.White;

            button3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button3.Cursor = Cursors.Hand;

            button3.Size = new Size(150, 42);
            button3.Left = ClientSize.Width - button3.Width - 20;

            if (checkBox27.Checked)
                ApplyDarkTheme(this);

            ApplyCheckboxColor();
        }


        private void ApplyDarkTheme(Control parent)
        {
            parent.BackColor = DarkBackground;
            parent.ForeColor = MainText;

            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.BackColor = DarkInput;
                        textBox.ForeColor = MainText;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case Button button:
                        button.BackColor = Accent;
                        button.ForeColor = Color.White;

                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;

                        button.Cursor = Cursors.Hand;
                        break;

                    case CheckBox checkBox:
                        checkBox.ForeColor = MainText;
                        checkBox.BackColor = DarkBackground;
                        break;

                    case Label label:
                        label.ForeColor = MainText;
                        label.BackColor = DarkBackground;
                        break;

                    case TabPage tabPage:
                        tabPage.BackColor = DarkBackground;
                        tabPage.ForeColor = MainText;
                        break;

                    case Panel panel:
                        panel.BackColor = DarkPanel;
                        panel.ForeColor = MainText;
                        break;
                }

                if (control.HasChildren)
                {
                    ApplyDarkTheme(control);
                }
            }
            ApplyCheckboxColor();
        }

        private void ApplyLightTheme(Control parent)
        {
            parent.BackColor = Color.White;
            parent.ForeColor = Color.Black;

            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.BackColor = Color.White;
                        textBox.ForeColor = Color.Black;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case Button button:
                        button.BackColor = Accent;
                        button.ForeColor = Color.White;

                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;

                        button.Cursor = Cursors.Hand;
                        break;

                    case CheckBox checkBox:
                        checkBox.ForeColor = Color.Black;
                        checkBox.BackColor = Color.White;
                        break;

                    case Label label:
                        label.ForeColor = Color.Black;
                        label.BackColor = Color.White;
                        break;

                    case TabPage tabPage:
                        tabPage.BackColor = Color.White;
                        tabPage.ForeColor = Color.Black;
                        break;

                    case Panel panel:
                        panel.BackColor = Color.White;
                        panel.ForeColor = Color.Black;
                        break;
                }

                if (control.HasChildren)
                {
                    ApplyLightTheme(control);
                }
            }
            ApplyCheckboxColor();
        }

        private void EnableDarkTabs()
        {
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;

            tabControl.Appearance = TabAppearance.Normal;

            tabControl.DrawItem -= TabControl_DrawItem;
            tabControl.DrawItem += TabControl_DrawItem;

            tabControl.Invalidate();

            Rectangle lastTab = tabControl.GetTabRect(tabControl.TabPages.Count - 1);

            if (darkTabHeaderFill == null)
            {
                darkTabHeaderFill = new Panel
                {
                    BackColor = DarkBackground
                };

                tabControl.Parent.Controls.Add(darkTabHeaderFill);
            }

            darkTabHeaderFill.BackColor = DarkBackground;

            darkTabHeaderFill.Location = new Point(
                tabControl.Left + lastTab.Right,
                tabControl.Top
            );

            darkTabHeaderFill.Size = new Size(
                tabControl.Width - lastTab.Right,
                lastTab.Height
            );

            darkTabHeaderFill.Visible = true;
            darkTabHeaderFill.BringToFront();
        }

        private void DisableDarkTabs()
        {
            tabControl.DrawItem -= TabControl_DrawItem;
            tabControl.DrawMode = TabDrawMode.Normal;

            if (darkTabHeaderFill != null)
            {
                darkTabHeaderFill.Visible = false;
            }

            tabControl.Invalidate();
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabs = (TabControl)sender;
            bool selected = e.Index == tabs.SelectedIndex;

            Rectangle rect = e.Bounds;

            rect.Inflate(2, 2);

            Color background = selected
                ? Color.FromArgb(32, 36, 45)
                : DarkBackground;

            using (SolidBrush backgroundBrush = new SolidBrush(background))
            {
                e.Graphics.FillRectangle(backgroundBrush, rect);
            }

            Rectangle textRect = e.Bounds;

            TextRenderer.DrawText
            (
                e.Graphics,
                tabs.TabPages[e.Index].Text,
                new Font("Segoe UI", 9F, selected ? FontStyle.Bold : FontStyle.Regular),
                textRect,
                MainText,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding
            );

            if (selected)
            {
                using Pen accentPen = new Pen(Accent, 2);

                e.Graphics.DrawLine(
                    accentPen,
                    e.Bounds.Left + 5,
                    e.Bounds.Bottom - 1,
                    e.Bounds.Right - 5,
                    e.Bounds.Bottom - 1
                );
            }
        }

        private void ApplyCheckboxColor()
        {
            checkBox1.ForeColor = ColorTranslator.FromHtml("0x4e4e4e");
            checkBox2.ForeColor = ColorTranslator.FromHtml("0xc2f2ff");
            checkBox3.ForeColor = ColorTranslator.FromHtml("0xb6cbd1");
            checkBox4.ForeColor = ColorTranslator.FromHtml("0x0000ff");
            checkBox5.ForeColor = ColorTranslator.FromHtml("0xffbb00");
            checkBox6.ForeColor = ColorTranslator.FromHtml("0x770a0a");
            checkBox7.ForeColor = ColorTranslator.FromHtml("0x3b3abc");
            checkBox8.ForeColor = ColorTranslator.FromHtml("0xf4fb01");
            checkBox9.ForeColor = ColorTranslator.FromHtml("0x310387");
            checkBox10.ForeColor = ColorTranslator.FromHtml("0x000000");
            checkBox11.ForeColor = ColorTranslator.FromHtml("0xbf6c00");
            checkBox12.ForeColor = ColorTranslator.FromHtml("0x08043f");
            checkBox13.ForeColor = ColorTranslator.FromHtml("0xe500ff");
            checkBox14.ForeColor = ColorTranslator.FromHtml("0x212121");
            checkBox15.ForeColor = ColorTranslator.FromHtml("0xfaff00");
            checkBox16.ForeColor = ColorTranslator.FromHtml("0x9fff9a");
            checkBox17.ForeColor = ColorTranslator.FromHtml("0x996505");
            checkBox18.ForeColor = ColorTranslator.FromHtml("0x3e0000");
            checkBox19.ForeColor = ColorTranslator.FromHtml("0xc1ecff");
            checkBox20.ForeColor = ColorTranslator.FromHtml("0x8d7dc7");
        }
    }
}
