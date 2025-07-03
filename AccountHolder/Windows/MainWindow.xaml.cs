using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AccountHolder
{
    public partial class MainWindow : Window
    {
        private bool loaded = false;

        public MainWindow()
        {
            Settings settings = XmlHelper.LoadSettingsFromFile();
            if (settings.Localization == Localization.en)
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            }
            else
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
            }
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (loaded)
            {
                (DataContext as MainWindowViewModel).ChangeLanguage();
                MessageBox.Show(Properties.Resources.LanguageMessage, "", MessageBoxButton.OK);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }
    }
}
