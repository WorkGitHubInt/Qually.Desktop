using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuallyFlash
{
    public class LoginViewModel : BaseViewModel
    {
        #region Commands
        public ICommand LoginCommand { get; set; }
        public ICommand DeleteAccCommand { get; set; }
        public ICommand ClearAccCommand { get; set; }
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        public Account SelectedAccount { get; set; }
        public List<Account> SavedAccounts { get; set; } = new List<Account>();
        public Account Account { get; set; } = new Account();
        public SecureString Password { get; set; }
        public bool RememberAcc { get; set; }
        public string ProxyIP { get; set; } = string.Empty;
        public string ProxyLogin { get; set; } = string.Empty;
        public string ProxyPassword { get; set; } = string.Empty;
        public bool LoginIsRunning { get; set; }
        public bool LoginIsRunning2 { get; set; } = true;
        #endregion

        public LoginViewModel()
        {
            SavedAccounts = XmlHelper.LoadAccountsFromFile();
            LoginCommand = new RelayParamCommand(async (parameter) => await Login(parameter));
            DeleteAccCommand = new RelayParamCommand((obj) => DeleteAcc(obj as Account));
            ClearAccCommand = new RelayCommand(() => ClearAccs());
            BackCommand = new RelayCommand(() =>
            {
                MainHelper.ShowPage(new MainPage());
            });
        }

        public async Task Login(object parameter)
        {
            if (LoginIsRunning)
            {
                return;
            }
            try
            {
                if (SelectedAccount != null)
                {
                    Account = SelectedAccount;
                }
                if (Account.Type == AccountType.Normal)
                {
                    foreach (Account account in SavedAccounts)
                    {
                        if (account.Login == Account.Login && account.Password == Account.Password && account.
                            Server == Account.Server && account.Type == AccountType.Normal)
                        {
                            Account = account;
                        }
                    }
                    Account.Password = (parameter as IHavePassword).SecurePassword.Unsecure();
                    Account.ProxyIP = ProxyIP;
                    Account.ProxyLogin = ProxyLogin;
                    Account.ProxyPassword = ProxyPassword;
                }
                bool answer = false;
                await RunCommandAsync(() => LoginIsRunning, async () =>
                {
                    if (Account.Type == AccountType.Normal)
                    {
                        LoginIsRunning2 = false;
                        answer = await Account.LogIn();
                    }
                    else
                    {
                        LoginIsRunning2 = false;
                        string login = Account.LoginCo;
                        string loginCo = Account.Login;
                        Account.Login = login;
                        answer = await Account.LogIn();
                        answer = await Account.LogInCo(loginCo, true);
                        Account.Login = loginCo;
                        Account.LoginCo = login;
                        if (!answer)
                        {
                            MessageBox.Show(Properties.Resources.LoginErrorMessage1, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                });
                if (answer)
                {
                    if (RememberAcc)
                    {
                        SavedAccounts.Add(Account);
                    }
                    XmlHelper.SaveAccountsToFile(SavedAccounts);
                    MainViewModel.GetInstance().Account = Account;
                    MainHelper.ShowPage(new MainPage());
                }
                else
                {
                    MessageBox.Show(Properties.Resources.LoginErrorMessage2, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch
            {
                MessageBox.Show(Properties.Resources.LoginErrorMessage2, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                LoginIsRunning2 = true;
            }
        }

        private void DeleteAcc(Account acc)
        {
            SavedAccounts.Remove(acc);
            XmlHelper.SaveAccountsToFile(SavedAccounts);
            SavedAccounts = XmlHelper.LoadAccountsFromFile();
        }

        private void ClearAccs()
        {
            SavedAccounts.Clear();
            XmlHelper.SaveAccountsToFile(SavedAccounts);
            SavedAccounts = XmlHelper.LoadAccountsFromFile();
        }
    }
}
