using Microsoft.Win32;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace BotQually
{
    public class SettingsViewModel : BaseViewModel
    {
        public ICommand ReturnCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public Settings Settings { get; set; }
        public IEnumerable<ProductType> Products { get => Enum.GetValues(typeof(ProductType)).Cast<ProductType>().Where(product => product != ProductType.Hay && product != ProductType.Oat && product != ProductType.Shit && product != ProductType.OR); }

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
                using (FileStream stream = File.Create(dialog.FileName))
                {
                    var xs = new XmlSerializer(typeof(Settings));
                    xs.Serialize(stream, Settings);
                }
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
                using (FileStream xmlLoad = File.Open(dialog.FileName, FileMode.Open))
                {
                    var xs = new XmlSerializer(typeof(Settings));
                    Settings = (Settings)xs.Deserialize(xmlLoad);
                }
            }
        }

        private void ResetSettings()
        {
            Settings.HorsingFemale = false;
            Settings.HorsingFemalePrice = "500";
            Settings.Breeder = string.Empty;
            Settings.ClearBlood = false;
            Settings.SelfMale = false;
            Settings.HorsingMale = false;
            Settings.HorsingMalePrice = "500";
            Settings.Carrot = false;
            Settings.MaleName = "Муж";
            Settings.FemaleName = "Жен";
            Settings.Affix = string.Empty;
            Settings.Farm = string.Empty;
            Settings.CentreDuration = "3";
            Settings.CentreHay = false;
            Settings.CentreOat = false;
            Settings.ReserveID = string.Empty;
            Settings.ReserveDuration = string.Empty;
            Settings.ContinueDuration = string.Empty;
            Settings.SelfReserve = false;
            Settings.WriteToAll = false;
            Settings.Continue = false;
            Settings.BuyHay = string.Empty;
            Settings.BuyOat = string.Empty;
            Settings.MainProductToSell = ProductType.Wheat;
            Settings.SubProductToSell = ProductType.Wheat;
            Settings.SellShit = false;
            Settings.Mission = false;
            Settings.OldHorses = false;
            Settings.HealthEdge = "0";
            Settings.SkipIndex = 0;
            Settings.RandomNames = false;
            Settings.LoadSleep = true;
        }

        private void Return()
        {
            var vm = IoC.Kernel.Get<MainViewModel>();
            if (vm.GlobalSettings.WorkType == WorkType.SingleOrder || vm.GlobalSettings.WorkType == WorkType.SingleParallel)
            {
                vm.SelectedAccount.PrivateSettings = Settings;
                foreach (var acc in vm.Accounts)
                {
                    acc.Settings = Settings;
                }
            }
            else
            {
                vm.GlobalSettings.Settings = Settings;
                foreach (var acc in vm.Accounts)
                {
                    acc.Settings = Settings;
                }
            }
            vm.SelectedAccount.SetMainProductToSell();
            vm.SelectedAccount.SetSubProductToSell();
            ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
        }

        private void Back()
        {
            ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
        }
    }
}
