using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace BotQually
{
    public partial class App : Application
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings"));
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "Accounts.xml")))
            {
                XmlHelper.SaveAccountsToFile(new System.Collections.Generic.List<Account>());
            }
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "Settings.xml")))
            {
                GlobalSettings.instance = new GlobalSettings();
                XmlHelper.SaveGlobalSettingsToFile(GlobalSettings.instance);
            }
            base.OnStartup(e);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }
    }
}
