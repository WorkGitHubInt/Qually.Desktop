using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Windows.Threading;

namespace QuallyFlash
{
    public class MainViewModel : BaseViewModel
    {
        #region Command
        public ICommand LoadFarmCommand { get; set; }
        public ICommand AddHorseCommand { get; set; }
        public ICommand RemoveHorseCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand LoginCoCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand ReloadFarmsCommand { get; set; }
        public ICommand NextHorseCommand { get; set; }
        public ICommand OpenManagerCommand { get; set; }
        public ICommand OpenStatusCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand SaveCoAccountCommand { get; set; }
        #endregion

        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public Account Account { get; set; }
        public Farm SelectedFarm { get; set; }
        public Horse SelectedHorse { get; set; }
        public GlobalSettings GlobalSettings { get; set; }
        public ObservableCollection<Horse> Horses { get; set; } = new ObservableCollection<Horse>();
        public Horse SelectedSexHorse { get; set; }
        public bool MultiLineTab { get; set; } = false;
        public bool IsLoading { get; set; } = false;

        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }
            return instance;
        }

        private MainViewModel()
        {
            StartCommand = new RelayCommand(() => Start());
            StopCommand = new RelayCommand(() => Stop());
            SettingsCommand = new RelayCommand(() => OpenSettings());
            LoginCoCommand = new RelayCommand(() => LoginCo());
            LogoutCommand = new RelayCommand(() => Logout());
            LoadFarmCommand = new RelayCommand(async () => await LoadFarm());
            AddHorseCommand = new RelayCommand(() => AddHorse());
            RemoveHorseCommand = new RelayCommand(() => RemoveHorse());
            ReloadFarmsCommand = new RelayCommand(() => ReloadFarms());
            NextHorseCommand = new RelayCommand(() => ChangeNextHorse());
            OpenManagerCommand = new RelayCommand(() => OpenManager());
            OpenStatusCommand = new RelayCommand(() => OpenStatus());
            BackCommand = new RelayCommand(() => Back());
            SaveCoAccountCommand = new RelayCommand(() => SaveCoAccountToFile());
            logger.Info("Начало сессии");
            GlobalSettings = GlobalSettings.GetInstance();
        }

        private void Back()
        {
            logger.Info("Вовращение из статуса");
            MainHelper.ShowPage(new MainPage());
        }

        private void OpenStatus()
        {
            logger.Info("Открытие статуса");
            MainHelper.ShowPage(new StatusPage() { DataContext = this });
        }

        private string filter;
        public string Filter
        {
            get => filter;
            set
            {
                filter = value;
                if (SelectedFarm != null)
                {
                    if (SelectedFarm.Horses != null)
                    {
                        SelectedFarm.FilteredHorses.Clear();
                        foreach (var horse in SelectedFarm.Horses.Where(h => h.Name.Contains(value)))
                        {
                            SelectedFarm.FilteredHorses.Add(horse);
                        }
                    }
                }
            }
        }

        private async void ReloadFarms()
        {
            if (Account != null && !Account.IsWorking)
            {
                logger.Info("Перезагрузка заводов");
                await Account.LoadFarms();
            }
        }

        private async Task LoadFarm()
        {
            if (SelectedFarm != null)
            {
                logger.Info("Загрузка лошадей");
                await SelectedFarm.LoadHorses(GlobalSettings.Sort);
            }
        }

        private async void AddHorse()
        {
            if (SelectedHorse != null)
            {
                logger.Info("Добавление лошади");
                SelectedHorse.Scheme = new ObservableCollection<Training>();
                foreach (var training in Account.Settings.Scheme)
                {
                    SelectedHorse.Scheme.Add(training.Clone() as Training);
                }
                if (SelectedHorse.LoadInfo(await SelectedHorse.GetDoc()))
                {
                    if (Account.Settings.SchemeType == SchemeType.HalfPair && Horses.Count < 1)
                    {
                        Horses.Add(SelectedHorse.Clone() as Horse);
                        SelectedSexHorse = Horses[Horses.Count - 1];
                        if (SelectedHorse.Sex == HorseSex.Male)
                        {
                            Account.MaleHorse = SelectedSexHorse;
                        }
                        else
                        {
                            Account.FemaleHorse = SelectedSexHorse;
                        }
                    }
                    else if (Account.Settings.SchemeType == SchemeType.Pair && Horses.Count < 2)
                    {
                        if (Horses.Count == 0)
                        {
                            Horses.Add(SelectedHorse.Clone() as Horse);
                            SelectedSexHorse = Horses[Horses.Count - 1];
                            if (SelectedHorse.Sex == HorseSex.Male)
                            {
                                Account.MaleHorse = SelectedSexHorse;
                            }
                            else
                            {
                                Account.FemaleHorse = SelectedSexHorse;
                            }
                        }
                        else if (Horses.Count == 1 && Horses[0].Sex == HorseSex.Male && SelectedHorse.Sex == HorseSex.Female)
                        {
                            Horses.Add(SelectedHorse.Clone() as Horse);
                            SelectedSexHorse = Horses[Horses.Count - 1];
                            if (SelectedHorse.Sex == HorseSex.Male)
                            {
                                Account.MaleHorse = SelectedSexHorse;
                            }
                            else
                            {
                                Account.FemaleHorse = SelectedSexHorse;
                            }
                        }
                        else if (Horses.Count == 1 && Horses[0].Sex == HorseSex.Female && SelectedHorse.Sex == HorseSex.Male)
                        {
                            Horses.Add(SelectedHorse.Clone() as Horse);
                            SelectedSexHorse = Horses[Horses.Count - 1];
                            if (SelectedHorse.Sex == HorseSex.Male)
                            {
                                Account.MaleHorse = SelectedSexHorse;
                            }
                            else
                            {
                                Account.FemaleHorse = SelectedSexHorse;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MainErrorMessage1, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RemoveHorse()
        {
            if (SelectedSexHorse != null && !Account.IsWorking)
            {
                if (Horses.Count > 0)
                {
                    logger.Info("Удаление лошади");
                    Horses.Remove(SelectedSexHorse);
                }
            }
        }

        private async void LoginCo()
        {
            if (!Account.IsWorking)
            {
                if (Account.Type == AccountType.Co)
                {
                    await Account.LogOut(AccountType.Co);
                } else if (Account.Type == AccountType.Normal)
                {
                    logger.Info("Вход в соу");
                    MainHelper.ShowPage(new LoginCoPage() { DataContext = new LoginCoViewModel(Account) });
                }
            }
        }

        private void OpenSettings()
        {
            logger.Info("Открытие настроек");
            MainHelper.ShowPage(new SettingsPage() { DataContext = new SettingsViewModel(Account.Settings.Clone() as Settings) });
        }

        public async void Start()
        {
            if (!Account.IsWorking)
            {
                logger.Info("Старт работы");
                if ((Account.Settings.SchemeType == SchemeType.Pair && Horses.Count == 2) || (Account.Settings.SchemeType == SchemeType.HalfPair && Horses.Count == 1))
                {
                    try
                    {
                        if (Account.Settings.SchemeType == SchemeType.Pair)
                        {
                            MainHelper.ShowPage(new StatusPage() { DataContext = this });
                        }
                        await Account.StartTraining(Horses).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        if (Application.Current.Dispatcher.CheckAccess())
                        {
                            MessageBox.Show(Application.Current.MainWindow, Properties.Resources.MainCancelMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                MessageBox.Show(Application.Current.MainWindow, Properties.Resources.MainCancelMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
                            }));
                        }
                        logger.Info("Отмена работы");
                        Account.Client.Ct = new CancellationToken();
                    }
                    catch (Exception e)
                    {
                        
                        if (Application.Current.Dispatcher.CheckAccess())
                        {
                            MessageBox.Show(Application.Current.MainWindow, Properties.Resources.MainErrorMessage2, "", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                MessageBox.Show(Application.Current.MainWindow, Properties.Resources.MainErrorMessage2, "", MessageBoxButton.OK, MessageBoxImage.Error);
                            }));
                        }
                        logger.Error($"--------------------------------------------------------------");
                        logger.Error($"Ошибка: {e.Message}; {e.StackTrace}");
                        if (Account.MaleHorse != null)
                        {
                            logger.Error($"Лошадь М параметры: Возраст:{Account.MaleHorse.Age}; Здоровье:{Account.MaleHorse.Health}; Энергия:{Account.MaleHorse.Energy}");
                        }
                        if (Account.FemaleHorse != null)
                        {
                            logger.Error($"Лошадь Ж параметры: Возраст:{Account.FemaleHorse.Age}; Здоровье:{Account.FemaleHorse.Health}; Энергия:{Account.FemaleHorse.Energy}");
                        }
                        logger.Error($"Аккаунт {Account.Type}: Экю:{Account.Equ}; Фураж:{Account.Hay.Amount}; Овес:{Account.Oat.Amount}; ОР:{Account.OR.Amount}; ");
                    }
                    finally
                    {
                        Account.IsWorking = false;
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MainErrorMessage3, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Stop()
        {
            if (Account.Cts != null)
            {
                logger.Info("Стоп работы");
                Account.Cts.Cancel();
            }
        }

        private async void Logout()
        {
            if (!Account.IsWorking)
            {
                logger.Info("Выход из аккаунта");
                List<Account> accounts = XmlHelper.LoadAccountsFromFile();
                for (int i = 0; i < accounts.Count; i++)
                {
                    if (Account.Login == accounts[i].Login && Account.Server == accounts[i].Server && Account.Type == accounts[i].Type)
                    {
                        accounts[i] = Account;
                    }
                }
                XmlHelper.SaveAccountsToFile(accounts);
                if (Account.Type == AccountType.Co)
                {
                    await Account.LogOut(AccountType.Co);
                }
                await Account.LogOut(AccountType.Normal);
                MainHelper.ShowPage(new LoginPage());
            }
        }

        private void ChangeNextHorse()
        {
            logger.Info("Смена лошади");
            Account.NextHorseSex = Account.NextHorseSex == HorseSex.Male ? HorseSex.Female : HorseSex.Male;
        }

        private void OpenManager()
        {
            logger.Info("Открытие управление");
            MainHelper.ShowPage(new ManagerPage() { DataContext = new ManagerViewModel(Account) });
        }

        private void SaveCoAccountToFile()
        {
            if (Account != null & Account.Type == AccountType.Co)
            {
                logger.Info("Сохранение отдельного соу");
                List<Account> accounts = XmlHelper.LoadAccountsFromFile();
                if (accounts.All(a => a.Login != Account.Login || a.Server != Account.Server || a.Type != AccountType.Co || a.LoginCo != Account.LoginCo))
                {
                    accounts.Add(Account);
                    XmlHelper.SaveAccountsToFile(accounts);
                }
                MessageBox.Show(Properties.Resources.MainSaveAccountMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainErrorMessage4, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
