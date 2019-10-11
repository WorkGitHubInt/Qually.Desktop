using Ninject;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;

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
            try
            {
                SavedAccounts = XmlHelper.LoadAccountsFromFile();
            } catch
            {
                MessageBox.Show("Обнаружена проблема с файлом аккаунтов, файл будет перезаписан.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                File.Delete("settings/Accounts.xml");
            }
            //Account.Login = Properties.Settings.Default.Login;
            //Account.Server = (Server)Properties.Settings.Default.Server;
            LoginCommand = new RelayParamCommand(async (parameter) => await Login(parameter));
            DeleteAccCommand = new RelayParamCommand((obj) => DeleteAcc(obj as Account));
            ClearAccCommand = new RelayCommand(() => ClearAccs());
            BackCommand = new RelayCommand(() =>
            {
                ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
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
                var settings = IoC.Kernel.Get<MainViewModel>().GlobalSettings;
                if (settings.WorkType == WorkType.GlobalOrder || settings.WorkType == WorkType.GlobalParallel)
                {
                    Account.Settings = settings.Settings;
                }
                else
                {
                    Account.Settings = Account.PrivateSettings;
                }
                WebProxy proxy = null;
                if (ProxyIP != string.Empty)
                {
                    proxy = new WebProxy($"http://{ProxyIP}/", true);
                    if (ProxyLogin != string.Empty && ProxyPassword != string.Empty)
                    {
                        proxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPassword);
                    }
                }
                bool answer = false;
                await RunCommandAsync(() => LoginIsRunning, async () =>
                {
                    if (Account.Type == AccountType.Normal)
                    {
                        LoginIsRunning2 = false;
                        answer = await Account.LogIn(true);
                    } else
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
                            MessageBox.Show("Не удалось войти в соу!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    //Properties.Settings.Default.Login = Account.Login;
                    //Properties.Settings.Default.Password = Account.Password;
                    //Properties.Settings.Default.Server = (int)Account.Server;
                    //Properties.Settings.Default.Save();
                    IoC.Kernel.Get<MainViewModel>().Accounts.Add(Account);
                    IoC.Kernel.Get<MainViewModel>().SelectedAccount = Account;
                    ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
                }
                else
                {
                    MessageBox.Show("Ошибка входа!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка входа!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                LoginIsRunning2 = true;
            }
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
