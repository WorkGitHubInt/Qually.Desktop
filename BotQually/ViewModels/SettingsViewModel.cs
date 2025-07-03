using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace BotQually
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Commands
        public ICommand ReturnCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand BackCommand { get; set; }
        #endregion

        public Settings Settings { get; set; }
        public IEnumerable<ProductType> Products { get => Enum.GetValues(typeof(ProductType)).Cast<ProductType>().Where(product => product != ProductType.Oat && product != ProductType.Shit && product != ProductType.OR); }

        public SettingsViewModel(Settings settings)
        {
            ReturnCommand = new RelayCommand(() => Return());
            SaveCommand = new RelayCommand(() => SaveToFile());
            LoadCommand = new RelayCommand(() => LoadFromFile());
            ResetCommand = new RelayCommand(() => ResetSettings());
            BackCommand = new RelayCommand(() => Back());
            Settings = settings;
        }

        private void SaveToFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "XML files(.xml)|*.xml|all Files(*.*)|*.*",
            };
            if (dialog.ShowDialog() == true)
            {
                XmlHelper.SaveSettingsToFile(dialog.FileName, Settings);
            }
        }

        private void LoadFromFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "XML files(.xml)|*.xml|all Files(*.*)|*.*",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (dialog.ShowDialog() == true)
            {
                Settings = XmlHelper.LoadSettingsFromFile(dialog.FileName);
            }
        }

        private void ResetSettings()
        {
            Settings = new Settings();
        }

        private void Return()
        {
            var vm = MainViewModel.GetInstance();
            GlobalSettings globalSettings = GlobalSettings.GetInstance();
            if (globalSettings.WorkType == WorkType.SingleOrder || globalSettings.WorkType == WorkType.SingleParallel)
            {
                vm.SelectedAccount.PrivateSettings = Settings;
                foreach (var acc in vm.Accounts)
                {
                    acc.Settings = Settings;
                }
            }
            else
            {
                globalSettings.Settings = Settings;
                foreach (var acc in vm.Accounts)
                {
                    acc.Settings = Settings;
                }
            }
            vm.SelectedAccount.SetMainProductToSell();
            vm.SelectedAccount.SetSubProductToSell();
            MainHelper.ShowPage(new MainPage());
        }

        private void Back()
        {
            MainHelper.ShowPage(new MainPage());
        }
    }
}
