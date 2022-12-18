using NLog;
using System;
using System.Threading;
using System.Windows.Forms;

namespace XRNeckSafer
{
    static class Program
    {
        private static readonly ILogger _logger = LogManager.GetLogger(nameof(Program));

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (var mutex = new Mutex(false, "XRNeckSafer singleton application"))
                {
                    bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                    if (isAnotherInstanceOpen)
                    {
                        _logger.Debug("XRNS already running");
                        MessageBox.Show("XRNS already running!", "XRNeckSafer - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    _logger.Debug("Application starting.");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    KeyInterceptor.SetHook();
                    JoystickService.Start();
                    using (KeyboardToJoystickService.Instanse)
                    {
                        using (new ActionPropertyProcessor(Config.Instance.ActionProperties))
                        {
                            Application.Run(new MainForm());
                            Config.Instance.WriteConfig();
                        }
                    }
                    KeyInterceptor.RemoveHook();
                    JoystickService.Stop();
                    mutex.ReleaseMutex();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            finally
            {
                _logger.Debug("Application stopped.");
            }
        }
    }
}
