using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace QuallyFlash
{
    public class WindowViewModel
    {
        #region Commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand ChangeLanguageCommand { get; set; }
        #endregion

        public string Logo { get; set; } = "Qually Flash";
        public string FlagImage { get; set; }

        public WindowViewModel(Window window)
        {
            FlagImage = @"\..\Resources\ruFlag.png";
            MinimizeCommand = new RelayCommand(() => Minimize(window));
            MaximizeCommand = new RelayCommand(() => Maximize(window));
            CloseCommand = new RelayCommand(() => Close(window));
            ChangeLanguageCommand = new RelayCommand(() => ChangeLanguage());
            Logo = $"Qually Flash ({MainHelper.GetProgramVersion().Substring(0, MainHelper.GetProgramVersion().Length - 4)})";
            StartUp();
        }

        private async void Close(Window window)
        {
            MainViewModel.logger.Info("Конец сессии");
            XmlHelper.SaveGlobalSettingsToFile(GlobalSettings.GetInstance());
            List<Account> accounts = XmlHelper.LoadAccountsFromFile();
            Account acc = MainViewModel.GetInstance().Account;
            if (acc != null)
            {
                for (int i = 0; i < accounts.Count; i++)
                {
                    if (acc.Login == accounts[i].Login && acc.Server == accounts[i].Server && acc.Type == accounts[i].Type)
                    {
                        accounts[i] = acc;
                    }
                }
                XmlHelper.SaveAccountsToFile(accounts);
                if (acc.Type == AccountType.Co)
                {
                    await acc.LogOut(AccountType.Co);
                }
                await acc.LogOut(AccountType.Normal);
            }
            window.Close();
        }

        private void StartUp()
        {
            GlobalSettings.GetInstance();
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
            MainHelper.ShowPage(new LoginPage());
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

        private void Minimize(Window window)
        {
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
            }
            else
            {
                window.Width = width;
                window.Height = height;
                window.Left = left;
                window.Top = top;
                maxCheck = false;
            }
        }

        private void ChangeLanguage()
        {
            if (CultureInfo.CurrentUICulture.Name == CultureInfo.GetCultureInfo("ru-RU").Name)
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                FlagImage = @"\..\Resources\usaFlag.png";
                GlobalSettings.GetInstance().Localization = Localization.en;
            }
            else
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
                FlagImage = @"\..\Resources\ruFlag.png";
                GlobalSettings.GetInstance().Localization = Localization.ru;
            }

            if (((MainWindow)Application.Current.MainWindow).CurrentPage is LoginPage) {
                MainHelper.ShowPage(new LoginPage());
            } else
            {
                MainHelper.ShowPage(new MainPage());
            }
        }
    }
}
