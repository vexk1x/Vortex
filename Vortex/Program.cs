using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;

namespace Vortex
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {

            if (args.Length == 3 && args[0] == "--update")
            {
                int pid;

                if (int.TryParse(args[1], out pid))
                {
                    AutoUpdate.ApplyUpdate(pid, args[2]);
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    class Running
    {
        private CancellationTokenSource ctRunningMain;
        private CancellationTokenSource ctAntiAfk;

        private readonly Form1 form;

        public Running(Form1 form)
        {
            this.form = form;
        }

        private async Task RegRunning(CancellationTokenSource token)
        {
            try
            {
                form.PrintLogs("Started", Color.Green);

                List<Task> Tasks = new List<Task>()
                {
                    form.UpdateUptime(token.Token),
                    Detector.Biomes(token.Token, form)
                };

                if (Webhook.Webhooks.Count > 0)
                {
                    await form.Webhooks.StartStopWebhook(true, TimeSpan.Zero, 0, 0);
                }

                await Task.WhenAll(Tasks);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (ctRunningMain == token)
                    ctRunningMain = null;

                if (Webhook.Webhooks.Count > 0)
                {
                    await form.Webhooks.StartStopWebhook(false, form.GetSessionTime(), int.Parse(Form1.SessionTotalCounter[0].Text), int.Parse(Form1.SessionTotalCounter[1].Text));
                }

                form.PrintLogs("Stopped", Color.Red);
                token.Dispose();
            }
        }

        public void SetRunning(bool run)
        {
            if (run)
            {
                if (ctRunningMain != null && !ctRunningMain.IsCancellationRequested)
                    return;

                ctRunningMain = new CancellationTokenSource();
                _ = RegRunning(ctRunningMain);
            }
            else
            {
                if (ctRunningMain != null && !ctRunningMain.IsCancellationRequested)
                {
                    ctRunningMain.Cancel();
                }
                
                if (ctAntiAfk != null && !ctAntiAfk.IsCancellationRequested)
                {
                    ctAntiAfk.Cancel();
                }
            }
        }

        public void RegAntiAfk(bool Checked)
        {
            if (Checked)
            {
                if (ctAntiAfk != null && !ctAntiAfk.IsCancellationRequested)
                    return;

                ctAntiAfk = new CancellationTokenSource();

                AntiAfk afk = new AntiAfk();

                _ = Task.Run(() => afk.SendActivity(ctAntiAfk.Token));

            }
            else
            {
                if (ctAntiAfk != null && !ctAntiAfk.IsCancellationRequested)
                    ctAntiAfk.Cancel();
            }
        }
    }
}
