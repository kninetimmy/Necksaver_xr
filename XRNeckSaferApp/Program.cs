using System;
using System.Threading;
using System.Windows.Forms;

namespace XRNeckSafer
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(false, "XRNeckSafer singleton application"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                if (isAnotherInstanceOpen)
                {
                    MessageBox.Show("VRNS already running!", "XRNeckSafer - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // main application entry point
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SplashScreen());
                KeyInterceptor.SetHook();
                using (new ActionPropertyProcessor(Config.Instance.ActionProperties))
                {
                    Application.Run(new MainForm());
                    Config.Instance.WriteConfig();
                }
                KeyInterceptor.RemoveHook();

                mutex.ReleaseMutex();
            }
        }
    }
}
