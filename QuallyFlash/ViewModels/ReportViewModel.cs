using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace QuallyFlash
{
    public class ReportViewModel : BaseViewModel
    {
        public ICommand SendCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public string Type { get; set; } = "Error";
        public string Body { get; set; }
        public string Contacts { get; set; }

        public ReportViewModel()
        {
            SendCommand = new RelayCommand(() => Send());
            BackCommand = new RelayCommand(() => Back());
        }

        private void Back()
        {
            ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
        }

        private async void Send()
        {
            Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null);
            RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey sqlsrvKey = localMachineX64View.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
            string sqlExpressKeyName = (string)sqlsrvKey.GetValue("MachineGuid");
            string sha = Sha256(sqlExpressKeyName).Substring(0, 20);
            try
            {
                using (var client = new HttpClient())
                {
                    string logmain = string.Empty;
                    string logacc = string.Empty;
                    string logerror = string.Empty;
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/logMain.txt"))
                    {
                        logmain = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "logs/logMain.txt", Encoding.GetEncoding("windows-1251"));
                    }
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/logAcc.txt"))
                    {
                        logacc = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "logs/logAcc.txt", Encoding.GetEncoding("windows-1251"));
                    }
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/logError.txt"))
                    {
                        logerror = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "logs/logError.txt", Encoding.GetEncoding("windows-1251"));
                    }
                    string postData = $"id={sha}&programId=2&type={Type}&body={Body}&version={typeof(MainWindow).Assembly.GetName().Version}&contacts={Contacts}&logmain={logmain}&logacc={logacc}&logerror={logerror}&status=Ожидает%20просмотра";
                    var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = await client.PostAsync("https://botqually.ru/api/managment/message", content);
                    string answer = await result.Content.ReadAsStringAsync();
                    MessageBox.Show("Сообщение отправлено!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    Back();
                }
            }
            catch
            {

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
    }
}