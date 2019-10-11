using Ninject;
using System.Windows.Controls;

namespace BotQually
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = IoC.Kernel.Get<MainViewModel>();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalSettings globalSettings = (DataContext as MainViewModel).GlobalSettings;
            foreach (var acc in (DataContext as MainViewModel).Accounts)
            {
                if (globalSettings.WorkType == WorkType.GlobalOrder || globalSettings.WorkType == WorkType.GlobalParallel)
                {
                    acc.Settings = globalSettings.Settings;
                }
                else
                {
                    acc.Settings = acc.PrivateSettings;
                }
                acc.SetMainProductToSell();
                acc.SetSubProductToSell();
            }
            XmlHelper.SaveSettingsToFile();
        }

        private void Page_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel).GlobalSettingsOpen = false;
        }

        private void CheckBox_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            XmlHelper.SaveSettingsToFile();
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            XmlHelper.SaveSettingsToFile();
        }
    }
}
