using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls;
using System.Linq;

namespace BotQually
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
            InitActions();
            LoginCommand = new RelayParamCommand(async (parameter) => await Login(parameter));
            DeleteAccCommand = new RelayParamCommand((obj) => DeleteAcc(obj as Account));
            ClearAccCommand = new RelayCommand(() => ClearAccs());
            BackCommand = new RelayCommand(() => Back());
        }

        private void Back()
        {
            MainHelper.ShowPage(new MainPage());
        }

        private void InitActions()
        {
            try
            {
                SavedAccounts = XmlHelper.LoadAccountsFromFile();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.LoginErrorLoadFileMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                File.Delete("settings/Accounts.xml");
            }
        }

        private async Task Login(object parameter)
        {
            if (!LoginIsRunning)
            {
                try
                {
                    if (SelectedAccount != null && Account.Login == SelectedAccount.Login && Account.Password == SelectedAccount.Password && Account.Server == SelectedAccount.Server)
                    {
                        Account = SelectedAccount.Copy();
                    } else if (Account.Type == AccountType.Normal)
                    {
                        foreach (var account in SavedAccounts.Where(account => account.Login == Account.Login && account.Password == Account.Password && account.Server == Account.Server && account.Type == AccountType.Normal).Select(account => account))
                        {
                            Account = account.Copy();
                        }
                        Account.Password = ((PasswordBox)parameter).Password;
                        Account.ProxyIP = ProxyIP;
                        Account.ProxyLogin = ProxyLogin;
                        Account.ProxyPassword = ProxyPassword;
                    }
                    SettingsInit();
                    WebProxy proxy = ProxyInit();
                    bool answer = false;
                    await RunCommandAsync(() => LoginIsRunning, async () =>
                    {
                        if (Account.Type == AccountType.Normal)
                        {
                            LoginIsRunning2 = false;
                            answer = await Account.LogIn(true);
                        }
                        else
                        {
                            LoginIsRunning2 = false;
                            string login = Account.LoginCo;
                            string loginCo = Account.Login;
                            Account.Login = login;
                            answer = await Account.LogIn(true);
                            answer = await Account.LogInCo(loginCo, true);
                            Account.Login = loginCo;
                            Account.LoginCo = login;
                            if (!answer)
                            {
                                MessageBox.Show(Properties.Resources.LoginErrorLoginCoMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        Account.AddSavedFarms();
                    });
                    if (answer)
                    {
                        if (RememberAcc)
                        {
                            AddLogin();
                        }
                        else
                        {
                            XmlHelper.SaveAccountsToFile(SavedAccounts);
                        }
                        MainViewModel.GetInstance().Accounts.Add(Account);
                        MainViewModel.GetInstance().SelectedAccount = Account;
                        MainHelper.ShowPage(new MainPage());
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.LoginErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.LoginErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                finally
                {
                    LoginIsRunning2 = true;
                }
            }
        }

        private void SettingsInit()
        {
            var settings = GlobalSettings.GetInstance();
            if (settings.WorkType == WorkType.GlobalOrder || settings.WorkType == WorkType.GlobalParallel)
            {
                Account.Settings = settings.Settings;
            }
            else
            {
                Account.Settings = Account.PrivateSettings;
            }
        }

        private WebProxy ProxyInit()
        {
            WebProxy proxy = null;
            if (ProxyIP != string.Empty)
            {
                proxy = new WebProxy($"http://{ProxyIP}/", true);
                if (ProxyLogin != string.Empty && ProxyPassword != string.Empty)
                {
                    proxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPassword);
                }
            }
            return proxy;
        }

        private void AddLogin()
        {
            SavedAccounts.Add(Account);
            XmlHelper.SaveAccountsToFile(SavedAccounts);
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
