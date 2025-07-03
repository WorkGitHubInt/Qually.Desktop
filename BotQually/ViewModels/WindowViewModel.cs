using System.Windows;
using System.Windows.Input;
using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BotQually
{
    public class WindowViewModel : BaseViewModel
    {
        #region Commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand ChangeLanguageCommand { get; set; }
        #endregion

        public string FlagImage { get; set; }

        public WindowViewModel(Window window)
        {
            FlagImage = @"\..\Resources\ruFlag.png";
            MinimizeCommand = new RelayCommand(() => Minimize(window));
            MaximizeCommand = new RelayCommand(() => Maximize(window));
            CloseCommand = new RelayCommand(() => Close(window));
            SettingsCommand = new RelayCommand(() =>
            {
                MainViewModel.GetInstance().GlobalSettingsOpen = !MainViewModel.GetInstance().GlobalSettingsOpen;
            });
            ChangeLanguageCommand = new RelayCommand(() => ChangeLanguage());
            StartUp();
        }

        private async void Close(Window window)
        {
            XmlHelper.SaveGlobalSettingsToFile(GlobalSettings.GetInstance());
            List<Account> accounts = XmlHelper.LoadAccountsFromFile();
            foreach (Account account in MainViewModel.GetInstance().Accounts)
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
            foreach (Account account in MainViewModel.GetInstance().Accounts)
            {
                if (account.Type == AccountType.Co)
                {
                    await account.LogOut(AccountType.Co);
                }
                await account.LogOut(AccountType.Normal);
            }
            MainViewModel.logger.Info("Конец сессии");
            window.Close();
        }

        private void Minimize(Window window)
        {
            bool tray = GlobalSettings.GetInstance().Tray;
            if (tray)
            {
                window.Hide();
            }
            window.WindowState = WindowState.Minimized;
        }

        private bool maxCheck = false;
        private double width = 0;
        private double height = 0;
        private double left = 0;
        private double top = 0;
        private void Maximize(Window window)
        {
            if (!maxCheck)
            {
                width = window.Width;
                height = window.Height;
                left = window.Left;
                top = window.Top;
                window.Width = SystemParameters.WorkArea.Width;
                window.Height = SystemParameters.WorkArea.Height;
                window.Left = 0;
                window.Top = 0;
                maxCheck = true;
            } else
            {
                window.Width = width;
                window.Height = height;
                window.Left = left;
                window.Top = top;
                maxCheck = false;
            }
        }

        private void StartUp()
        {
            if (GlobalSettings.GetInstance().Localization == Localization.ru)
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
                FlagImage = @"\..\Resources\ruFlag.png";
            }
            else
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                FlagImage = @"\..\Resources\usaFlag.png";
            }
            SettingsControl();
            MainHelper.ShowPage(new MainPage());
        }

        private void SettingsControl()
        {
            try
            {
                string[] filesNames = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
                foreach (string file in filesNames)
                {
                    DateTime creationDateLog = File.GetCreationTime(file);
                    if ((DateTime.Now - creationDateLog).TotalDays > 2)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch { }
        }

        private void ChangeLanguage()
        {
            if (GlobalSettings.GetInstance().Localization == Localization.ru)
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                FlagImage = @"\..\Resources\usaFlag.png";
                GlobalSettings.GetInstance().Localization = Localization.en;
            } else
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
                FlagImage = @"\..\Resources\ruFlag.png";
                GlobalSettings.GetInstance().Localization = Localization.ru;
            }
            MainHelper.ShowPage(new MainPage());
        }
    }
}
