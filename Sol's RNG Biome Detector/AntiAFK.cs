using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sol_s_RNG_Biome_Detector
{
    internal class AntiAFK
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint cInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private const uint INPUT_KEYBOARD = 1;

        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        private const ushort SPACE_SCANCODE = 0x39;

        private const ushort VK_ALT = 0x12;

        private const int SW_RESTORE = 9;

        public static int SendActivity()
        {
            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            IntPtr oldWindow = GetForegroundWindow();

            int successCount = 0;

            foreach (Process process in processes)
            {
                try
                {
                    process.Refresh();

                    IntPtr window = process.MainWindowHandle;

                    if (window == IntPtr.Zero)
                        continue;

                    if (!FocusRoblox(window))
                        continue;

                    Thread.Sleep(200);

                    if (GetForegroundWindow() != window)
                        continue;

                    if (!SendSpace(window))
                        continue;

                    successCount++;

                    Thread.Sleep(250);
                }
                catch
                {
                }
            }

            if (oldWindow != IntPtr.Zero)
            {
                Thread.Sleep(100);
                SetForegroundWindow(oldWindow);
            }

            return successCount;
        }

        private static bool FocusRoblox(IntPtr window)
        {
            ShowWindowAsync(window, SW_RESTORE);

            Thread.Sleep(200);

            for (int attempt = 0; attempt < 3; attempt++)
            {
                if (GetForegroundWindow() == window)
                    return true;

                TapAlt();

                Thread.Sleep(50);

                SetForegroundWindow(window);

                for (int i = 0; i < 10; i++)
                {
                    if (GetForegroundWindow() == window)
                        return true;

                    Thread.Sleep(50);
                }
            }

            return false;
        }

        private static bool SendSpace(IntPtr window)
        {
            if (GetForegroundWindow() != window)
                return false;

            INPUT[] down = new INPUT[1];

            down[0].type = INPUT_KEYBOARD;
            down[0].U.ki.wVk = 0;
            down[0].U.ki.wScan = SPACE_SCANCODE;
            down[0].U.ki.dwFlags = KEYEVENTF_SCANCODE;

            uint downResult = SendInput(1, down, Marshal.SizeOf<INPUT>());

            if (downResult != 1)
                return false;

            Thread.Sleep(150);

            INPUT[] up = new INPUT[1];

            up[0].type = INPUT_KEYBOARD;
            up[0].U.ki.wVk = 0;
            up[0].U.ki.wScan = SPACE_SCANCODE;
            up[0].U.ki.dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;

            uint upResult = SendInput(1, up, Marshal.SizeOf<INPUT>());

            return upResult == 1;
        }

        private static void TapAlt()
        {
            INPUT[] inputs = new INPUT[2];

            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = VK_ALT;

            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].U.ki.wVk = VK_ALT;
            inputs[1].U.ki.dwFlags = KEYEVENTF_KEYUP;

            SendInput(2, inputs, Marshal.SizeOf<INPUT>());
        }
    }
}