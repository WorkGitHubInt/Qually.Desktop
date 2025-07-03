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
                MainHelper.ShowPage(new MainPage());
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
                MessageBox.Show(Properties.Resources.LoginCoErrorMessage1, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainHelper.ShowPage(new MainPage());
            }
        }

        private async void LoginCo()
        {
            NotLoading = false;
            if (await Account.LogInCo(CoAccount, true))
            {
                NotLoading = true;
                MainHelper.ShowPage(new MainPage());
            }
            else
            {
                MessageBox.Show(Properties.Resources.LoginCoErrorMessage2, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
