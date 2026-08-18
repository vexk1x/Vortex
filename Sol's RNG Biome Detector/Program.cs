using System;

namespace Sol_s_RNG_Biome_Detector
{
    internal class Program
    {
        [STAThread]

        static void Main()
        {

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

        }
    }
}
    
