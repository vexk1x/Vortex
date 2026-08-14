using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sol_s_RNG_Biome_Detector
{
    public class HiddenTabControl : TabControl
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328 && !DesignMode)
            {
                m.Result = (IntPtr)1;
                return;
            }

            base.WndProc(ref m);
        }
    }
}