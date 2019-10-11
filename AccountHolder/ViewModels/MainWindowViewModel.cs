using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AccountHolder
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Commands
        public ICommand CreateNewListCommand { get; set; }
        public ICommand AddListCommand { get; set; }
        public ICommand LoadListCommand { get; set; }
        public ICommand TrainListCommand { get; set; }
        public ICommand GoListCommand { get; set; }
        public ICommand AddAccCommand { get; set; }
        public ICommand DeleteAccCommand { get; set; }
        public ICommand DeleteListCommand { get; set; }
        #endregion

        public string FileName { get; set; }
        public ObservableCollection<Account> AccList { get; set; }
        public Account SelectedAccount { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Server SelectedServer { get; set; } = Server.Russia;
        public bool NotWorking { get; set; } = true;
        public int Count { get; set; }
        public string GoldenApple { get; set; } = "100130670";
        public string Version2 { get; set; } = "";
        public string Usb { get; set; }

        public MainWindowViewModel()
        {
            CreateNewListCommand = new RelayCommand(() => CreateNewList());
            AddListCommand = new RelayCommand(() => AddList());
            LoadListCommand = new RelayCommand(() => LoadList());
            TrainListCommand = new RelayCommand(() => TrainList());
            GoListCommand = new RelayCommand(() => GoList());
            AddAccCommand = new RelayCommand(() => AddAcc());
            DeleteAccCommand = new RelayCommand(() => DeleteAcc());
            DeleteListCommand = new RelayCommand(() => DeleteList());
            if (!Directory.Exists("Accounts"))
            {
                Directory.CreateDirectory("Accounts");
            }
            Version2 = typeof(MainWindow).Assembly.GetName().Version.ToString().Substring(0, typeof(MainWindow).Assembly.GetName().Version.ToString().Length - 4);
            //OnLoad();
        }

        private void DeleteList()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "(*.xml)|*.xml",
                CheckFileExists = true,
                InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Accounts"),
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    File.Delete(dialog.FileName);
                    FileName = dialog.SafeFileName;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Произошла ошибка при удалении файла! {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void OnLoad()
        {
            await Check();
            LaunchCount();
            Version version = await CheckVersion();
            if (typeof(MainWindow).Assembly.GetName().Version < version)
            {
                MessageBox.Show(Application.Current.MainWindow, $"Доступно обновление {version}! Запустите лаунчер чтобы обновиться.", "", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }

        private void DeleteAcc()
        {
            AccList.Remove(SelectedAccount);
        }

        private void CreateNewList()
        {
            if (AccList != null)
            {
                AccList.Clear();
            }
            string fileName = "Accounts.xml";
            int number = 0;
            while (File.Exists(Path.Combine("Accounts", fileName)))
            {
                fileName = "Accounts_" + number + ".xml";
                number++;
            }
            AccList = new ObservableCollection<Account>();
            FileName = fileName;
        }

        private void AddList()
        {
            if (AccList != null)
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Текстовые(*.txt)|*.txt",
                    CheckFileExists = true,
                };
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        string[] readText = File.ReadAllLines(dialog.FileName, Encoding.UTF8);
                        List<string> logins = new List<string>();
                        List<string> passwords = new List<string>();
                        foreach (string s in readText)
                        {
                            string[] arrStr = s.Split(':');
                            logins.Add(arrStr[0]);
                            passwords.Add(arrStr[1]);
                        }
                        int length = Convert.ToInt32(readText.Length);
                        for (int i = 0; i < length; i++)
                        {
                            Account acc = new Account
                            {
                                Login = logins[i],
                                Password = passwords[i],
                                Days = "0",
                                Tutorial = "Нет"
                            };
                            AccList.Add(acc);
                        }
                        XmlHelper.SaveAccountsToFile(new List<Account>(AccList), Path.Combine("Accounts", FileName));
                    }
                    catch
                    {
                        MessageBox.Show("Неверный файл!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Список аккаунтов не создан!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadList()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "(*.xml)|*.xml",
                CheckFileExists = true,
                InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Accounts"),
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    AccList = new ObservableCollection<Account>(XmlHelper.LoadAccountsFromFile(dialog.FileName));
                    FileName = dialog.SafeFileName;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Произошла ошибка при загрузке файла! {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void TrainList()
        {
            if (AccList != null && AccList.Count > 0)
            {
                Count = 0;

                NotWorking = false;
                foreach (var acc in AccList)
                {
                    acc.Server = SelectedServer;
                    SelectedAccount = acc;
                    try
                    {
                        await acc.DoTraining(GoldenApple);

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Произошла ошибка! {e.Message}\n{e.StackTrace}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    Count++;
                }
                NotWorking = true;
                MessageBox.Show("Готово!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                XmlHelper.SaveAccountsToFile(new List<Account>(AccList), Path.Combine("Accounts", FileName));
            }
            else
            {
                MessageBox.Show("Список аккаунтов не создан или количество аккаунтов 0!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GoList()
        {
            if (AccList != null && AccList.Count > 0)
            {
                Count = 0;
                NotWorking = false;
                foreach (var acc in AccList)
                {
                    SelectedAccount = acc;
                    acc.Server = SelectedServer;
                    try
                    {
                        await acc.GetDays();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Произошла ошибка! {e.Message}\n{e.StackTrace}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    Count++;
                }
                MessageBox.Show("Готово!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                XmlHelper.SaveAccountsToFile(new List<Account>(AccList), Path.Combine("Accounts", FileName));
                NotWorking = true;
            }
            else
            {
                MessageBox.Show("Список аккаунтов не создан или количество аккаунтов 0!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAcc()
        {
            if (AccList != null)
            {
                var acc = new Account
                {
                    Login = Login,
                    Password = Password,
                    Days = "0",
                    Tutorial = "Нет",
                };
                AccList.Add(acc);
                XmlHelper.SaveAccountsToFile(new List<Account>(AccList), Path.Combine("Accounts", FileName));
            }
            else
            {
                MessageBox.Show("Список аккаунтов не создан!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LaunchCount()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent("programId=3", Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = await client.PostAsync("https://botqually.ru/api/managment/count", content);
                }
            }
            catch { }
        }

        private async Task Check()
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
                    if (!(bool)user["active"])
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
                                    MessageBox.Show(Application.Current.MainWindow, "Обнаружена попытка несанкционированого доступа!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                                    Process.GetCurrentProcess().Kill();
                                }
                            }
                            catch { }
                        }
                        if (!chk)
                        {
                            MessageBox.Show(Application.Current.MainWindow, "Ваша подписка не активна или не соотвествует уровень!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(Application.Current.MainWindow, "Обнаружена попытка несанкционированого доступа!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, $"Произошла какая-то ошибка! Обратитесь к администрации! {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Process.GetCurrentProcess().Kill();
            }
        }

        private async Task<Version> CheckVersion()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://botqually.ru/api/managment/program?progId=3");
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
