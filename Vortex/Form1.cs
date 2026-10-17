using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Vortex
{
    public partial class Form1 : Form
    {
        // Why are you even reading this

        // VERSION
        private static readonly Version @Version = new Version("1.0.6"); 
        public static readonly string _Version = $"Vortex v{@Version} - Early Release";


        // Public 
        public List<CheckBox> BiomeCheckboxes { get; } = new List<CheckBox>();

        private List<CheckBox> PingCheckBoxes = new List<CheckBox>();

        private List<Label> StatsCounter = new List<Label>();

        public static List<Label> SessionTotalCounter = new List<Label>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBox logs { get; set; } = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBox AccountsList { get; set; } = null!;

        public static long Auras_MinPingRarity { get; set; } = -1;
        public static long Auras_NotificationID { get; set; } = -1;

        public static bool Auras_Notifications { get; set; } = false;


        public static Action<string, Color> _PrintLogs = (log, color) => // ts lwk peak
        {
            Instance.PrintLogs(log, color);
        };

        public static readonly string[] CurrentBiomes = {
            "Normal",
            "Windy",
            "Snowy",
            "Rainy",
            "Sand Storm",
            "Hell",
            "Starfall",
            "Heaven",
            "Corruption",
            "Null",
            "Singularity",
            "Cyberspace",
            "Dreamspace",
            "Glitched",
            "Lost Forest", // misplaced but it doesn't really matter since as of rn there isnt much info about these 2 forest biomes.
            "Radiant Forest"
        }; // Blazing Sun & Incinerator are invalid as of rn // maybe i'll add a list on my github so older versions are able to detect newer biomes aswell, that would also mean i'd have to add colors etc

        public static readonly Color[] Colors =
                {
                    Color.FromArgb(100, 134, 134, 134), // Normal
                    Color.FromArgb(100, 185, 244, 255), // Windy
                    Color.FromArgb(100, 202, 238, 255), // Snowy
                    Color.FromArgb(100, 10, 92, 216), // Rainy
                    Color.FromArgb(100, 202, 173, 57), // Sand Storm
                    Color.FromArgb(100, 156, 44, 44), // Hell
                    Color.FromArgb(100, 28, 74, 255), // Starfall
                    Color.FromArgb(100, 222, 207, 71), // Heaven
                    Color.FromArgb(100, 77, 15, 73), // Corruption
                    Color.FromArgb(100, 0, 0, 0), // Null
                    Color.FromArgb(100, 172, 103, 0), // Singularity
                    Color.FromArgb(100, 19, 53, 91), // Cyberspace
                    Color.FromArgb(100, 181, 69, 215), // Dreamspace (peak biome)
                    Color.FromArgb(100, 62, 59, 63), // Glitched
                    Color.FromArgb(101, 176, 83), // Lost Forest
                    Color.FromArgb(94, 37, 158), // Radiant Forest
                    //Color.FromArgb(100, 255, 248, 137), // Blazing Sun
                    //Color.FromArgb(95, 255, 83, 57) // Incinerator
                };

        // Private

        private static Form1 Instance { get; set; }

        private List<Label> StatsCL = new List<Label>();
        private List<Panel> Panels = new List<Panel>();
        private List<Button> SidebarButtons = new List<Button>();
        private List<Panel> Seperators = new List<Panel>();

        private bool PictureRunningState = false;

        private Label Uptime = null;

        private Stopwatch SessionTime { get; } = new Stopwatch();


        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, ref uint pcFonts);


        private  Color Theme_Backcolor = Color.FromArgb(17, 19, 24);
        private  Color Theme_Seperator = Color.FromArgb(200, 200, 200);
        private  Color Theme_Forecolor = Color.White;
        private  Color Theme_LinkLabelColor = Color.FromArgb(60, 60, 180);
        

        private int ThemesIndex = 0;
        private int FontIndex = 0;

        private bool LoadingSettings;

        private readonly PrivateFontCollection Sarpanch_Regular = new PrivateFontCollection();

        private FontStyle style; 

        private readonly List<(string, Color)> PendingLogs = new List<(string, Color)>();

        private readonly Running set;
        private readonly Accounts AccountData = new Accounts();
        private readonly ListBox AccountBox = new ListBox();
        internal readonly Webhook Webhooks = new Webhook();
        private ListBox WebhookList = new ListBox();

        private static readonly Size FormSize = new Size(720, 520);
        private static readonly Size SidebarSize = new Size(160, 0); // Dockstyle Left
        private static readonly Size TitlebarSize = new Size(0, 36); // Dockstyle Top
        private static readonly Size PanelSize = new Size(FormSize.Width - SidebarSize.Width, FormSize.Height - TitlebarSize.Height); // Dockstyle Fill

        private CheckBox cAuraNotifications;
        private TextBox tMinAuraRarity;
        private CheckBox cAuraPingUser;
        private TextBox tAuraUserId;
        private TextBox tPingRoleId;
        private TextBox tPingUserIdValue;
        private CheckBox cSingularityRare;
        private CheckBox cIncludeUsername;
        private ListBox lbListBox;


        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            Instance = this;

            Settings.Load(); // Dangerous shit

            ThemesIndex = Settings.Data.ThemeIndex;
            FontIndex = Settings.Data.FontIndex;

            LoadingSettings = true;

            Load += Form1_Load;
            FormClosed += Form1_Close;

            set = new Running(this);

            // Form itself

            FormBorderStyle = FormBorderStyle.None;
            
            Size = FormSize;
            ClientSize = FormSize;
            RoundFCorners(15);
            BackColor = Theme_Backcolor; // Needed cuz when switching panels the forms backcolor sometimes shows. just cleaner transitions this way

            StartPosition = FormStartPosition.CenterScreen;

            // Gui stuff
            LoadFont();

            InitializePanels();

            // settings
            LoadSettings();
            LoadingSettings = false;
            // /settings

            DrawSidebar();

            DrawTitlebar();

            ApplySidebarFont(false, "");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            lbListBox.BackColor = Theme_Backcolor; // idk why but it kept bugging out sometimes so i added this.
            lbListBox.ForeColor = Theme_Forecolor;

            Webhooks.Load(WebhookList);

            Task auras = ProcessData.LoadAuras();

            await AccountData.Load(AccountBox);

            RefreshAccountsList();

            await auras;
        }

        private async void Form1_Close(object sender, EventArgs e)
        {
            SaveSettings();
            await Webhooks.StartStopWebhook(false, GetSessionTime(), int.Parse(SessionTotalCounter[0].Text), int.Parse(SessionTotalCounter[1].Text));
            Application.Exit();
        }

        private void RoundFCorners(int r) // Round the corners of the form (self explanatory)
        {
            GraphicsPath gp = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            int d = r * 2;

            gp.StartFigure();

            gp.AddArc(rect.X, rect.Y, d, d, 180, 90);
            gp.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            gp.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            gp.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

            gp.CloseFigure();

            Region = new Region(gp);

            gp.Dispose();
        }

        private void LoadFont()
        {
            byte[] fontdata = Properties.Resources.Sarpanch_Regular;

            IntPtr memory = Marshal.AllocCoTaskMem((int)fontdata.Length);

            Marshal.Copy(fontdata, 0, memory, (int)fontdata.Length);

            uint cFonts = 0;

            AddFontMemResourceEx(memory, (uint)fontdata.Length, IntPtr.Zero, ref cFonts);

            Sarpanch_Regular.AddMemoryFont(memory, fontdata.Length);

            Font = new Font(Sarpanch_Regular.Families[0], Font.Size, style, GraphicsUnit.Point);
        }

        private void ApplySidebarFont(bool diffbutton, string Text)
        {
            foreach (Button b in SidebarButtons)
            {
                b.Font = new Font(Sarpanch_Regular.Families[0], 11f, style, GraphicsUnit.Point);
                b.BackgroundImage = Properties.Resources.Deselected_Button;

                if (b == SidebarButtons[0] && !diffbutton)
                {
                    b.Font = new Font(Sarpanch_Regular.Families[0], 16f, style, GraphicsUnit.Point);
                    b.BackgroundImage = Properties.Resources.Selected_Button;
                }

                if (diffbutton && b.Text == Text)
                {
                    b.Font = new Font(Sarpanch_Regular.Families[0], 16f, style, GraphicsUnit.Point);
                    b.BackgroundImage = Properties.Resources.Selected_Button;
                }
                    
            }


            /*
             * foreach (CheckBox cb in BiomeCheckboxes)
             *  cb.Font = new Font(Sarpanch_Regular.Families[0], 9.5f, FontStyle.Regular, GraphicsUnit.Point);
            */
        }

        private void InitializePanels()
        {
            InitializeGeneral();
            InitializeBiomes();
            InitializeAuras();
            InitializeWebhook();
            InitializeInfo();
            InitializeLogs();
            InitializeStats();
            InitializeAccounts();
            InitializeUI();
        }

        private void DrawTitlebar()
        {
            Panel Titlebar = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Theme_Backcolor,
            };

            Panel Titlebar_Seperator = new Panel()
            {
                Dock = DockStyle.Bottom,
                BackColor = Theme_Seperator,
                Height = 1,
            };

            Seperators.Add(Titlebar_Seperator);

            Button ExitButton = new Button()
            {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Red, MouseDownBackColor = Color.Red },
                Size = new Size(40, Titlebar.Height - 1),
                ForeColor = Theme_Forecolor,
                Text = "\u2715",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Dock = DockStyle.Right
            };

            Button MinimizeButton = new Button()
            {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(160, 160, 160), MouseDownBackColor = Color.FromArgb(160, 160, 160) },
                Size = new Size(40, Titlebar.Height - 1),
                ForeColor = Theme_Forecolor,
                Text = "\u0C7C",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Dock = DockStyle.Right
            };

            Label Ver = new Label()
            {
                ForeColor = Theme_Forecolor,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Text = $"{_Version}",
                Size = new Size(Titlebar.Left, Titlebar.Height - 1),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(Titlebar.Left, Titlebar.Height - 25)
            };

            Titlebar.MouseDown += Drag;

            ExitButton.Click += delegate { Close(); };
            MinimizeButton.Click += delegate { WindowState = FormWindowState.Minimized; };

            Controls.Add(Titlebar);

            Titlebar.Controls.Add(MinimizeButton);
            Titlebar.Controls.Add(ExitButton);
            Titlebar.Controls.Add(Titlebar_Seperator);
            Titlebar.Controls.Add(Ver);

            Titlebar.SendToBack();
        }

        private void Drag(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
            }
        }

        private void DrawSidebar()
        {
            Panel Sidebar = new Panel()
            {
                Dock = DockStyle.Left,
                BackColor = Theme_Backcolor,
                Width = 160
            };

            Controls.Add(Sidebar);

            Panel Sidebar_Seperator = new Panel()
            {
                Dock = DockStyle.Right,
                BackColor = Theme_Seperator,
                Width = 1
            };

            Seperators.Add(Sidebar_Seperator);

            Size ButtonSize = new Size(Sidebar.ClientSize.Width, 40);

            int NextY = 1;

            string[] ButtonText = { "General", "Biomes", "Auras", "Webhook", "Info", "Logs", "Stats", "Accounts", "UI" };

            for (int i = 0; i < ButtonText.Length; i++)
            {
                CreateSidebarbuttons(Sidebar, ButtonText[i], NextY);

                NextY += 41;
            }

            SidebarButtons[0].BackgroundImage = Properties.Resources.Selected_Button;


            Sidebar.Controls.Add(Sidebar_Seperator);

            Sidebar_Seperator.BringToFront();
            Sidebar.SendToBack();
        }

        private void CreateSidebarbuttons(Panel Sidebar, string Text, int Y)
        {
            Button b = new Button()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Text = Text,
                Size = new Size(Sidebar.ClientSize.Width, 40),
                ForeColor = Theme_Forecolor,
                Location = new Point(0, Y),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(25, 150, 150, 150), MouseDownBackColor = Color.FromArgb(25, 150, 150, 150) },
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Tag = Text.ToLower()
            };

            b.Click += HandleSelectedSidebarButton;

            RoundCorners(b, 2);

            Sidebar.Controls.Add(b);

            SidebarButtons.Add(b);

            // return b;
        }

        private void HandleSelectedSidebarButton(object sender, EventArgs e)
        {
            Panel selectedpanel = new Panel();
            Button selectedbutton = (Button)sender;


            int idx = SidebarButtons.IndexOf(selectedbutton);

            // if (idx < 0) return; Will implement a better way than try/catch in the future.

            try
            {
                selectedpanel = Panels[idx];
            }
            catch
            {
                return;
            }


            foreach (Panel panel in Panels)
            {
                if (panel != selectedpanel)
                    panel.Hide();
            }

            selectedpanel.Show();

            /*foreach (Button button in SidebarButtons)
            {
                if (selectedbutton != button)
                {
                    button.BackgroundImage = Properties.Resources.Deselected_Button;
                    button.Font = new Font(Sarpanch_Regular.Families[0], 11f, style, GraphicsUnit.Point);
                }
            }
            */

            ApplySidebarFont(true, selectedbutton.Text);
        }

        private void InitializeGeneral()
        {
            Panel General = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            Panels.Add(General);
            Controls.Add(General);

            PictureBox StatusBox = new PictureBox()
            {
                Image = Properties.Resources.Base,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(250, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
            };

            Label StatusLabel = new Label()
            {
                ForeColor = Theme_Forecolor,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Status",
                BackColor = Color.Transparent
            };

            Label ClickMe = new Label()
            {
                ForeColor = Theme_Forecolor,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Click me to Toggle the Macro!",
                AutoSize = true
            };

            CheckBox AntiAFK = new CheckBox()
            {
                ForeColor = Theme_Forecolor,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Text = "AntiAFK",
                AutoSize = true
            };

            AntiAFK.CheckedChanged += delegate
            {
                set.RegAntiAfk(AntiAFK.Checked);
               
            };

            CheckBox AllowMultipleInstances = new CheckBox()
            {
                ForeColor = Theme_Forecolor,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Text = "Allow Multiple Instances",
                AutoSize = true
            };

            AllowMultipleInstances.CheckedChanged += delegate // Todo: Add AllowMultipleInstances (prob never coming at my update rate) Expect it by 2028
            {

            };

            TimeSpan time = SessionTime.Elapsed;
            string fmt = $"{(int)time.TotalDays:D2}:{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";

            if (Uptime is null)
            {
                Uptime = new Label()
                {
                    ForeColor = Theme_Forecolor,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                    Text = $"Uptime: {fmt}",
                    AutoSize = true
                };
            }

            StatusBox.Click += (sender, e) =>
            {
                PictureBox box = sender as PictureBox;

                PictureRunningState = !PictureRunningState;

                set.SetRunning(PictureRunningState);

                box.Image = PictureRunningState ? Properties.Resources.Enabled : Properties.Resources.Base;
            };

            General.Controls.Add(StatusBox);
            // General.Controls.Add(AllowMultipleInstances);
            General.Controls.Add(AntiAFK);
            General.Controls.Add(Uptime);
            

            StatusBox.Controls.Add(StatusLabel);
            StatusBox.Controls.Add(ClickMe);

            // Locations n shit

            int Padding = 10;
            int StartX = 20;

            StatusBox.Location = new Point(General.Right - StatusBox.ClientSize.Width - Padding, General.Bottom - StatusBox.ClientSize.Height + 5);

            // AllowMultipleInstances.Location = new Point(StartX, General.Top + AllowMultipleInstances.Height);4

            AntiAFK.Location = new Point(StartX, AllowMultipleInstances.Bottom + AntiAFK.Height);

            Uptime.Location = new Point(StartX, General.Bottom - Uptime.ClientSize.Height);

            StatusLabel.Location = new Point((StatusBox.ClientSize.Width - StatusLabel.Width) / 2 - 10, (StatusBox.ClientSize.Height - StatusLabel.Height) / 2 - 10);

            ClickMe.Location = new Point((StatusBox.ClientSize.Width - ClickMe.Width) / 2 - 14, (StatusBox.ClientSize.Height - ClickMe.Height * -4) / 2 - 10); // bs

            ClickMe.Click += (_, _) =>
            {
                StatusClicked(StatusBox, null!);
            };

            StatusLabel.Click += (_, _) =>
            {
                StatusClicked(StatusBox, null!);
            };
        }

        private void StatusClicked(object sender, EventArgs e)
        {
            PictureBox box = sender as PictureBox;

            PictureRunningState = !PictureRunningState;

            set.SetRunning(PictureRunningState);

            box.Image = PictureRunningState ? Properties.Resources.Enabled : Properties.Resources.Base;
        }

        public async Task UpdateUptime(CancellationToken ct)
        {
            TimeSpan time;
            string fmt;

            SessionTime.Restart();
            SessionTotalCounter[0].Text = "0";
            SessionTotalCounter[1].Text = "0";
            SessionTotalCounter[2].Text = "0";

            while (!ct.IsCancellationRequested)
            {
                time = SessionTime.Elapsed;

                fmt = $"{(int)time.TotalDays:D2}:{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";

                Uptime.Text = $"Uptime: {fmt}";

                await Task.Delay(1000);
            }

            if (SessionTime.IsRunning)
                SessionTime.Stop();

        }

        public TimeSpan GetSessionTime()
        {
            return SessionTime.Elapsed;
        }

        private void InitializeBiomes()
        {
            Panel Biomes = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            int NextX = 10; // Pos X
            int NextY = 55; // Pos Y

            string[] splitby = { "Sand Storm", "Null", "Glitched" }; // Common, Uncommon, Rare after that follows event

            // bool containsevent = true;

            for (int i = 0; i < CurrentBiomes.Length; i++)
            {
                CreateBiomeCheckbox(Biomes, CurrentBiomes[i], NextX, NextY, Colors[i]);

                NextX += 112;

                if (CurrentBiomes[i] == splitby[0] || CurrentBiomes[i] == splitby[1] || CurrentBiomes[i] == splitby[2])
                {
                    NextX = 10;
                    NextY += 125;
                }
            }

            CreateBiomeBox(Biomes, Properties.Resources.Common_Biomes, "Normal", "Sand Storm");
            CreateBiomeBox(Biomes, Properties.Resources.Uncommon_Biomes, "Hell", "Null");
            CreateBiomeBox(Biomes, Properties.Resources.Rare_Biomes, "Singularity", "Glitched");

            Panels.Add(Biomes);
            Controls.Add(Biomes);
        }

        private CheckBox CreateBiomeCheckbox(Panel Biomes, string Text, int X, int Y, Color ForeColor)
        {
            CheckBox cb = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                Location = new Point(X, Y),
                Text = Text,
                AutoSize = true,
                ForeColor = ForeColor,
                Checked = false,
            };

            switch (Text) // auto check rare biomes
            {
                case "Cyberspace":
                    CheckBoxHandle(cb);
                    break;
                case "Dreamspace":
                    CheckBoxHandle(cb);
                    break;
                case "Glitched":
                    CheckBoxHandle(cb);
                    break;
                case "Radiant Forest":
                    CheckBoxHandle(cb);
                    break;
            }

            cb.CheckedChanged += delegate { SaveSettings(); };

            if (!cb.Checked)
                cb.Checked = true;

            Biomes.Controls.Add(cb);
            BiomeCheckboxes.Add(cb);

            return cb;
        }

        private void CheckBoxHandle(CheckBox cb)
        {
            cb.Tag = "Rare";
            cb.AutoCheck = false;
            cb.Checked = true;
        }

        private void CreateBiomeBox(Panel Biomes, Image img, string splitfirst, string splitlast)
        {
            CheckBox first = BiomeCheckboxes.Find(cb => cb.Text == splitfirst);
            CheckBox last = BiomeCheckboxes.Find(cb => cb.Text == splitlast);

            if (first == null || last == null)
                return;

            int left = 0;
            int top = first.Top - 35;

            int right = Biomes.ClientSize.Width;
            int bottom = last.Bottom + 35;

            PictureBox box = new PictureBox()
            {
                Image = img,
                Location = new Point(left, top),
                Size = new Size(right - left, bottom - top),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Biomes.Controls.Add(box);
            box.SendToBack();
        }

        private void InitializeAuras()
        {
            Panel Auras = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            int StartX = 20;
            int StartY = 30;
            int Padding = 40;

            CheckBox AuraNotifications = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, StartY),
                Text = "Aura Notifications",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.AuraNotifications
            };

            AuraNotifications.CheckedChanged += delegate
            {
                Auras_Notifications = AuraNotifications.Checked;
                SaveSettings();
            };

            Label lMinPingRarity = new Label()
            {
                ForeColor = Theme_Forecolor,
                AutoSize = true,
                Location = new Point(StartX, StartY + Padding),
                Text = "Min. Ping Rarity"
            };

            TextBox MinPingRarity = new TextBox()
            {
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(170, 30),
                Location = new Point(StartX + lMinPingRarity.ClientSize.Width + 20, StartY + Padding - 3),
                Text = Settings.Data.MinAuraRarity.ToString(),
            };

            Label InvalidInput = new Label()
            {
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                Text = "! Invalid Input", // rlly like this, displayes !invalid input if u enter a string that cant be parsed.
                Location = new Point(MinPingRarity.Location.X + MinPingRarity.ClientSize.Width, lMinPingRarity.Location.Y),
            };

            InvalidInput.Hide();

            MinPingRarity.TextChanged += delegate
            {
                string text = MinPingRarity.Text;

                text = text.Replace(",", "");
                text = text.Replace(".", "");
                text = text.Replace("_", "");

                bool success = long.TryParse(text, out long result);

                if (!success)
                {
                    InvalidInput.Show();
                }
                else
                {
                    InvalidInput.Hide();
                    Auras_MinPingRarity = result;
                    SaveSettings();
                }

                if (text == string.Empty)// just so if you delete ur input, it won't show invalid input and just accept that its empty now
                {
                    InvalidInput.Hide();
                    Auras_MinPingRarity = -1;
                    SaveSettings();
                }
            };

            CheckBox PingUser = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, StartY + 3 * Padding),
                Text = "Ping User",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.AuraPingUserID
            };

            PingUser.CheckedChanged += delegate 
            {
                SaveSettings();
            };

            Label lPingUserID = new Label()
            {
                ForeColor = Theme_Forecolor,
                AutoSize = true,
                Location = new Point(StartX, StartY - 3 + Padding * 3 + Padding),
                Text = "Discord User ID"
            };

            TextBox PingUserID = new TextBox()
            {
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(170, 30),
                Location = new Point(StartX + lPingUserID.ClientSize.Width + 20, lPingUserID.Location.Y),
                Text = Settings.Data.AuraUserID.ToString()
            };

            Label InvalidInput2 = new Label()
            {
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                Text = "! Invalid Input",
                Location = new Point(PingUserID.Location.X + PingUserID.ClientSize.Width, PingUserID.Location.Y),
            };

            InvalidInput2.Hide();

            PingUserID.TextChanged += delegate
            {
                string text = PingUserID.Text;

                text = text.Replace(",", "");
                text = text.Replace(".", "");
                text = text.Replace("_", "");
                
                bool success = long.TryParse(text, out long result);


                if (!success)
                {
                    InvalidInput2.Show();
                }
                else
                {
                    InvalidInput2.Hide();
                    Auras_NotificationID = result;
                    SaveSettings();
                }

                if (text == string.Empty)
                {
                    InvalidInput2.Hide();
                    Auras_NotificationID = -1;
                    SaveSettings();
                }
            };

            cAuraNotifications = AuraNotifications;
            tMinAuraRarity = MinPingRarity;
            cAuraPingUser = PingUser;
            tAuraUserId = PingUserID;

            Auras.Controls.Add(lMinPingRarity);
            Auras.Controls.Add(AuraNotifications);
            Auras.Controls.Add(MinPingRarity);
            Auras.Controls.Add(InvalidInput);
            Auras.Controls.Add(PingUser);
            Auras.Controls.Add(PingUserID);
            Auras.Controls.Add(InvalidInput2);
            Auras.Controls.Add(lPingUserID);

            Panels.Add(Auras);
            Controls.Add(Auras);
        }

        private void InitializeWebhook()
        {
            Panel Webhook = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
            };

            Panels.Add(Webhook);
            Controls.Add(Webhook);

            int StartX = 20;
            int StartY = 25;
            int MinPadding = 35; // Good thought, wrong implementation.
            // int MaxPadding = 100;

            int BottomY = Webhook.Bottom;

            Label Inform = new Label()
            {
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX + 20 - StartY - 10),
                AutoSize = true,
                Text = "* Only pings for rare biomes"
            };

            CheckBox PingRoleID = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, StartY),
                Text = "Ping Role ID",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.PingRole
            };

            TextBox tPingRoleID = new TextBox()
            {
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(170, 30),
                Location = new Point(PingRoleID.ClientSize.Width + StartX + 20, PingRoleID.Location.Y),
                Text = Settings.Data.PingRoleID
            };

            CheckBox PingUserID = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, StartY + MinPadding),
                Text = "Ping User ID",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.PingUserID
            };

            TextBox tPingUserID = new TextBox()
            {
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(170, 30),
                Location = new Point(PingRoleID.ClientSize.Width + StartX + 20, PingUserID.Location.Y),
                Text = Settings.Data.PingUserIDValue
            };

            CheckBox PingEveryone = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, StartY + MinPadding * 2),
                Text = "Ping Everyone",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.PingEveryone
            };

            CheckBox PingNoOne = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, StartY + MinPadding * 3), // like what is padding * 3
                Text = "Don't Ping",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.DontPing
            };

            CheckBox TreatSingularityAsRare = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, BottomY - 95),
                Text = "Treat Singularity as a Rare Biome",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.TreatSingularityAsRare
            };

            TreatSingularityAsRare.CheckedChanged += delegate
            {
                BiomeCheckboxes[10].Tag = TreatSingularityAsRare.Checked ? "Rare" : null;
                SaveSettings();
            };

            CheckBox IncludeUsername = new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                AutoSize = true,
                Location = new Point(StartX, BottomY - 65), // works.
                Text = "Include Roblox Username",
                ForeColor = Theme_Forecolor,
                Checked = Settings.Data.IncludeUsername
            };

            IncludeUsername.CheckedChanged += delegate
            {
                SaveSettings();
            };

            Button AddWebhok = new Button()
            {
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Text = "Add Webhook",
                Size = new Size(150, 40),
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX, StartY + MinPadding * 5),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {BorderSize = 0}
            };

            Button ViewWebhooks = new Button()
            {
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Text = "View Webhooks",
                Size = new Size(150, 40),
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX, AddWebhok.Bottom + 50),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };

            AddWebhok.Click += (sender, e) => MiniWebhook();
            ViewWebhooks.Click += (sender, e) => MiniViewWebhook();

            tPingRoleId = tPingRoleID;
            tPingUserIdValue = tPingUserID;
            cSingularityRare = TreatSingularityAsRare;
            cIncludeUsername = IncludeUsername;

            Webhook.Controls.Add(Inform);
            Webhook.Controls.Add(PingRoleID);
            Webhook.Controls.Add(tPingRoleID);
            Webhook.Controls.Add(PingUserID);
            Webhook.Controls.Add(tPingUserID);
            Webhook.Controls.Add(PingEveryone);
            Webhook.Controls.Add(PingNoOne);
            Webhook.Controls.Add(IncludeUsername);
            Webhook.Controls.Add(AddWebhok);
            Webhook.Controls.Add(ViewWebhooks);
            Webhook.Controls.Add(TreatSingularityAsRare);

            PingRoleID.CheckStateChanged += HandleSelectedCheckBox;
            PingUserID.CheckStateChanged += HandleSelectedCheckBox;
            PingEveryone.CheckStateChanged += HandleSelectedCheckBox;
            PingNoOne.CheckStateChanged += HandleSelectedCheckBox;

            PingCheckBoxes.Add(PingRoleID);
            PingCheckBoxes.Add(PingUserID);
            PingCheckBoxes.Add(PingEveryone);
            PingCheckBoxes.Add(PingNoOne);

            tPingRoleID.TextChanged += delegate
            {
                SaveSettings();
            };

            tPingUserID.TextChanged += delegate
            {
                SaveSettings();
            };
        }

        private void MiniWebhook(int editing = -1)
        {
            Panel AddWebhook = new Panel()
            {
                BackColor = Theme_Backcolor,
                Dock = DockStyle.Fill,
                Size = PanelSize
            };

            Controls.Add(AddWebhook);

            TextBox Link = new TextBox()
            {
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(480, 25),
                Location = new Point(20, 30),
                Text = editing == -1 ? "" : Webhooks.Get(editing)
            };

            Button Apply = new Button()
            {
                Text = "Apply",
                Size = new Size(150, 30),
                Location = new Point(20, 90)
            };

            Apply.Click += delegate
            {
                string webhook = Link.Text.Trim();

                if (string.IsNullOrWhiteSpace(webhook))
                {
                    MessageBox.Show("Invalid Webhook Link!", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Error); // type valid links!!!!!!!
                    return;
                }

                if (Webhook.Webhooks.Contains(webhook) && (editing == -1 || Webhooks.Get(editing) != webhook))
                {
                    MessageBox.Show("This webhook is already added.", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (editing == -1)
                    Webhooks.Add(webhook);
                else
                    Webhooks.Update(editing, webhook);

                AddWebhook.Dispose();
            };

            Button Return = new Button()
            {
                Text = "Return",
                Size = new Size(150, 30),
                Location = new Point(20, 350),
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                ForeColor = Theme_Forecolor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
            };

            Return.Click += delegate { AddWebhook.Dispose(); };

            AddWebhook.Controls.Add(Link);
            AddWebhook.Controls.Add(Apply);
            AddWebhook.Controls.Add(Return);
            AddWebhook.BringToFront();
        }

        private void MiniViewWebhook()
        {
            Panel ViewWebhook = new Panel()
            {
                BackColor = Theme_Backcolor,
                Dock = DockStyle.Fill,
                Size = PanelSize
            };

            Controls.Add(ViewWebhook);
            WebhookList = new ListBox();
            Webhooks.Load(WebhookList);
            WebhookList.ForeColor = Theme_Forecolor;
            WebhookList.BackColor = Theme_Backcolor;
            WebhookList.Size = new Size(500, 250);
            WebhookList.Location = new Point(20, 15);

            Button Edit = new Button()
            {
                Text = "Edit",
                Size = new Size(100, 30),
                Location = new Point(20, 280)
            };

            Edit.Click += delegate
            {
                if (WebhookList.SelectedIndex == -1)
                    return;

                int index = WebhookList.SelectedIndex;
                ViewWebhook.Dispose();
                MiniWebhook(index);
            };

            Button Remove = new Button()
            {
                Text = "Remove",
                Size = new Size(100, 30),
                Location = new Point(130, 280)
            };

            Remove.Click += delegate
            {
                if (WebhookList.SelectedIndex == -1)
                    return;

                if (MessageBox.Show("Delete this webhook?", "Vortex", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                Webhooks.Remove(WebhookList.SelectedIndex);
                Webhooks.Load(WebhookList);
            };

            Button Test = new Button()
            {
                Text = "Test Webhook",
                Size = new Size(200, 30),
                Location = new Point(240, 280)
            };

            Test.Click += delegate
            {
                if (WebhookList.SelectedIndex == -1)
                    return;

                Webhook.TestWebhook(Webhook.Webhooks[WebhookList.SelectedIndex]);
            };

            Button Return = new Button()
            {
                Text = "Return",
                Size = new Size(150, 30),
                Location = new Point(20, 350),
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                ForeColor = Theme_Forecolor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
            };

            Return.Click += delegate { ViewWebhook.Dispose(); };

            ViewWebhook.Controls.Add(WebhookList);
            ViewWebhook.Controls.Add(Edit);
            ViewWebhook.Controls.Add(Remove);
            ViewWebhook.Controls.Add(Return);
            ViewWebhook.Controls.Add(Test);
            ViewWebhook.BringToFront();
        }

        private void HandleSelectedCheckBox(object sender, EventArgs e)
        {
            if (LoadingSettings)
                return;

            CheckBox latestselected = sender as CheckBox;

            int totalchecked = 0;

            foreach (CheckBox cb in PingCheckBoxes)
                if (cb.Checked)
                    totalchecked++;

            if (totalchecked > 1)
            {
                foreach (CheckBox cb in PingCheckBoxes)
                    cb.Checked = false;

                latestselected.Checked = true;
            }

            SaveSettings();
        }

        private void InitializeInfo()
        {
            Panel Info = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            Controls.Add(Info);
            Panels.Add(Info);

            Label Credits = new Label
            {
                AutoSize = true,
                ForeColor = Theme_Forecolor,
                BackColor = Color.Transparent,
                Text = "Credits:\n\n\n" + // awesome people 
                "vexk1x (vexk1x) - Developer\n\n" +
                "Fiko (fiko2022) - Macro Tester, Tutorial Video, Assets and ideas\n\n" +
                "2119 (211nine) - Macro Tester, Server Creator\n\n" +
                "ultra (ultranekrosma) - Lead Macro Tester, ideas\n\n" +
                "Raiko (ggraiko) - Macro Tester\n\n" +
                "dxv (idkdxv) - Macro Tester, ideas\n\n" +
                "Koni (koninosk) - Macro Tester\n\n" +
                "Lacrimosa - Macro Tester, Feedback, ideas",
                Location = new Point(20, 30),
            };

            LinkLabel DiscordServer = new LinkLabel() // cool ppl hangout
            {
                Text = "Join the Discord Server",
                LinkColor = Theme_LinkLabelColor,
                Location = new Point(20, 370),
                AutoSize = true, 
            };

            LinkLabel TutorialVideo = new LinkLabel() // Thanks to Fiko (again)
            {
                Text = "Watch the Tutorial Video (Thanks to Fiko)",
                LinkColor = Theme_LinkLabelColor,
                Location = new Point(20, DiscordServer.Bottom),
                AutoSize = true,
            };

            DiscordServer.LinkClicked += delegate
            {
                LinkLabelClicked("https://discord.gg/gSbuSNuAAW");
            };

            TutorialVideo.LinkClicked += delegate
            {
                LinkLabelClicked("https://www.youtube.com/watch?v=d-MhfJgXGq0&t=1s");
            };

            Info.Controls.Add(Credits);
            Info.Controls.Add(DiscordServer);
            Info.Controls.Add(TutorialVideo);
        }

        private void LinkLabelClicked(string Link)
        {
            Process.Start
            (
                new ProcessStartInfo(Link)

                { UseShellExecute = true }
            );
        }

        private void InitializeLogs()
        {
            Panel Logs = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            Controls.Add(Logs);
            Panels.Add(Logs);

            logs = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                ForeColor = Theme_Forecolor,
                ReadOnly = true,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = false,

            };

            Logs.Controls.Add(logs);

            foreach (var entry in PendingLogs) // needed otherwise the logs from settings won't show due to logs being initialized after settings are loaded.
                PrintLogs(entry.Item1, entry.Item2);

            PendingLogs.Clear();
        }

        private void InitializeStats()
        {
            Panel Stats = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            Controls.Add(Stats);
            Panels.Add(Stats);

            int StartX = 20;
            int StartY = 15;

            foreach (CheckBox cb in BiomeCheckboxes)
            {
                var Label = StatsLabel(cb.Text, cb.ForeColor);

                Label.Item1.Location = new Point(StartX, StartY);
                Label.Item2.Location = new Point(Label.Item1.Right + 10, StartY);

                StartY += 30;

                if (cb.Text.ToUpper() == "NULL")
                {
                    StartX = 250;
                    StartY = 15;
                }

                Stats.Controls.Add(Label.Item1);
                Stats.Controls.Add(Label.Item2);

                StatsCounter.Add(Label.Item2);
                StatsCL.Add(Label.Item1);
            }

            string[] LText = { "Biomes found this Session: ", "Rare Biomes found this Session: ", "Globals Rolled this Session: ", "Total Globals Rolled: ", "Total Biomes found: ", "Total Rare Biomes found: " };

            int PosX = 20;
            int PosY = 350;

            for (int i = 0; i < LText.Length; i++)
            {
                var Result = StatsLabel(LText[i], Theme_Forecolor);

                Result.Item1.Location = new Point(PosX, PosY);
                Result.Item2.Location = new Point(Result.Item1.Right + Result.Item2.ClientSize.Width + 20, PosY);

                PosY += 25;

                Stats.Controls.Add(Result.Item1);
                Stats.Controls.Add(Result.Item2);

                SessionTotalCounter.Add(Result.Item2);

                if (i % 2 != 0)
                {
                    PosX += 275;
                    PosY = 350;
                }

                if (i == 3)
                {
                    PosX = 20;
                    PosY = 425;
                }
            }
        }

        private (Label, Label) StatsLabel(string Text, Color TextColor)
        {
            Label StatLabel = new Label()
            {
                ForeColor = TextColor,
                Text = $"{Text}",
                AutoSize = true
            };

            Label Counter = new Label()
            {
                ForeColor = Theme_Forecolor,
                Text = "0",
                AutoSize = true
            };

            return (StatLabel, Counter);
        }
       

        private void InitializeAccounts()
        {
            Panel Accounts = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            Controls.Add(Accounts);
            Panels.Add(Accounts);

            int StartX = 20;
            int StartY = 15;

            Button AddAccounts = new Button()
            {
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Text = "Add Account",
                Size = new Size(150, 40),
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX, StartY),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };

            AddAccounts.Click += (sender, e) => MiniAddAccounts();

            Button ViewAccounts = new Button()
            {
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Text = "View Accounts",
                Size = new Size(150, 40),
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX, StartY + AddAccounts.Bottom),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0}
            };

            ViewAccounts.Click += (sender, e) => MiniViewAccount();

            AccountsList = new RichTextBox()
            {
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                ReadOnly = true,
                Size = new Size(Accounts.Width, (int)(Accounts.Height / 1.5f)),
                Location = new Point(1, (int)(Accounts.ClientSize.Height / 3))
            };

            Label LabelAccountsList = new Label()
            {
                ForeColor = Theme_Forecolor,
                AutoSize = true,
                Text = "Accounts List: ",
                Location = new Point(AccountsList.Left, AccountsList.Top - 15)
            };

            Accounts.Controls.Add(AddAccounts);
            Accounts.Controls.Add(ViewAccounts);
            Accounts.Controls.Add(AccountsList);
            Accounts.Controls.Add(LabelAccountsList);
        }

        private void MiniAddAccounts(int editing = -1)
        {
            Panel AddAccounts = new Panel()
            {
                BackColor = Theme_Backcolor,
                Dock = DockStyle.Fill,
                Size = PanelSize
            };

            Controls.Add(AddAccounts);

            int StartX = 20;
            int StartY = 15;
            int Padding = 40;

            Label lRobloxUserID = new Label()
            {
                ForeColor = Theme_Forecolor,
                Text = "Roblox User ID:",
                AutoSize = true,
                Location = new Point(StartX, StartY)
            };

            TextBox RobloxUserID = new TextBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                Text = editing == -1 ? "" : AccountData.Get(editing).UserId,
                Location = new Point(lRobloxUserID.Right + 15, StartY),
                Size = new Size(250, 30)
            };
            

            Label lPrivateServer = new Label()
            {
                ForeColor = Theme_Forecolor,
                Text = "Private Server:",
                AutoSize = true,
                Location = new Point(StartX, StartY + RobloxUserID.Height + Padding)
            };

            TextBox PrivateServer = new TextBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                Text = editing == -1 ? "" : AccountData.Get(editing).Link,
                Location = new Point(RobloxUserID.Left, lPrivateServer.Location.Y),
                Size = new Size(250, 25)
            };


            Button Return = new Button()
            {
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Text = "Return",
                Size = new Size(150, 30),
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX, 350),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
            };

            Return.Click += delegate
            {
                AddAccounts.Dispose();
            };

            Button Apply = new Button()
            {
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Text = "Apply",
                Size = new Size(150, 30),
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX, 400),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };

            Apply.Click += async delegate
            {
                if (!AccountData.IsValidUserId(RobloxUserID.Text) || !AccountData.IsValidLink(PrivateServer.Text))
                {
                    MessageBox.Show("Invalid Roblox User ID or Private Server Link!", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int found = AccountData.FindUser(RobloxUserID.Text);

                if (found != -1 && found != editing)
                {
                    MessageBox.Show("Account already exists!", "Vortex", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (editing == -1)
                    AddAccount(long.Parse(RobloxUserID.Text), PrivateServer.Text);
                else
                    AccountData.Update(editing, RobloxUserID.Text, PrivateServer.Text);

                await AccountData.RefreshList(AccountBox);
                RefreshAccountsList();
                AddAccounts.Dispose();
            };

            AddAccounts.Controls.Add(lRobloxUserID);
            AddAccounts.Controls.Add(RobloxUserID);
            AddAccounts.Controls.Add(lPrivateServer);
            AddAccounts.Controls.Add(PrivateServer);
            AddAccounts.Controls.Add(Return);
            AddAccounts.Controls.Add(Apply);

            AddAccounts.BringToFront();
        }

        private void AddAccount(long UID, string PSL)
        {
            AccountData.Add(UID.ToString(), PSL);
            RefreshAccountsList();
        }

        private void MiniViewAccount()
        {
            Panel ViewAccounts = new Panel()
            {
                BackColor = Theme_Backcolor,
                Dock = DockStyle.Fill,
                Size = PanelSize
            };

            Controls.Add(ViewAccounts);

            int StartX = 20;

            ListBox list = new ListBox()
            {
                ForeColor = Theme_Forecolor,
                BackColor = Theme_Backcolor,
                Size = new Size(500, 250),
                Location = new Point(StartX, 15)
            };

            for (int i = 0; i < AccountData.Servers.Count; i++)
                list.Items.Add(AccountData.GetUsername(AccountData.Servers[i].UserId) + (AccountData.Servers[i].Enabled ? "" : " (Disabled)"));

            Button Edit = new Button()
            {
                Text = "Edit",
                Size = new Size(100, 30),
                Location = new Point(StartX, 280)
            };

            Edit.Click += delegate
            {
                if (list.SelectedIndex == -1)
                    return;

                int index = list.SelectedIndex;
                ViewAccounts.Dispose();
                MiniAddAccounts(index);
            };

            Button Enable = new Button()
            {
                Text = "Enable / Disable",
                Size = new Size(130, 30),
                Location = new Point(130, 280)
            };

            Enable.Click += delegate
            {
                if (list.SelectedIndex == -1)
                    return;

                Accounts.Entry entry = AccountData.Get(list.SelectedIndex);
                entry.Enabled = !entry.Enabled;
                AccountData.Save();
                list.Items[list.SelectedIndex] = AccountData.GetUsername(entry.UserId) + (entry.Enabled ? "" : " (Disabled)");
                RefreshAccountsList();
            };

            Button Remove = new Button()
            {
                Text = "Remove",
                Size = new Size(100, 30),
                Location = new Point(270, 280)
            };

            Remove.Click += delegate
            {
                if (list.SelectedIndex == -1)
                    return;

                int index = list.SelectedIndex;

                if (MessageBox.Show($"Delete the private server for User ID {AccountData.Get(index).UserId}?", "Vortex", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                AccountData.Remove(index);
                list.Items.RemoveAt(index);
                RefreshAccountsList();
            };

            Button Return = new Button()
            {
                BackgroundImage = Properties.Resources.Deselected_Button,
                BackgroundImageLayout = ImageLayout.Stretch,
                Text = "Return",
                Size = new Size(150, 30),
                ForeColor = Theme_Forecolor,
                Location = new Point(StartX, 350),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
            };

            Return.Click += delegate
            {
                ViewAccounts.Dispose();
            };

            ViewAccounts.Controls.Add(list);
            ViewAccounts.Controls.Add(Edit);
            ViewAccounts.Controls.Add(Enable);
            ViewAccounts.Controls.Add(Remove);
            ViewAccounts.Controls.Add(Return);

            ViewAccounts.BringToFront();
        }

        private void InitializeUI()
        {
            Panel UI = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Theme_Backcolor,
                Size = PanelSize
            };

            Controls.Add(UI);
            Panels.Add(UI);

            lbListBox = new ListBox()
            {
                BackColor = Theme_Backcolor,
                ForeColor = Theme_Forecolor,
                Location = new Point(20, 30),
                Size = new Size(200, 200)
            };

            Label ThemesLabel = new Label()
            {
                Text = "Themes: ",
                AutoSize = true,
                ForeColor = Theme_Forecolor,
            };

            ThemesLabel.Location = new Point(lbListBox.Left + (lbListBox.Width - ThemesLabel.ClientSize.Width + 10) / 2, lbListBox.Top - ThemesLabel.ClientSize.Height);

            AddThemes(lbListBox);


            ListBox Font = new ListBox()
            {
                BackColor = Theme_Backcolor,
                ForeColor = Theme_Forecolor,
                Location = new Point(lbListBox.Right + 30, 30),
                Size = new Size(200, 200)
            };

            Label FontsLabel = new Label()
            {
                Text = "Fonts: ",
                AutoSize = true,
                ForeColor = Theme_Forecolor,
            };

            FontsLabel.Location = new Point(Font.Left + (Font.Width - FontsLabel.ClientSize.Width + 10) / 2, Font.Top - FontsLabel.ClientSize.Height);

            AddFonts(Font);

            UI.Controls.Add(lbListBox);
            UI.Controls.Add(ThemesLabel);
            UI.Controls.Add(Font);
            UI.Controls.Add(FontsLabel);
        }

        private void AddThemes(ListBox l)
        {
            l.Items.Add("Classic");
            l.Items.Add("Dark Classic");
            l.Items.Add("Light Classic");
            l.Items.Add("Gray");
            l.Items.Add("Dark Gray");
            l.Items.Add("Cherry Blossom");
            l.Items.Add("Light Blue");

            l.SelectedIndexChanged += delegate
            {
                ThemesIndex = l.SelectedIndex;

                switch (l.SelectedItem)
                {
                    case "Classic":
                        {
                            Theme_Backcolor = Color.FromArgb(17, 19, 24);
                            Theme_Forecolor = Color.White;
                            Theme_Seperator = Color.FromArgb(200, 200, 200);
                            Theme_LinkLabelColor = Color.FromArgb(60, 60, 180);
                            break;
                        }

                    case "Dark Classic":
                        {
                            Theme_Backcolor = Color.FromArgb(5, 6, 5);
                            Theme_Forecolor = Color.LightGray;
                            Theme_Seperator = Color.DarkGray;
                            Theme_LinkLabelColor = Color.AntiqueWhite;
                            break;
                        }

                    case "Light Classic":
                        {
                            Theme_Backcolor = Color.FromArgb(50, 55, 70);
                            Theme_Forecolor = Color.White;
                            Theme_Seperator = Color.FromArgb(220, 220, 220);
                            Theme_LinkLabelColor = Color.FromArgb(80, 200, 80);
                            break;
                        }

                    case "Gray":
                        {
                            Theme_Backcolor = Color.FromArgb(38, 38, 38);
                            Theme_Forecolor = Color.FromArgb(222, 222, 222);
                            Theme_Seperator = Color.FromArgb(72, 72, 72);
                            Theme_LinkLabelColor = Color.FromArgb(60, 60, 180);
                            break;
                        }

                    case "Dark Gray": // personal favourite
                        {
                            Theme_Backcolor = Color.FromArgb(16, 16, 16);
                            Theme_Forecolor = Color.FromArgb(188, 188, 188);
                            Theme_Seperator = Color.FromArgb(85, 85, 85);
                            Theme_LinkLabelColor = Color.FromArgb(60, 60, 180);
                            break;
                        }

                    case "Cherry Blossom":
                        {
                            Theme_Backcolor = Color.FromArgb(237, 202, 255);
                            Theme_Forecolor = Color.FromArgb(88, 75, 87);
                            Theme_Seperator = Color.FromArgb(168, 119, 165);
                            Theme_LinkLabelColor = Color.FromArgb(255, 113, 247);
                            break;
                        }

                    case "Light Blue":
                        {
                            Theme_Backcolor = Color.FromArgb(158, 221, 255);
                            Theme_Forecolor = Color.FromArgb(88, 75, 87);
                            Theme_Seperator = Color.FromArgb(88, 174, 199);
                            Theme_LinkLabelColor = Color.FromArgb(118, 120, 157);
                            break;
                        }
                }

                
                ApplyTheme();
                SaveSettings();
            };

            l.SelectedIndex = ThemesIndex;
        }

        private void ApplyTheme()
        {
            this.BackColor = Theme_Backcolor;
            this.ForeColor = Theme_Forecolor;


            ApplyTheme(this);

            for (int i = 0; i < Colors.Length; i++)
            {
                try
                {
                    BiomeCheckboxes[i].ForeColor = Colors[i];
                    StatsCL[i].ForeColor = Colors[i];
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            foreach (Panel seperator in Seperators)
                seperator.BackColor = Theme_Seperator;
        }

        private void ApplyTheme(Control p)
        {
            foreach (Control c in p.Controls)
            {
                switch (c)
                {
                    case Panel pl:
                        {
                            pl.BackColor = Theme_Backcolor;
                            break;
                        }
                    case LinkLabel ll:
                        {
                            ll.LinkColor = Theme_LinkLabelColor;
                            break;
                        }
                    case Label l:
                        {
                            l.ForeColor = Theme_Forecolor;
                            break;
                        }
                    case Button b:
                        {
                            b.ForeColor = Theme_Forecolor;
                            b.BackColor = Theme_Backcolor;
                            break;
                        }
                    case TextBox t:
                        {
                            t.ForeColor = Theme_Forecolor;
                            t.BackColor = Theme_Backcolor;
                            break;
                        }
                    case RichTextBox rt:
                        {
                            rt.ForeColor = Theme_Forecolor;
                            rt.BackColor = Theme_Backcolor;
                            break;
                        }
                    case CheckBox cb:
                        {
                            c.ForeColor = Theme_Forecolor;

                            break;
                        }
                    case ListBox lb:
                        {
                            lb.BackColor = Theme_Backcolor;
                            lb.ForeColor = Theme_Forecolor;
                            break;
                        }
                }
                ApplyTheme(c);
            }
        }

        private void AddFonts(ListBox l)
        {
            l.Items.Add("Bold");
            l.Items.Add("Regular");
            l.Items.Add("Underline");
            l.Items.Add("Strikeout");
            l.Items.Add("Italic");

            l.SelectedIndexChanged += delegate
            {
                FontIndex = l.SelectedIndex;

                style = FontStyle.Bold;

                switch (l.SelectedItem) // lwk only valid fonts are bold and italic but choose to ur liking
                {
                    case "Bold":
                        style = FontStyle.Bold;
                        break;

                    case "Regular":
                        style = FontStyle.Regular;
                        break;

                    case "Underline":
                        style = FontStyle.Underline | FontStyle.Bold;
                        break;

                    case "Strikeout":
                        style = FontStyle.Strikeout | FontStyle.Bold;
                        break;

                    case "Italic":
                        style = FontStyle.Italic | FontStyle.Bold;
                        break;
                }

                this.Font = new Font(Sarpanch_Regular.Families[0], Font.Size, style, GraphicsUnit.Point);

                ApplySidebarFont(true, "UI");

                SaveSettings();
            };

            l.SelectedIndex = FontIndex;
        }

        private void RoundCorners(Control ctrl, int r) // fucking useless btw
        {
            if (ctrl.Width <= 0 || ctrl.Height <= 0)
                return;

            int d = r * 2;

            int s = Math.Min(d, Math.Min(ctrl.Width, ctrl.Height));

            GraphicsPath gp = new GraphicsPath();

            gp.AddArc(0, 0, s, s, 180, 90);
            gp.AddArc(ctrl.Width - s, 0, s, s, 270, 90);

            gp.AddArc(ctrl.Width - s, ctrl.Height - s, s, s, 0, 90);
            gp.AddArc(0, ctrl.Height - s, s, s, 90, 90);

            gp.CloseFigure();

            Region old = ctrl.Region;

            ctrl.Region = new Region(gp);

            old?.Dispose();

            gp.Dispose();
        }

        public void PrintLogs(string log, Color color)
        {
            if (IsDisposed || Disposing)
                return;

            if (logs == null)
            {
                PendingLogs.Add((log, color));
                return;
            }

            string logline = $"[{DateTime.Now:HH:mm:ss}] {log} {Environment.NewLine}";

            logs.SelectionStart = logs.TextLength;
            logs.SelectionLength = 0;
            logs.SelectionColor = color;

            logs.AppendText(logline);

            logs.SelectionColor = logs.ForeColor;
            logs.ScrollToCaret();
        }

        // Hell
        public bool AccEnabled(string userId)
        {
            int i = AccountData.FindUser(userId);

            return i != -1 && AccountData.Get(i).Enabled;
        }

        public string GetPrivateServerForUser(string userId)
        {
            return AccountData.GetForUser(userId);
        }

        private void RefreshAccountsList()
        {
            AccountsList.Clear();

            foreach (Accounts.Entry entry in AccountData.Servers)
                AccountsList.AppendText(AccountData.GetUsername(entry.UserId) + (entry.Enabled ? "" : " (Disabled)") + Environment.NewLine);
        }

        public bool BiomeSelected(string biome)
        {
            for (int i = 0; i < CurrentBiomes.Length; i++)
                if (CurrentBiomes[i].Equals(biome, StringComparison.OrdinalIgnoreCase))
                    return BiomeCheckboxes[i].Checked;

            return false;
        }

        public bool UpdateBiomeStats(string biome)
        {
            CheckBox cb = GetCheckBox(biome);

            if (cb is null)
                return false;

            bool rare = string.Equals(cb.Tag as string, "Rare", StringComparison.OrdinalIgnoreCase);
            int index = BiomeCheckboxes.IndexOf(cb);

            StatsCounter[index].Text = (int.Parse(StatsCounter[index].Text) + 1).ToString();
            SessionTotalCounter[0].Text = (int.Parse(SessionTotalCounter[0].Text) + 1).ToString();
            SessionTotalCounter[4].Text = (int.Parse(SessionTotalCounter[4].Text) + 1).ToString();

            if (rare)
            {
                SessionTotalCounter[1].Text = (int.Parse(SessionTotalCounter[1].Text) + 1).ToString();
                SessionTotalCounter[5].Text = (int.Parse(SessionTotalCounter[5].Text) + 1).ToString();
            }

            SaveSettings();
            return rare;
        }

        private CheckBox GetCheckBox(string biome)
        {
            return BiomeCheckboxes.Find(cb => cb.Text.Equals(biome, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateAuraStats()
        {
            SessionTotalCounter[2].Text = (int.Parse(SessionTotalCounter[2].Text) + 1).ToString();
            SessionTotalCounter[3].Text = (int.Parse(SessionTotalCounter[3].Text) + 1).ToString();
            SaveSettings();
        }

        // Flaming hell

        private void SaveSettings()
        {
            if (LoadingSettings)
                return;

            Settings.Data.Normal = BiomeCheckboxes[0].Checked;
            Settings.Data.Windy = BiomeCheckboxes[1].Checked;
            Settings.Data.Snowy = BiomeCheckboxes[2].Checked;
            Settings.Data.Rainy = BiomeCheckboxes[3].Checked;
            Settings.Data.SandStorm = BiomeCheckboxes[4].Checked;
            Settings.Data.Hell = BiomeCheckboxes[5].Checked;
            Settings.Data.Starfall = BiomeCheckboxes[6].Checked;
            Settings.Data.Heaven = BiomeCheckboxes[7].Checked;
            Settings.Data.Corruption = BiomeCheckboxes[8].Checked;
            Settings.Data.Null = BiomeCheckboxes[9].Checked;
            Settings.Data.Singularity = BiomeCheckboxes[10].Checked;
            Settings.Data.Cyberspace = BiomeCheckboxes[11].Checked;
            Settings.Data.Dreamspace = BiomeCheckboxes[12].Checked;
            Settings.Data.Glitched = BiomeCheckboxes[13].Checked;
            Settings.Data.LostForest = BiomeCheckboxes[14].Checked;
            Settings.Data.RadiantForest = BiomeCheckboxes[15].Checked;

            Settings.Data.TotalNormal = int.Parse(StatsCounter[0].Text);
            Settings.Data.TotalWindy = int.Parse(StatsCounter[1].Text);
            Settings.Data.TotalSnowy = int.Parse(StatsCounter[2].Text);
            Settings.Data.TotalRainy = int.Parse(StatsCounter[3].Text);
            Settings.Data.TotalSandStorm = int.Parse(StatsCounter[4].Text);
            Settings.Data.TotalHell = int.Parse(StatsCounter[5].Text);
            Settings.Data.TotalStarfall = int.Parse(StatsCounter[6].Text);
            Settings.Data.TotalHeaven = int.Parse(StatsCounter[7].Text);
            Settings.Data.TotalCorruption = int.Parse(StatsCounter[8].Text);
            Settings.Data.TotalNull = int.Parse(StatsCounter[9].Text);
            Settings.Data.TotalSingularity = int.Parse(StatsCounter[10].Text);
            Settings.Data.TotalCyberspace = int.Parse(StatsCounter[11].Text);
            Settings.Data.TotalDreamspace = int.Parse(StatsCounter[12].Text);
            Settings.Data.TotalGlitched = int.Parse(StatsCounter[13].Text);
            Settings.Data.TotalLostForest = int.Parse(StatsCounter[14].Text);
            Settings.Data.TotalRadiantForest = int.Parse(StatsCounter[15].Text);

            Settings.Data.TotalGlobalsRolled = int.Parse(SessionTotalCounter[3].Text);
            Settings.Data.TotalBiomes = int.Parse(SessionTotalCounter[4].Text);
            Settings.Data.TotalRareBiomes = int.Parse(SessionTotalCounter[5].Text);

            Settings.Data.TreatSingularityAsRare = cSingularityRare.Checked;

            Settings.Data.PingRole = PingCheckBoxes[0].Checked;
            Settings.Data.PingUserID = PingCheckBoxes[1].Checked;
            Settings.Data.PingEveryone = PingCheckBoxes[2].Checked;
            Settings.Data.DontPing = PingCheckBoxes[3].Checked;
            Settings.Data.PingRoleID = tPingRoleId.Text;

            Settings.Data.PingUserIDValue = tPingUserIdValue.Text;
            Settings.Data.IncludeUsername = cIncludeUsername.Checked;

            Settings.Data.AuraNotifications = cAuraNotifications.Checked;
            Settings.Data.AuraPingUserID = cAuraPingUser.Checked;

            string rarityText = tMinAuraRarity.Text.Replace(",", "").Replace(".", "").Replace("_", "");

            if (long.TryParse(rarityText, out long rarity))
                Settings.Data.MinAuraRarity = rarity;
            else if (rarityText.Length == 0)
                Settings.Data.MinAuraRarity = 0;

            string userIdText = tAuraUserId.Text.Replace(",", "").Replace(".", "").Replace("_", "");

            if (long.TryParse(userIdText, out long userId))
                Settings.Data.AuraUserID = userId;
            else if (userIdText.Length == 0)
                Settings.Data.AuraUserID = 0;

            Settings.Data.ThemeIndex = ThemesIndex;
            Settings.Data.FontIndex = FontIndex;
            Settings.Save();
        }

        private void LoadSettings()
        {
            BiomeCheckboxes[0].Checked = Settings.Data.Normal;
            BiomeCheckboxes[1].Checked = Settings.Data.Windy;
            BiomeCheckboxes[2].Checked = Settings.Data.Snowy;
            BiomeCheckboxes[3].Checked = Settings.Data.Rainy;
            BiomeCheckboxes[4].Checked = Settings.Data.SandStorm;
            BiomeCheckboxes[5].Checked = Settings.Data.Hell;
            BiomeCheckboxes[6].Checked = Settings.Data.Starfall;
            BiomeCheckboxes[7].Checked = Settings.Data.Heaven;
            BiomeCheckboxes[8].Checked = Settings.Data.Corruption;
            BiomeCheckboxes[9].Checked = Settings.Data.Null;
            BiomeCheckboxes[10].Checked = Settings.Data.Singularity;
            BiomeCheckboxes[11].Checked = Settings.Data.Cyberspace;
            BiomeCheckboxes[12].Checked = Settings.Data.Dreamspace;
            BiomeCheckboxes[13].Checked = Settings.Data.Glitched;
            BiomeCheckboxes[14].Checked = Settings.Data.LostForest;
            BiomeCheckboxes[15].Checked = Settings.Data.RadiantForest;

            StatsCounter[0].Text = Settings.Data.TotalNormal.ToString();
            StatsCounter[1].Text = Settings.Data.TotalWindy.ToString();
            StatsCounter[2].Text = Settings.Data.TotalSnowy.ToString();
            StatsCounter[3].Text = Settings.Data.TotalRainy.ToString();
            StatsCounter[4].Text = Settings.Data.TotalSandStorm.ToString();
            StatsCounter[5].Text = Settings.Data.TotalHell.ToString();
            StatsCounter[6].Text = Settings.Data.TotalStarfall.ToString();
            StatsCounter[7].Text = Settings.Data.TotalHeaven.ToString();
            StatsCounter[8].Text = Settings.Data.TotalCorruption.ToString();
            StatsCounter[9].Text = Settings.Data.TotalNull.ToString();
            StatsCounter[10].Text = Settings.Data.TotalSingularity.ToString();
            StatsCounter[11].Text = Settings.Data.TotalCyberspace.ToString();
            StatsCounter[12].Text = Settings.Data.TotalDreamspace.ToString();
            StatsCounter[13].Text = Settings.Data.TotalGlitched.ToString();
            StatsCounter[14].Text = Settings.Data.TotalLostForest.ToString();
            StatsCounter[15].Text = Settings.Data.TotalRadiantForest.ToString();

            SessionTotalCounter[3].Text = Settings.Data.TotalGlobalsRolled.ToString();
            SessionTotalCounter[4].Text = Settings.Data.TotalBiomes.ToString();
            SessionTotalCounter[5].Text = Settings.Data.TotalRareBiomes.ToString();

            cSingularityRare.Checked = Settings.Data.TreatSingularityAsRare;
            BiomeCheckboxes[10].Tag = Settings.Data.TreatSingularityAsRare ? "Rare" : null;

            PingCheckBoxes[0].Checked = Settings.Data.PingRole;
            PingCheckBoxes[1].Checked = Settings.Data.PingUserID;
            PingCheckBoxes[2].Checked = Settings.Data.PingEveryone;
            PingCheckBoxes[3].Checked = Settings.Data.DontPing;

            tPingRoleId.Text = Settings.Data.PingRoleID;
            tPingUserIdValue.Text = Settings.Data.PingUserIDValue;
            cIncludeUsername.Checked = Settings.Data.IncludeUsername;
            cAuraNotifications.Checked = Settings.Data.AuraNotifications;
            cAuraPingUser.Checked = Settings.Data.AuraPingUserID;

            tMinAuraRarity.Text = Settings.Data.MinAuraRarity.ToString();
            tAuraUserId.Text = Settings.Data.AuraUserID.ToString();

            Auras_MinPingRarity = Settings.Data.MinAuraRarity;
            Auras_NotificationID = Settings.Data.AuraUserID;
            Auras_Notifications = Settings.Data.AuraNotifications;

            ThemesIndex = Settings.Data.ThemeIndex;
            FontIndex = Settings.Data.FontIndex;
        }
    }
}
