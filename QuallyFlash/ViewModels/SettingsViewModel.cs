using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace QuallyFlash
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Commands
        public ICommand AcceptCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand AddTrainCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand RefreshSchemeCommand { get; set; }
        public ICommand OpenNamesWindowCommand { get; set; }
        #endregion

        #region Public members
        public Settings Settings { get; set; } = new Settings();
        public ObservableCollection<Training> Training { get; set; } = new ObservableCollection<Training>()
        {
            new Training(Properties.Resources.TrainingForest, "forest"),
            new Training(Properties.Resources.TrainingMountain, "mountain"),
            new Training(Properties.Resources.TrainingVitality, "vitality"),
            new Training(Properties.Resources.TrainingSpeed, "speed"),
            new Training(Properties.Resources.TrainingDressage, "dressage"),
            new Training(Properties.Resources.TrainingGalop, "galop"),
            new Training(Properties.Resources.TrainingLynx, "lynx"),
            new Training(Properties.Resources.TrainingJump, "jump"),
        };
        public ObservableCollection<Training> Classic { get; set; } = new ObservableCollection<Training>()
        {
            new Training(Properties.Resources.TrainingClassicLynx, "lynxcompet"),
            new Training(Properties.Resources.TrainingClassicGalop, "galopcompet"),
            new Training(Properties.Resources.TrainingClassicDressage, "dressagecompet"),
            new Training(Properties.Resources.TrainingClassicCross, "cross"),
            new Training(Properties.Resources.TrainingClassicConcur, "concur"),
        };
        public ObservableCollection<Training> Western { get; set; } = new ObservableCollection<Training>()
        {
            new Training(Properties.Resources.TrainingWesternBarell, "barell"),
            new Training(Properties.Resources.TrainingWesternCutting, "cutting"),
            new Training(Properties.Resources.TrainingWesternTrail, "trail"),
            new Training(Properties.Resources.TrainingWesternRaining, "raining"),
            new Training(Properties.Resources.TrainingWesternPlege, "plege"),
        };
        public ObservableCollection<Training> SpecList { get; set; }
        public Training SelectedTraining { get; set; }
        public Training SelectedCompetition { get; set; }
        public Training SelectedSchemeItem { get; set; }
        public IEnumerable<Limit> Limits { get => Enum.GetValues(typeof(Limit)).Cast<Limit>(); }
        public IEnumerable<HorsingEdge> Edges { get => Enum.GetValues(typeof(HorsingEdge)).Cast<HorsingEdge>(); }
        public IEnumerable<Specialization> Specializations { get => Enum.GetValues(typeof(Specialization)).Cast<Specialization>(); }
        public IEnumerable<SchemeType> SchemeTypes { get => Enum.GetValues(typeof(SchemeType)).Cast<SchemeType>(); }
        public IEnumerable<ProductType> Products { get => Enum.GetValues(typeof(ProductType)).Cast<ProductType>().Where(product => product != ProductType.Hay && product != ProductType.Oat && product != ProductType.Shit && product != ProductType.OR && product != ProductType.Mash); }
        public IEnumerable<TrainType> TrainTypes { get => Enum.GetValues(typeof(TrainType)).Cast<TrainType>(); }
        #endregion

        public SettingsViewModel(Settings settings)
        {
            AcceptCommand = new RelayCommand(() => Accept());
            SaveCommand = new RelayCommand(() => SaveToFile());
            LoadCommand = new RelayCommand(() => LoadFromFile());
            CancelCommand = new RelayCommand(() => Cancel());
            ReloadCommand = new RelayCommand(() => Reload());
            AddTrainCommand = new RelayCommand(() => AddTrain());
            RemoveItemCommand = new RelayCommand(() => RemoveItem());
            RefreshSchemeCommand = new RelayCommand(() => RefreshScheme());
            OpenNamesWindowCommand = new RelayCommand(() => OpenNamesWindow());
            Settings = settings;
            ReloadLists();
            SpecList = (Settings.Specialization == Specialization.Classic) ? Classic : Western;
        }

        private void ResetLists()
        {
            Training = new ObservableCollection<Training>()
            {
                new Training("Лес", "forest"),
                new Training("Горы", "mountain"),
                new Training("Выносливость", "vitality"),
                new Training("Скорость", "speed"),
                new Training("Выездка", "dressage"),
                new Training("Галоп", "galop"),
                new Training("Рысь", "lynx"),
                new Training("Прыжки", "jump"),
            };
            Classic = new ObservableCollection<Training>
            {
                new Training("Рысь(с)", "lynxcompet"),
                new Training("Галоп(с)", "galopcompet"),
                new Training("Выездка(с)", "dressagecompet"),
                new Training("Кросс", "cross"),
                new Training("Конкур", "concur"),
            };
            Western = new ObservableCollection<Training>
            {
                new Training("Бочки", "barell"),
                new Training("Каттинг", "cutting"),
                new Training("Трейл", "trail"),
                new Training("Рейнинг", "raining"),
                new Training("Плеже", "plege"),
            };
            SpecList = (Settings.Specialization == Specialization.Classic) ? Classic : Western;
            ReloadLists();
        }

        private void ReloadLists()
        {
            foreach (var pair in Training.ToList())
            {
                foreach (var pair1 in Settings.Scheme)
                {
                    if (pair.Value == pair1.Value)
                    {
                        Training.Remove(pair);
                    }
                }
            }
            if (Settings.Specialization == Specialization.Classic)
            {
                foreach (var pair in Classic.ToList().SelectMany(pair => Settings.Scheme.Where(pair1 => pair.Value == pair1.Value).Select(pair1 => pair)))
                {
                    Classic.Remove(pair);
                }
            }
            else
            {
                foreach (var pair in Western.ToList().SelectMany(pair => Settings.Scheme.Where(pair1 => pair.Value == pair1.Value).Select(pair1 => pair)))
                {
                    Western.Remove(pair);
                }
            }
        }

        private void RemoveItem()
        {
            if (SelectedSchemeItem != null)
            {
                Settings.Scheme.Remove(SelectedSchemeItem);
                ResetLists();
            }
        }

        private void AddTrain()
        {
            if (SelectedTraining != null)
            {
                Settings.Scheme.Add(SelectedTraining);
                Training.Remove(SelectedTraining);
            }
            if (SelectedCompetition != null)
            {
                Settings.Scheme.Add(SelectedCompetition);
                if (Settings.Specialization == Specialization.Classic)
                {
                    Classic.Remove(SelectedCompetition);
                }
                else
                {
                    Western.Remove(SelectedCompetition);
                }
            }
        }

        private void RefreshScheme()
        {
            Settings.Scheme = new ObservableCollection<Training>();
            ResetLists();
        }

        private void Reload()
        {
            Settings = new Settings();
        }

        private void Cancel()
        {
            MainHelper.ShowPage(new MainPage());
        }

        private void LoadFromFile()
        {
            try
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
                    ResetLists();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"{Properties.Resources.SettingsErrorMessage1}: " + e.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveToFile()
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show($"{Properties.Resources.SettingsErrorMessage2}: " + e.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Accept()
        {
            if ((Settings.Reserve && string.IsNullOrEmpty(Settings.ReserveID)) && (Settings.Reserve && !Settings.SelfReserve))
            {
                MessageBox.Show(Properties.Resources.SettingsErrorMessage3, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var vm = MainViewModel.GetInstance();
                vm.Account.Settings = Settings;
                vm.Account.SetMainProductToSell();
                vm.Account.SetSubProductToSell();
                vm.Account.SetEquipmentToShow();
                if (!vm.Account.IsWorking)
                {
                    if (vm.Horses.Count > 0)
                    {
                        foreach (var horse in vm.Horses)
                        {
                            horse.Scheme = new ObservableCollection<Training>();
                            foreach (var training in Settings.Scheme)
                            {
                                horse.Scheme.Add(training.Clone() as Training);
                            }
                        }
                    }
                }
                MainHelper.ShowPage(new MainPage());
            }
        }

        private void OpenNamesWindow()
        {
            var window = new NamesWindow();
            window.ShowDialog();
        }
    }
}
