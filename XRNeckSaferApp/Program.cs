using System;
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
            using (var mutex = new System.Threading.Mutex(false, "saebamini.com SingletonApp"))
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
                KeyInterceptor.SetHook();
                Application.Run(new MainForm());
                KeyInterceptor.RemoveHook();

                mutex.ReleaseMutex();
            }

        }
    }
}
