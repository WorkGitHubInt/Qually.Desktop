using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System;
using System.Threading;
using Ninject;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Reflection;

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
        public Account SelectedAccount { get; set; }
        public Farm SelectedFarm { get; set; }
        public Farm SelectedQueFarm { get; set; }
        public GlobalSettings GlobalSettings { get; set; } = IoC.Kernel.Get<GlobalSettings>();
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

        public MainViewModel()
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
            SetGlobalSettings();
            Version = typeof(MainWindow).Assembly.GetName().Version.ToString().Substring(0, typeof(MainWindow).Assembly.GetName().Version.ToString().Length - 4);
            //CheckInternet();
        }

        private async void CheckInternet()
        {
            //while (true)
            //{
            //    //if (await CheckConnection())
            //    //{
            //    //    Status = "Потеряно интернет соединение...";
            //    //}
            //    await Task.Delay(1000);
            //}
        }

        public async Task<bool> CheckConnection()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var result = await client.GetAsync("https://botqually.ru");
                    result.EnsureSuccessStatusCode();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void SetGlobalSettings()
        {
            GlobalSettings.Sort = Properties.Settings.Default.Sort;
            GlobalSettings.WorkType = (WorkType)Properties.Settings.Default.WorkType;
            GlobalSettings.ParallelHorse = Properties.Settings.Default.ParallelHorse;
            GlobalSettings.RandomPause = Properties.Settings.Default.RandomPause;
            GlobalSettings.Tray = Properties.Settings.Default.Tray;
            GlobalSettings.FemaleNames = Properties.Resources.FemaleNames.Split(',');
            GlobalSettings.MaleNames = Properties.Resources.MaleNames.Split(',');
            GlobalSettings.ClientType = (ClientType)Properties.Settings.Default.ClientType;
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
            ((MainWindow)Application.Current.MainWindow).ShowPage(new LoginPage());
        }

        private async void RemoveAcc()
        {
            if (SelectedAccount != null)
            {
                if ((GlobalSettings.WorkType == WorkType.GlobalParallel || GlobalSettings.WorkType == WorkType.SingleParallel) && SelectedAccount.IsRunning)
                {
                    MessageBox.Show("Выбранный аккаунт находится в состоянии прогона!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if ((GlobalSettings.WorkType == WorkType.GlobalOrder || GlobalSettings.WorkType == WorkType.SingleOrder) && GlobalIsRunning)
                {
                    MessageBox.Show("Невозможно удалять аккаунты в состоянии прогона!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                logger.Info("Удаление аккаунта");
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
                    await SelectedAccount.LogOut(AccountType.Normal);
                }
                else
                {
                    await SelectedAccount.LogOut(AccountType.Normal);
                }
                Accounts.Remove(SelectedAccount);
            }
            else
            {
                MessageBox.Show("Не был выбран аккаунт!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Не был выбран завод или аккаунт находится в состоянии прогона!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Не был выбран завод или аккаунт находится в состоянии прогона!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Аккаунт находится в состоянии прогона!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private CancellationTokenSource cts;
        private async void Start()
        {
            LogSettings();
            if (!(((MainWindow)Application.Current.MainWindow).CurrentPage is StatusPage))
            {
                ((MainWindow)Application.Current.MainWindow).ShowPage(new StatusPage()
                {
                    DataContext = this
                });
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
                    MessageBox.Show($"Отменено \nАккаунт: {accRef.Login}\nЗавод: {accRef.ProgressFarm}\nЛошадь: {accRef.ProgressHorse}", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    foreach (var account in Accounts)
                    {
                        if (!account.IsDone)
                        {
                            account.Progress = "остановлен";
                        }
                    }
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Notifications.Add($"Отмена Аккаунт: {accRef.Login}; Завод: {accRef.ProgressFarm}; Лошадь: {accRef.ProgressHorse}");
                    });
                }
                catch (Exception e)
                {
                    logger.Info("Ошибка последовательного прогона");
                    MessageBox.Show($"Во время прогона произошла ошибка! Все необходимые данные были записаны. Не забудьте отправить отчет.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error($"Ошибка последовательного: {e.Message}\n{e.StackTrace}");
                    logger.Error($"Данные аккаунта {accRef.Login} Сервер:{accRef.Server}; Тип:{accRef.Type}; Экю:{accRef.Equ}; Фураж:{accRef.Hay.Amount}; Овес:{accRef.Oat.Amount}; Основной продукт {accRef.MainProductToSell.Type}:{accRef.MainProductToSell.Amount}; Саб продукт{accRef.SubProductToSell}:{accRef.SubProductToSell.Amount}");
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Notifications.Add($"Ошибка Аккаунт: {accRef.Login}; Завод: {accRef.ProgressFarm}; Лошадь: {accRef.ProgressHorse}");
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
                        Notifications.Add($"Окончен прогон по очереди");
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
                    foreach (var acc in Accounts)
                    {
                        acc.Client.Ct = new CancellationToken();
                    }
                }
            }
        }

        private async void StartAll()
        {
            ((MainWindow)Application.Current.MainWindow).ShowPage(new StatusPage()
            {
                DataContext = this
            });
            if (GlobalSettings.WorkType == WorkType.GlobalParallel || GlobalSettings.WorkType == WorkType.SingleParallel)
            {
                logger.Info("Запуск всех параллельного прогона");
                foreach (var account in Accounts)
                {
                    if (!account.IsRunning)
                    {
                        account.StartParallel(GlobalSettings);
                        LoadCounts();
                    }
                }
            }
            else
            {
                MessageBox.Show("У вас включен режим прогона по очереди! Действия по этой кнопке производиться не будут!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("У вас включен режим прогона по очереди! Действия по этой кнопке производиться не будут!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void LoginCo()
        {
            if (SelectedAccount != null && !SelectedAccount.IsRunning)
            {
                if (SelectedAccount.Type == AccountType.Co)
                {
                    logger.Info("Выход из соу");
                    await SelectedAccount.LogOut(AccountType.Co);
                    SelectedAccount.AddSavedFarms();
                }
                else
                {
                    logger.Info("Вход в соу");
                    ((MainWindow)Application.Current.MainWindow).ShowPage(new LoginCoPage()
                    {
                        DataContext = new LoginCoViewModel(SelectedAccount)
                    });
                }
            }
            else
            {
                MessageBox.Show("Не был выбран аккаунт или аккаунт находится в состоянии прогона!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenSettings()
        {
            if (SelectedAccount != null)
            {
                logger.Info("Открытие настроек");
                if (GlobalSettings.WorkType == WorkType.SingleOrder || GlobalSettings.WorkType == WorkType.SingleParallel)
                {
                    ((MainWindow)Application.Current.MainWindow).ShowPage(new SettingsPage()
                    {
                        DataContext = new SettingsViewModel(SelectedAccount.PrivateSettings.Clone() as Settings)
                    });
                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).ShowPage(new SettingsPage()
                    {
                        DataContext = new SettingsViewModel(GlobalSettings.Settings.Clone() as Settings)
                    });
                }
            }
            else
            {
                MessageBox.Show("Не был выбран аккаунт!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenManagement()
        {
            if (SelectedAccount != null)
            {
                logger.Info("Открытие управления");
                ((MainWindow)Application.Current.MainWindow).ShowPage(new ManagementPage()
                {
                    DataContext = new ManagementViewModel(SelectedAccount)
                });
            }
            else
            {
                MessageBox.Show("Не был выбран аккаунт!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenStatus()
        {
            logger.Info("Открытие статуса");
            ((MainWindow)Application.Current.MainWindow).ShowPage(new StatusPage()
            {
                DataContext = this
            });
        }

        private void ReturnToMain()
        {
            logger.Info("Возвращение из статуса");
            ((MainWindow)Application.Current.MainWindow).ShowPage(new MainPage());
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
                                        MessageBox.Show($"Не удалось войти в соу {account.Login}!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                            MessageBox.Show($"Произошла ошибка при загрузке аккаунтов! {e.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            NotLoading = true;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка загрузки файла! Файлы можно использовать только те, которые были созданы этой версией!", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Аккаунт сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выбранный аккаунт не соу!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
