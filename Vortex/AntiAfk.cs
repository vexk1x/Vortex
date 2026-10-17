using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace Vortex
{
    internal class AntiAfk
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
        


        public async Task SendActivity(CancellationToken t)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

            await SendOnce();

            try
            {
                while (await timer.WaitForNextTickAsync(t))
                    await SendOnce();
            }
            catch (OperationCanceledException) when (t.IsCancellationRequested)
            {

            }
        }

        private async Task SendOnce()
        {
            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            IntPtr w = GetForegroundWindow();

            foreach (Process process in processes)
            {
                process.Refresh();

                IntPtr window = process.MainWindowHandle;

                if (window == IntPtr.Zero)
                    continue;

                var Result = FocusRoblox(window);

                if (!Result.Result)
                    continue;

                await Task.Delay(200);

                if (GetForegroundWindow() != window)
                    continue;

                var Result2 = SendSpace(window);

                if (!Result2.Result)
                    continue;

                await Task.Delay(250);
            }

            if (w != IntPtr.Zero)
            {
                await Task.Delay(100);
                SetForegroundWindow(w);
            }
        }

        private async Task<bool> FocusRoblox(IntPtr window)
        {
            ShowWindowAsync(window, SW_RESTORE);

            await Task.Delay(200);

            for (int attempt = 0; attempt < 3; attempt++)
            {
                if (GetForegroundWindow() == window)
                    return true;

                TapAlt();

                await Task.Delay(50);

                SetForegroundWindow(window);

                for (int i = 0; i < 10; i++)
                {
                    if (GetForegroundWindow() == window)
                        return true;

                    await Task.Delay(50);
                }
            }

            return false;
        }

        private async Task<bool> SendSpace(IntPtr window)
        {
            if (GetForegroundWindow() != window)
                return false;

            INPUT[] down = new INPUT[1];

            down[0].type = INPUT_KEYBOARD;
            down[0].U.ki.wVk = 0;
            down[0].U.ki.wScan = SPACE_SCANCODE;
            down[0].U.ki.dwFlags = KEYEVENTF_SCANCODE;

            uint Down = SendInput(1, down, Marshal.SizeOf<INPUT>());

            if (Down != 1)
                return false;

            await Task.Delay(150);

            INPUT[] up = new INPUT[1];

            up[0].type = INPUT_KEYBOARD;
            up[0].U.ki.wVk = 0;
            up[0].U.ki.wScan = SPACE_SCANCODE;
            up[0].U.ki.dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;

            uint Up = SendInput(1, up, Marshal.SizeOf<INPUT>());

            return Up == 1;
        }

        private void TapAlt()
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
