using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace BotQually
{
    public partial class MainWindow : Window
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        Stack<Page> pages = new Stack<Page>();
        public readonly NotifyIcon ni = new NotifyIcon();
        public Page CurrentPage { get; set; }
        public string Usb { get; set; }

        public MainWindow()
        {
            ni.Text = "Bot Qually";
            ni = new NotifyIcon
            {
                Icon = Properties.Resources.logo2,
                Visible = true
            };
            ni.MouseDown += new MouseEventHandler(NIClick);
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
            InitializeComponent();
            DataContext = new WindowViewModel(this);
        }

        public void NIClick(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                Show();
                WindowState = WindowState.Normal;
            }
        }

        private async Task<string> Check()
        {
            try
            {
                string userStr;
                if (!string.IsNullOrEmpty(Usb))
                {
                    userStr = Usb;
                }
                else
                {
                    RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    RegistryKey sqlsrvKey = localMachineX64View.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                    string sqlExpressKeyName = (string)sqlsrvKey.GetValue("MachineGuid");
                    userStr = Sha256(sqlExpressKeyName).Substring(0, 20);
                }
                JObject user = await GetUser(userStr);
                if ((string)user["hash"] == MD5hash($"{(string)user["pcid"]}|{(string)user["subscriptionExpDate"]}|{(string)user["unlimitedSub"]}|{(string)user["trial"]}|{(string)user["lvl"]}|{(string)user["active"]}|helicopter").ToLower())
                {
                    bool chk = false;
                    if ((bool)user["active"])
                    {
                        XmlHelper.LoadSettingsFromFile();
                    }
                    else if (!(bool)user["active"])
                    {
                        foreach (string usb in GetDevicesHash())
                        {
                            try
                            {
                                user = await GetUserByUSB(usb);
                                if ((string)user["hash"] == MD5hash($"{(string)user["pcid"]}|{(string)user["subscriptionExpDate"]}|{(string)user["unlimitedSub"]}|{(string)user["trial"]}|{(string)user["lvl"]}|{(string)user["active"]}|helicopter").ToLower())
                                {
                                    if ((bool)user["active"])
                                    {
                                        chk = true;
                                    }
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show(System.Windows.Application.Current.MainWindow, "Обнаружена попытка несанкционированого доступа!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                                    Process.GetCurrentProcess().Kill();
                                }
                            }
                            catch { }
                        }
                        if (chk)
                        {
                            XmlHelper.LoadSettingsFromFile();
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(System.Windows.Application.Current.MainWindow, "Ваша подписка не активна или не соотвествует уровень!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(System.Windows.Application.Current.MainWindow, "Обнаружена попытка несанкционированого доступа!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill();
                }
                return (string)user["pcid"];
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(System.Windows.Application.Current.MainWindow, $"Произошла какая-то ошибка! Обратитесь к администрации! {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
                return string.Empty;
            }
        }

        public void ShowPage(Page newPage)
        {
            pages.Push(newPage);

            Task.Factory.StartNew(() => ShowNewPage());
        }

        private void ShowNewPage()
        {
            Dispatcher.Invoke((Action)delegate
            {
                if (MainFrame.Content != null)
                {
                    Page oldPage = MainFrame.Content as Page;

                    if (oldPage != null)
                    {
                        oldPage.Loaded -= NewPage_Loaded;

                        UnloadPage(oldPage);
                    }
                }
                else
                {
                    ShowNextPage();
                }

            });
        }

        private void ShowNextPage()
        {
            Page newPage = pages.Pop();

            newPage.Loaded += NewPage_Loaded;

            MainFrame.Content = newPage;
        }

        private void ShowNextPageAfter()
        {
            Page newPage = pages.Pop();

            newPage.Loaded += NewPageAfter_Loaded;

            MainFrame.Content = newPage;
        }

        private async void UnloadPage(Page page)
        {
            Storyboard hidePage = (Resources["GrowAndFadeOut"] as Storyboard).Clone();
            hidePage.Begin(MainFrame);
            await Task.Delay(100);
            ShowNextPageAfter();
            //hidePage.Completed += hidePage_Completed;
        }

        private void NewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard showNewPage = Resources["GrowAndFadeIn"] as Storyboard;

            showNewPage.Begin(MainFrame);

            CurrentPage = sender as Page;
        }

        private void NewPageAfter_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard showNewPage = Resources["GrowAndFadeInAfter"] as Storyboard;

            showNewPage.Begin(MainFrame);

            CurrentPage = sender as Page;
        }

        private void HidePage_Completed(object sender, EventArgs e)
        {
            MainFrame.Content = null;

            ShowNextPageAfter();
        }

        private async void Window_Loaded(object sender, EventArgs e)
        {
            string id = await Check();
            Count(id);
            Version version = await CheckVersion();
            if (typeof(MainWindow).Assembly.GetName().Version < version)
            {
                System.Windows.MessageBox.Show(System.Windows.Application.Current.MainWindow, $"Доступно обновление {version}! Запустите лаунчер чтобы обновиться.", "", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "settings"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "settings");
            }
            ShowPage(new MainPage());
        }

        private void ClearLogs()
        {
            try
            {
                DateTime creationDateLog = File.GetCreationTime(AppDomain.CurrentDomain.BaseDirectory + "logs/logMain.txt");
                if ((DateTime.Now - creationDateLog).TotalDays >= 2)
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/logMain.txt");
                }
                creationDateLog = File.GetCreationTime(AppDomain.CurrentDomain.BaseDirectory + "logs/logAcc.txt");
                if ((DateTime.Now - creationDateLog).TotalDays >= 2)
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/logAcc.txt");
                }
                creationDateLog = File.GetCreationTime(AppDomain.CurrentDomain.BaseDirectory + "logs/logError.txt");
                if ((DateTime.Now - creationDateLog).TotalDays >= 2)
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/logError.txt");
                }
                creationDateLog = File.GetCreationTime(AppDomain.CurrentDomain.BaseDirectory + "logs/log.txt");
                if ((DateTime.Now - creationDateLog).TotalDays >= 2)
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/log.txt");
                }
            }
            catch
            {
            }
        }

        private async Task Count(string id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent($"userId={id}&programId=1", Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = await client.PostAsync("https://botqually.ru/api/managment/count", content);
                }
            }
            catch { }
        }

        private async Task<Version> CheckVersion()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://botqually.ru/api/managment/program?progId=1");
                string answer = await result.Content.ReadAsStringAsync();
                JObject jobj = JObject.Parse(answer);
                string version = (string)jobj["version"];
                return Version.Parse(version);
            }
        }

        private async Task<JObject> GetUser(string id)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://botqually.ru/api/users/?id=" + id);
                string answer = await result.Content.ReadAsStringAsync();
                JObject jobj = JObject.Parse(answer);
                return jobj;
            }
        }

        private async Task<JObject> GetUserByUSB(string id)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://botqually.ru/api/users/usb?id=" + id);
                string answer = await result.Content.ReadAsStringAsync();
                JObject jobj = JObject.Parse(answer);
                return jobj;
            }
        }

        private List<string> GetDevicesHash()
        {
            List<string> usb = new List<string>();
            foreach (ManagementObject drive in new ManagementObjectSearcher("select * from Win32_USBHub").Get())
            {
                try
                {
                    string PNPDeviceID = drive["PNPDeviceID"].ToString().Trim();
                    usb.Add(Sha1Hash($"{Convert.ToInt32(Regex.Match(PNPDeviceID, @"PID_(.*?)\\").Groups[1].Value, 16).ToString()}|{Convert.ToInt32(Regex.Match(PNPDeviceID, "VID_(.*?)&").Groups[1].Value, 16).ToString()}"));
                }
                catch { }
            }
            return usb;
        }

        private string Sha1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private string Sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlHelper.SaveSettingsToFile();
            List<Account> accounts = XmlHelper.LoadAccountsFromFile();
            foreach (Account account in IoC.Kernel.Get<MainViewModel>().Accounts)
            {
                for (int i = 0; i < accounts.Count; i++)
                {
                    if (account.Login == accounts[i].Login && account.Server == accounts[i].Server && account.Type == accounts[i].Type)
                    {
                        accounts[i] = account;
                    }
                }
            }
            XmlHelper.SaveAccountsToFile(accounts);
            foreach (Account account in IoC.Kernel.Get<MainViewModel>().Accounts)
            {
                if (account.Type == AccountType.Co)
                {
                    await account.LogOut(AccountType.Co);
                }
                await account.LogOut(AccountType.Normal);
            }
            MainViewModel.logger.Info("Конец сессии");
            Process.GetCurrentProcess().Kill();
        }

        private string MD5hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
