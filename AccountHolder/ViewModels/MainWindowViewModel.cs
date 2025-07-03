using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Globalization;

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
        public ICommand ChangeLanguageCommand { get; set; }
        #endregion

        public string FileName { get; set; }
        public ObservableCollection<Account> AccList { get; set; }
        public Account SelectedAccount { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Server SelectedServer { get; set; } = Server.Russia;
        public Localization SelectedLocalization { get; set; } = Localization.ru;
        public bool NotWorking { get; set; } = true;
        public int Count { get; set; }
        public string GoldenApple { get; set; } = "100130670";
        public string Version2 { get; set; } = "";
        public string Usb { get; set; }
        public Settings Settings { get; set; }

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
            ChangeLanguageCommand = new RelayCommand(() => ChangeLanguage());
            if (!Directory.Exists("Accounts"))
            {
                Directory.CreateDirectory("Accounts");
            }
            Settings = XmlHelper.LoadSettingsFromFile();
            SelectedLocalization = Settings.Localization;
            Version2 = typeof(MainWindow).Assembly.GetName().Version.ToString().Substring(0, typeof(MainWindow).Assembly.GetName().Version.ToString().Length - 4);
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
                    MessageBox.Show($"{Properties.Resources.MainDeleteFileErrorMessage} {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                MessageBox.Show(Properties.Resources.MainAddListErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show($"{Properties.Resources.MainLoadListErrorMessage} {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show($"{Properties.Resources.MainTutorialErrorMessage} {e.Message}\n{e.StackTrace}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    Count++;
                }
                NotWorking = true;
                MessageBox.Show(Properties.Resources.MainTutorialSuccessMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
                XmlHelper.SaveAccountsToFile(new List<Account>(AccList), Path.Combine("Accounts", FileName));
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainTutorailErrorMessage2, "", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show($"{Properties.Resources.MainGoAccErrorMessage} {e.Message}\n{e.StackTrace}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    Count++;
                }
                MessageBox.Show(Properties.Resources.MainGoAccSuccessMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
                XmlHelper.SaveAccountsToFile(new List<Account>(AccList), Path.Combine("Accounts", FileName));
                NotWorking = true;
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainGoAccErrorMessage2, "", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Properties.Resources.MainAddAccErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ChangeLanguage()
        {
            if (SelectedLocalization == Localization.en)
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                Settings.Localization = Localization.en;
            }
            else
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
                Settings.Localization = Localization.ru;
            }
            XmlHelper.SaveSettingsToFile(Settings);
        }
    }
}
