using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System;
using System.Threading;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BotQually
{
    public class MainViewModel : BaseViewModel
    {
        #region Command
        public ICommand AddAccCommand { get; set; }
        public ICommand RemoveAccCommand { get; set; }
        public ICommand ReloadFarmsCommand { get; set; }
        public ICommand AddFarmCommand { get; set; }
        public ICommand RemoveFarmCommand { get; set; }
        public ICommand ClearFarmCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand StartAllCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand StopAllCommand { get; set; }
        public ICommand LoginCoCommand { get; set; }
        public ICommand SaveAccCommand { get; set; }
        public ICommand LoadAccCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand ManagementCommand { get; set; }
        public ICommand StatusCommand { get; set; }
        public ICommand ReturnCommand { get; set; }
        public ICommand SaveCoAccountCommand { get; set; }
        #endregion

        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();
        public GlobalSettings GlobalSettings { get; set; }
        public Account SelectedAccount { get; set; }
        public Farm SelectedFarm { get; set; }
        public Farm SelectedQueFarm { get; set; }
        public bool GlobalIsRunning { get; set; } = false;
        public bool GlobalSettingsOpen { get; set; }
        public string GlobalAccountProgress { get; set; }
        public string GlobalFarmProgress { get; set; }
        public string GlobalProgress { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public int RunningCount { get; set; } = 0;
        public int DoneCount { get; set; } = 0;
        public bool NotLoading { get; set; } = true;
        public ObservableCollection<string> Notifications { get; set; } = new ObservableCollection<string>();

        private static MainViewModel instance;

        private MainViewModel() 
        {
            logger.Info("Начало сессии");
            AddAccCommand = new RelayCommand(() => AddAcc());
            RemoveAccCommand = new RelayCommand(() => RemoveAcc());
            AddFarmCommand = new RelayCommand(() => AddFarm());
            RemoveFarmCommand = new RelayCommand(() => RemoveFarm());
            ClearFarmCommand = new RelayCommand(() => ClearFarms());
            StartCommand = new RelayCommand(() => Start());
            StartAllCommand = new RelayCommand(() => StartAll());
            StopCommand = new RelayCommand(() => Stop());
            StopAllCommand = new RelayCommand(() => StopAll());
            LoginCoCommand = new RelayCommand(() => LoginCo());
            SaveAccCommand = new RelayCommand(() => SaveAccountsToFile());
            LoadAccCommand = new RelayCommand(() => LoadAccountsFromFile());
            SettingsCommand = new RelayCommand(() => OpenSettings());
            ManagementCommand = new RelayCommand(() => OpenManagement());
            StatusCommand = new RelayCommand(() => OpenStatus());
            ReturnCommand = new RelayCommand(() => ReturnToMain());
            SaveCoAccountCommand = new RelayCommand(() => SaveCoAccountToFile());
            Version = MainHelper.GetProgramVersion().Substring(0, MainHelper.GetProgramVersion().Length - 4);
            GlobalSettings = GlobalSettings.GetInstance();
        }

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }
            return instance;
        }

        private void AddAcc()
        {
            if ((GlobalSettings.WorkType == WorkType.GlobalOrder || GlobalSettings.WorkType == WorkType.SingleOrder) && GlobalIsRunning)
            {
                return;
            }
            if (!NotLoading)
            {
                return;
            }
            logger.Info("Добавление аккаунта");
            MainHelper.ShowPage(new LoginPage());
        }

        private async void RemoveAcc()
        {
            if (SelectedAccount != null)
            {
                if ((GlobalSettings.WorkType == WorkType.GlobalParallel || GlobalSettings.WorkType == WorkType.SingleParallel) && SelectedAccount.IsRunning)
                {
                    MessageBox.Show(Properties.Resources.MainDeleteAccError1, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if ((GlobalSettings.WorkType == WorkType.GlobalOrder || GlobalSettings.WorkType == WorkType.SingleOrder) && GlobalIsRunning)
                {
                    MessageBox.Show(Properties.Resources.MainDeleteAccError2, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                logger.Info("Удаление аккаунта");
                NotLoading = false;
                List<Account> accounts = XmlHelper.LoadAccountsFromFile();
                foreach (Account account in Accounts)
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
                if (SelectedAccount.Type == AccountType.Co)
                {
                    await SelectedAccount.LogOut(AccountType.Co);
                }
                await SelectedAccount.LogOut(AccountType.Normal);
                Accounts.Remove(SelectedAccount);
                NotLoading = true;
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainDeleteAccError3, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddFarm()
        {
            if (SelectedFarm != null && !SelectedAccount.IsRunning)
            {
                logger.Info("Добавление завода");
                SelectedAccount.Queue.Add(SelectedFarm);
                SelectedAccount.FarmsQueue.Add(SelectedFarm.Id);
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainAddFarmError, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveFarm()
        {
            if (SelectedQueFarm != null && !SelectedAccount.IsRunning)
            {
                logger.Info("Удаление завода");
                SelectedAccount.FarmsQueue.Remove(SelectedQueFarm.Id);
                SelectedAccount.Queue.Remove(SelectedQueFarm);
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainDeleteFarmError, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFarms()
        {
            if (!SelectedAccount.IsRunning)
            {
                logger.Info("Очистка заводов");
                SelectedAccount.Queue.Clear();
                SelectedAccount.FarmsQueue.Clear();
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainClearFarmsError, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private CancellationTokenSource cts;
        private async void Start()
        {
            LogSettings();
            if (!(((MainWindow)Application.Current.MainWindow).CurrentPage is StatusPage))
            {
                MainHelper.ShowPage(new StatusPage() { DataContext = this });
            }
            if ((GlobalSettings.WorkType == WorkType.GlobalParallel || GlobalSettings.WorkType == WorkType.SingleParallel) && !SelectedAccount.IsRunning)
            {
                logger.Info("Старт параллельного прогона");
                SelectedAccount.IsRunning = true;
                LoadCounts();
                await SelectedAccount.StartParallel(GlobalSettings).ConfigureAwait(false);
            }
            else if ((GlobalSettings.WorkType == WorkType.GlobalOrder || GlobalSettings.WorkType == WorkType.SingleOrder) && !GlobalIsRunning)
            {
                logger.Info("Старт последовательного прогона");
                Account accRef = new Account();
                try
                {
                    GlobalIsRunning = true;
                    cts = new CancellationTokenSource();
                    foreach (var account in Accounts)
                    {
                        account.IsRunning = true;
                    }
                    foreach (var account in Accounts)
                    {
                        accRef = account;
                        await account.StartOrder(cts.Token, GlobalSettings).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show($"{Properties.Resources.MainStoppedWorkMessage} \n{Properties.Resources.MainAccountTextMessage}: {accRef.Login}\n{Properties.Resources.MainFarmTextMessage}: {accRef.ProgressFarm}\n{Properties.Resources.MainHorseTextMessage}: {accRef.ProgressHorse}", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    foreach (var account in Accounts)
                    {
                        if (!account.IsDone)
                        {
                            account.Progress = "остановлен";
                        }
                        account.Client.Ct = new CancellationToken();
                    }
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Notifications.Add($"{Properties.Resources.MainNotificationCancelMessage} {Properties.Resources.MainNotificationAccountMessage}: {accRef.Login}; {Properties.Resources.MainNotificationFarmMessage}: {accRef.ProgressFarm}; {Properties.Resources.MainNotificationHorseMessage}: {accRef.ProgressHorse}");
                    });
                }
                catch (Exception e)
                {
                    logger.Info("Ошибка последовательного прогона");
                    MessageBox.Show(Properties.Resources.MainWorkErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error($"Ошибка последовательного: {e.Message}\n{e.StackTrace}");
                    logger.Error($"Данные аккаунта {accRef.Login} Сервер:{accRef.Server}; Тип:{accRef.Type}; Экю:{accRef.Equ}; Фураж:{accRef.Hay.Amount}; Овес:{accRef.Oat.Amount};");
                    if (accRef.MainProductToSell != null)
                    {
                        logger.Error($"Основной продукт {accRef.MainProductToSell.Type}:{accRef.MainProductToSell.Amount};");
                    }
                    if (accRef.SubProductToSell != null)
                    {
                        logger.Error($"Саб продукт{accRef.SubProductToSell}:{accRef.SubProductToSell.Amount}");
                    }
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Notifications.Add($"{Properties.Resources.MainNotificationErrorMessage} {Properties.Resources.MainNotificationAccountMessage}: {accRef.Login}; {Properties.Resources.MainNotificationFarmMessage}: {accRef.ProgressFarm}; {Properties.Resources.MainNotificationHorseMessage}: {accRef.ProgressHorse}");
                    });
                    foreach (var account in Accounts)
                    {
                        if (!account.IsDone)
                        {
                            account.Progress = "ошибка";
                        }
                    }
                }
                finally
                {
                    logger.Info("Окончание последовательного прогона");
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Notifications.Add(Properties.Resources.MainNotificationEndWorkMessage);
                    });
                    foreach (var account in Accounts)
                    {
                        account.IsRunning = false;
                    }
                    GlobalIsRunning = false;
                }
            }
        }

        private void LogSettings()
        {
            logger.Info($"Настройки:\n Сортировка:{GlobalSettings.Sort}\nТип работы:{GlobalSettings.WorkType}\nТип интернета:{GlobalSettings.ClientType}\nПараллельные лошади:{GlobalSettings.ParallelHorse}\nРандомные паузы:{GlobalSettings.RandomPause}");
        }

        private void Stop()
        {
            if (GlobalSettings.WorkType == WorkType.GlobalParallel || GlobalSettings.WorkType == WorkType.SingleParallel)
            {
                if (SelectedAccount != null && SelectedAccount.IsRunning && !SelectedAccount.Cts.IsCancellationRequested)
                {
                    logger.Info("Остановка пареллельного прогона");
                    SelectedAccount.Cts.Cancel();
                }
            }
            else
            {
                if (cts != null && GlobalIsRunning && !cts.IsCancellationRequested)
                {
                    logger.Info("Остановка последовательного прогона");
                    cts.Cancel();
                }
            }
        }

        #pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        private async void StartAll()
        {
            if (GlobalSettings.WorkType == WorkType.GlobalParallel || GlobalSettings.WorkType == WorkType.SingleParallel)
            {
                MainHelper.ShowPage(new StatusPage() { DataContext = this });
                logger.Info("Запуск всех параллельного прогона");
                foreach (var account in Accounts)
                {
                    if (!account.IsRunning)
                    {
                        #pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                        account.StartParallel(GlobalSettings);
                        #pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                        LoadCounts();
                    }
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainStartAllErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void StopAll()
        {
            if (GlobalSettings.WorkType == WorkType.GlobalParallel || GlobalSettings.WorkType == WorkType.SingleParallel)
            {
                logger.Info("Остановка всех параллельного прогона");
                foreach (var account in Accounts)
                {
                    if (account.IsRunning && !account.Cts.IsCancellationRequested)
                    {
                        account.Cts.Cancel();
                        account.Client.Ct = new CancellationToken();
                    }
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainStartAllErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void LoginCo()
        {
            if (SelectedAccount != null && !SelectedAccount.IsRunning)
            {
                if (SelectedAccount.Type == AccountType.Co)
                {
                    NotLoading = false;
                    logger.Info("Выход из соу");
                    await SelectedAccount.LogOut(AccountType.Co);
                    SelectedAccount.AddSavedFarms();
                    NotLoading = true;
                }
                else
                {
                    logger.Info("Вход в соу");
                    MainHelper.ShowPage(new LoginCoPage() { DataContext = new LoginCoViewModel(SelectedAccount) });
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainLoginCoErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenSettings()
        {
            if (SelectedAccount != null)
            {
                logger.Info("Открытие настроек");
                if (GlobalSettings.WorkType == WorkType.SingleOrder || GlobalSettings.WorkType == WorkType.SingleParallel)
                {
                    MainHelper.ShowPage(new SettingsPage() { DataContext = new SettingsViewModel(SelectedAccount.PrivateSettings.Copy() as Settings) });
                }
                else
                {
                    MainHelper.ShowPage(new SettingsPage() { DataContext = new SettingsViewModel(GlobalSettings.Settings.Copy() as Settings) });
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainOpenSettingsErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenManagement()
        {
            if (SelectedAccount != null)
            {
                logger.Info("Открытие управления");
                MainHelper.ShowPage(new ManagementPage() { DataContext = new ManagementViewModel(SelectedAccount) });
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainOpenManagmentErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenStatus()
        {
            logger.Info("Открытие статуса");
            MainHelper.ShowPage(new StatusPage() { DataContext = this });
        }

        private void ReturnToMain()
        {
            logger.Info("Возвращение из статуса");
            MainHelper.ShowPage(new MainPage());
        }

        private void SaveAccountsToFile()
        {
            logger.Info("Сохранение аккаунтов в файл");
            if (GlobalSettings.WorkType == WorkType.GlobalOrder || GlobalSettings.WorkType == WorkType.GlobalParallel)
            {
                foreach (var account in Accounts)
                {
                    account.PrivateSettings = GlobalSettings.Settings;
                }
            }
            List<Account> accounts = new List<Account>(Accounts);
            var dialog = new SaveFileDialog
            {
                Filter = "XML files(.xml)|*.xml|all Files(*.*)|*.*",
            };
            if (dialog.ShowDialog() == true)
            {
                XmlHelper.SaveAccountsToFile(accounts, dialog.FileName);
            }
        }

        private async void LoadAccountsFromFile()
        {
            logger.Info("Загрузка аккаунтов из файла");
            bool chk = false;
            foreach (var acc in Accounts)
            {
                if (acc.IsRunning)
                {
                    chk = true;
                }
            }
            if (!chk && !GlobalIsRunning)
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "XML files(.xml)|*.xml|all Files(*.*)|*.*",
                };
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        ObservableCollection<Account> accounts = new ObservableCollection<Account>(XmlHelper.LoadAccountsFromFile(dialog.FileName));
                        int i = 0;
                        NotLoading = false;
                        try
                        {
                            foreach (Account account in accounts)
                            {
                                if (GlobalSettings.WorkType == WorkType.GlobalOrder || GlobalSettings.WorkType == WorkType.GlobalParallel)
                                {
                                    account.Settings = GlobalSettings.Settings;
                                }
                                else
                                {
                                    account.Settings = account.PrivateSettings;
                                }
                                Status = $"Загрузка {i + 1} из {accounts.Count}";
                                if (account.Type == AccountType.Co)
                                {
                                    string login = account.LoginCo;
                                    string loginCo = account.Login;
                                    account.Login = login;
                                    await account.LogIn(true);
                                    if (!await account.LogInCo(loginCo, true))
                                    {
                                        MessageBox.Show($"{Properties.Resources.MainLoadAccErrorMessage1} {account.Login}!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }
                                }
                                else
                                {
                                    await account.LogIn(true);
                                }
                                account.AddSavedFarms();
                                i++;
                            }
                            Status = Path.GetFileName(dialog.FileName);
                            foreach (var acc in accounts)
                            {
                                Accounts.Add(acc);
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show($"{Properties.Resources.MainLoadAccErrorMessage2} {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            NotLoading = true;
                        }
                    }
                    catch
                    {
                        MessageBox.Show(Properties.Resources.MainLoadAccErrorMessage3, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void LoadCounts()
        {
            RunningCount = 0;
            DoneCount = 0;
            foreach (var account in Accounts)
            {
                if (account.IsDone)
                {
                    DoneCount++;
                }
                if (account.IsRunning)
                {
                    RunningCount++;
                }
            }
        }

        private void SaveCoAccountToFile()
        {
            if (SelectedAccount != null & SelectedAccount.Type == AccountType.Co)
            {
                logger.Info("Сохранение отдельного соу");
                List<Account> accounts = XmlHelper.LoadAccountsFromFile();
                if (accounts.All(a => a.Login != SelectedAccount.Login || a.Server != SelectedAccount.Server || a.Type != AccountType.Co || a.LoginCo != SelectedAccount.LoginCo))
                {
                    accounts.Add(SelectedAccount);
                    XmlHelper.SaveAccountsToFile(accounts);
                }
                MessageBox.Show(Properties.Resources.MainSaveCoAccMessage, "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(Properties.Resources.MainSaveCoAccErrorMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
