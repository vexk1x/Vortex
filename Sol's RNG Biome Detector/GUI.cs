using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace Sol_s_RNG_Biome_Detector
{
    class GUI
    {
        private readonly Color DarkBackground = Color.FromArgb(17, 19, 24);
        private readonly Color DarkPanel = Color.FromArgb(25, 28, 35);
        private readonly Color DarkInput = Color.FromArgb(32, 36, 45);
        private readonly Color MainText = Color.FromArgb(241, 243, 245);

        private readonly PrivateFontCollection privatefonts = new();
        private IntPtr fontmemory;

        private volatile bool running = false;


        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx
        (IntPtr pbFont, uint cbFont, IntPtr pdv, ref uint pcFonts);


        public void DrawGui(Form form, TabControl tabControl, CheckBox[] biomeCheckboxes, Label[] labels, Panel panelSidebar, Panel panelContent, Button[] sidebarButtons, PictureBox[] tabBoxes, Label[] tabLabels)
        {

            LoadFont();

            ApplyFont(form);

            ApplyDarkTheme(form);

            ApplyCheckboxColor(biomeCheckboxes);
            ApplyLabelColor(labels);

            PageContainer(form, tabControl, panelSidebar);

            StyleSidebar(panelSidebar, sidebarButtons);
            StyleBiomeTab(tabBoxes, tabLabels);

            ApplyRoundedCorners(15, form);

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
                        ApplyButtonStuff(button);
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
        }

        private void ApplyCheckboxColor(CheckBox[] checkBoxes)
        {
            string[] colors =
            {
                "#4e4e4e",
                "#c2f2ff",
                "#b6cbd1",
                "#0000ff",
                "#ffbb00",
                "#770a0a",
                "#3b3abc",
                "#f4fb01",
                "#310387",
                "#000000",
                "#bf6c00",
                "#fffc8f",
            };

            for (int i = 0; i < checkBoxes.Length && i < colors.Length; i++)
            {
                checkBoxes[i].ForeColor = ColorTranslator.FromHtml(colors[i]);
            }
        }

        private void ApplyLabelColor(Label[] labels)
        {
            string[] colors =
            {
                "#4e4e4e",
                "#c2f2ff",
                "#b6cbd1",
                "#0000ff",
                "#ffbb00",
                "#770a0a",
                "#3b3abc",
                "#f4fb01",
                "#310387",
                "#000000",
                "#f1ff00",
                "#bf6c00",
                "#08043f",
                "#e500ff",
                "#212121"
            };

            for (int i = 0; i < labels.Length && i < colors.Length; i++)
            {
                labels[i].ForeColor = ColorTranslator.FromHtml(colors[i]);
            }
        }

        private void PageContainer(Form form, TabControl tabControl, Panel panelSidebar)
        {
            tabControl.Dock = DockStyle.None;

            tabControl.Location = new Point(panelSidebar.Width, 0);

            tabControl.Size = new Size(form.ClientSize.Width - panelSidebar.Width, form.ClientSize.Height);
        }

        private void StyleSidebar(Panel sidebar, Button[] buttons)
        {
            sidebar.BackColor = Color.FromArgb(14, 17, 22);

            int y = 60;

            foreach (Button button in buttons)
            {
                button.Size = new Size(sidebar.Width - 20, 42);
                button.Location = new Point(10, y);

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;

                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(25, 29, 36);

                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 36, 45);

                button.BackColor = sidebar.BackColor;
                button.ForeColor = Color.FromArgb(180, 185, 195);

                button.TextAlign = ContentAlignment.MiddleLeft;
                button.Padding = new Padding(14, 0, 0, 0);

                button.Cursor = Cursors.Hand;

                button.Font = new Font(privatefonts.Families[0], 10F, FontStyle.Regular);

                y += 48;

                buttons[0].Text = "General";
                buttons[1].Text = "Biomes";
                buttons[2].Text = "Auras";
                buttons[3].Text = "Webhook";
                buttons[4].Text = "Info";
                buttons[5].Text = "Logs";
                buttons[6].Text = "Stats";
                buttons[7].Text = "Accounts";


                button.BackgroundImage = Properties.Resources.deselected;
                button.BackgroundImageLayout = ImageLayout.Stretch;
                
            }
        }

        public void SetActiveButton(Button selected, Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                button.BackColor = Color.FromArgb(14, 17, 22);
                button.ForeColor = Color.FromArgb(180, 185, 195);
                button.Font = new Font(privatefonts.Families[0], 10F, FontStyle.Regular);
                button.BackgroundImage = Properties.Resources.deselected;
            }
            
            selected.BackColor = Color.FromArgb(32, 36, 45);
            selected.ForeColor = Color.White;
            selected.Font = new Font(privatefonts.Families[0], 12F, FontStyle.Bold);
            selected.BackgroundImage = Properties.Resources.selected;
        }

        private void ApplyButtonStuff(Button button)
        {
            button.BackColor = Color.FromArgb(14, 17, 22);
            button.ForeColor = Color.White;

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;

            button.Cursor = Cursors.Hand;
            button.BackgroundImage = Properties.Resources.deselected;
            button.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void LoadFont()
        {
            byte[] fontdata = Properties.Resources.Sarpanch_Regular;

            fontmemory = Marshal.AllocCoTaskMem(fontdata.Length);
            Marshal.Copy(fontdata, 0, fontmemory, fontdata.Length);

            privatefonts.AddMemoryFont(fontmemory, fontdata.Length);

            uint added = 0;

            AddFontMemResourceEx(fontmemory, (uint)fontdata.Length, IntPtr.Zero, ref added);
        }

        private void ApplyFont(Control parent)
        {
            parent.Font = new Font(privatefonts.Families[0], parent.Font.Size, FontStyle.Regular, GraphicsUnit.Point);

            if (parent is Label label)
                label.UseCompatibleTextRendering = true;
            if (parent is Button button)
                button.UseCompatibleTextRendering = true; 

            foreach (Control ctrl in parent.Controls)
                ApplyFont(ctrl);
            
        }

        private void StyleBiomeTab(PictureBox[] tabBoxes, Label[] tabLabels)
        {
            // Common
            tabBoxes[0].Image = Properties.Resources.common;
            tabBoxes[1].Image = Properties.Resources.common;
            tabLabels[0].ForeColor = Color.White;

            // Uncommon
            tabBoxes[2].Image = Properties.Resources.uncommon;
            tabBoxes[3].Image = Properties.Resources.uncommon;
            tabLabels[1].ForeColor = Color.DarkSlateBlue;

            // Rare
            tabBoxes[4].Image = Properties.Resources.rare;
            tabBoxes[5].Image = Properties.Resources.rare;
            tabLabels[2].ForeColor = Color.LightGray;

            // Event
            tabBoxes[6].Image = Properties.Resources._event;
            tabBoxes[7].Image = Properties.Resources._event;
            tabLabels[3].ForeColor = Color.LightGreen;


            foreach (PictureBox box in tabBoxes)
                box.SizeMode = PictureBoxSizeMode.StretchImage;

            foreach (Label label in tabLabels)
                label.Font = new Font(privatefonts.Families[0], 12F, FontStyle.Regular);
        }

        private void ApplyRoundedCorners(int radius, Form form)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = new Rectangle(0, 0, form.Width, form.Height);
            int d = radius * 2;

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            form.Region = new Region(path);
        }
    }
}
