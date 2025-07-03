using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuallyFlash
{
    public partial class App : Application
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "settings"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "settings");
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "Accounts.xml")))
            {
                XmlHelper.SaveAccountsToFile(new System.Collections.Generic.List<Account>());
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "Settings.xml")))
            {
                XmlHelper.SaveGlobalSettingsToFile(new GlobalSettings());
            }
            base.OnStartup(e);
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (!(e.ExceptionObject is TaskCanceledException) && !(e.ExceptionObject is OperationCanceledException))
                {
                    MessageBox.Show($"Произошла критическая ошибка! Направьте отчет или скриншот администрации! \nMessage: {(e.ExceptionObject as Exception).Message} StackTrace: {(e.ExceptionObject as Exception).StackTrace}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error($"Message: {(e.ExceptionObject as Exception).Message} StackTrace: {(e.ExceptionObject as Exception).StackTrace}");
                    string logError = "";
                    string logMain = "";
                    if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "logError.txt")))
                    {
                        logError = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "logError.txt"), Encoding.GetEncoding("windows-1251"));
                    }
                    if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "logMain.txt")))
                    {
                        logMain = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "logMain.txt"), Encoding.GetEncoding("windows-1251"));
                    }
                }
            }
            catch { }
        }
    }
}
