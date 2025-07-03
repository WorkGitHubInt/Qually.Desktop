using System.Security;
using System.Windows.Controls;
using System.Windows;

namespace QuallyFlash
{
    public partial class LoginPage : Page, IHavePassword
    {
        public SecureString SecurePassword => PasswordText.SecurePassword;

        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void PasswordText_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordText.SelectAll();
        }

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
    }
}
