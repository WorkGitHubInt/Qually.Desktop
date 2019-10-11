using System.Windows;
using System.Windows.Input;

namespace QuallyFlash
{
    public class LoginCoViewModel : BaseViewModel
    {
        public ICommand LoginCoCommand { get; set; }
        public ICommand ReturnCommand { get; set; }
        public Account Account { get; set; }
        public string CoAccount { get; set; }
        public bool NotLoading { get; set; } = false;

        public LoginCoViewModel(Account account)
        {
            LoginCoCommand = new RelayCommand(() => LoginCo());
            ReturnCommand = new RelayCommand(() =>
            {
                ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
            });
            Account = account;
            OnLoad();
        }

        private async void OnLoad()
        {
            try
            {
                await Account.LoadCoAccounts();
                NotLoading = true;
            }
            catch
            {
                MessageBox.Show("Ошибка при загрузке соу аккаунтов!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
            }
        }

        private async void LoginCo()
        {
            if (await Account.LogInCo(CoAccount, true))
            {
                ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
            }
            else
            {
                MessageBox.Show("Ошибка входу в соу!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
