using System;
using System.Diagnostics;

namespace Sol_s_RNG_Biome_Detector
{
    internal class Program
    {
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
    
