using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                using (var host = CreateHostBuilder().Build())
                {
                    ServiceProvider = host.Services;
                    KeyInterceptor.SetHook();
                    Application.Run(ServiceProvider.GetRequiredService<MainForm>());
                    KeyInterceptor.RemoveHook();
                }

                mutex.ReleaseMutex();
            }

        }

        private static IServiceProvider ServiceProvider { get; set; }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    services.AddSingleton<VRStuff>();
                    services.AddTransient<MainForm>();
                    services.AddTransient<JoystickKeyboardScanner>();
                    services.AddTransient<JoystickButtonScanner>();
                });
        }
    }
}
