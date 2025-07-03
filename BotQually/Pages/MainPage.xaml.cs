using System.Windows.Controls;
using System.Windows.Input;

namespace BotQually
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = MainViewModel.GetInstance();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalSettings globalSettings = GlobalSettings.GetInstance();
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
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel).GlobalSettingsOpen = false;
        }
    }
}
