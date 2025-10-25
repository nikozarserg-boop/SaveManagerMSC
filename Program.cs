using System;
using System.IO;
using System.Windows.Forms;

namespace SaveManagerMSC
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.ThreadException += (s, e) => LogUnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex) LogUnhandledException(ex);
            };

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                LogUnhandledException(ex);
                throw;
            }
        }

        private static void LogUnhandledException(Exception ex)
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
                string logPath = Path.Combine(dataDir, "programmLog.txt");
                File.AppendAllText(logPath, DateTime.Now.ToString("o") + " [FATAL] " + ex.ToString() + Environment.NewLine);
            }
            catch { }
        }
    }
}
