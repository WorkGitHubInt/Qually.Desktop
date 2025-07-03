using System.Globalization;
using System.Windows;

namespace AccountHolder
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }
    }
}
