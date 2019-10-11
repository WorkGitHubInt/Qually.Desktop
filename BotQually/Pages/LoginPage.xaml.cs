using System.Security;
using System.Windows.Controls;

namespace BotQually
{
    public partial class LoginPage : Page, IHavePassword
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
            //PasswordText.Password = Properties.Settings.Default.Password;
        }

        public SecureString SecurePassword => PasswordText.SecurePassword;

        private void SavedAccs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SavedAccs.SelectedIndex > -1)
            {
                (DataContext as LoginViewModel).Account.Login = (SavedAccs.SelectedItem as Account).Login;
                (DataContext as LoginViewModel).Account.Password = (SavedAccs.SelectedItem as Account).Password;
                PasswordText.Password = (SavedAccs.SelectedItem as Account).Password;
                (DataContext as LoginViewModel).Account.Server = (SavedAccs.SelectedItem as Account).Server;
                (DataContext as LoginViewModel).ProxyIP = (SavedAccs.SelectedItem as Account).ProxyIP;
                (DataContext as LoginViewModel).ProxyLogin = (SavedAccs.SelectedItem as Account).ProxyLogin;
                (DataContext as LoginViewModel).ProxyPassword = (SavedAccs.SelectedItem as Account).ProxyPassword;
            }
        }

        private void PasswordText_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordText.SelectAll();
        }
    }
}
