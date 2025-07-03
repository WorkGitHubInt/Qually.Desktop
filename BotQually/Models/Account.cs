using System;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using System.Xml.Serialization;
using System.Web;
using System.Threading;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using System.Collections.Generic;
using System.Windows;

namespace BotQually
{
    public class Account : BaseModel
    {
        #region Ingorable Props
        [XmlIgnore]
        public Dictionary<Server, string> Servers = new Dictionary<Server, string>
        {
            { Server.Australia, "https://au.howrse.com"}, //
            { Server.England, "https://www.howrse.co.uk"}, //
            { Server.Arabic, "https://ar.howrse.com" },
            { Server.Bulgaria, "https://www.howrse.bg" },
            { Server.International, "https://www.howrse.com" }, //
            { Server.Spain, "https://www.caballow.com" },
            { Server.Canada,"https://ca.howrse.com" },
            { Server.Germany, "https://www.howrse.de" },
            { Server.Norway, "https://www.howrse.no" }, //
            { Server.Poland, "https://www.howrse.pl" },
            { Server.Russia, "https://www.lowadi.com" },
            { Server.Romain, "https://www.howrse.ro" }, //
            { Server.USA, "https://us.howrse.com" }, //
            { Server.FranceOuranos, "https://ouranos.equideow.com" },
            { Server.FranceGaia, "https://gaia.equideow.com" },
            { Server.Sweden, "https://www.howrse.se" } //
        };
        [XmlIgnore]
        public IClient Client { get; private set; }
        [XmlIgnore]
        public Product Hay { get; private set; }
        [XmlIgnore]
        public Product Oat { get; private set; }
        [XmlIgnore]
        public Product Wheat { get; private set; }
        [XmlIgnore]
        public Product Shit { get; private set; }
        [XmlIgnore]
        public Product Leather { get; private set; }
        [XmlIgnore]
        public Product Apples { get; private set; }
        [XmlIgnore]
        public Product Carrot { get; private set; }
        [XmlIgnore]
        public Product Wood { get; private set; }
        [XmlIgnore]
        public Product Steel { get; private set; }
        [XmlIgnore]
        public Product Sand { get; private set; }
        [XmlIgnore]
        public Product Straw { get; private set; }
        [XmlIgnore]
        public Product Flax { get; private set; }
        [XmlIgnore]
        public Product OR { get; private set; }
        [XmlIgnore]
        public Product MainProductToSell { get; private set; }
        [XmlIgnore]
        public Product SubProductToSell { get; private set; }
        [XmlIgnore]
        public int Equ { get; private set; }
        [XmlIgnore]
        public ObservableCollection<Farm> Farms { get; private set; } = new ObservableCollection<Farm>();
        [XmlIgnore]
        public ObservableCollection<Farm> Queue { get; set; } = new ObservableCollection<Farm>();
        [XmlIgnore]
        public ObservableCollection<string> CoAccounts { get; set; } = new ObservableCollection<string>();
        [XmlIgnore]
        public bool IsRunning { get; set; } = false;
        [XmlIgnore]
        public bool IsLoading { get; set; } = false;
        [XmlIgnore]
        public bool IsDone { get; set; } = false;
        [XmlIgnore]
        public CancellationTokenSource Cts { get; set; }
        [XmlIgnore]
        public string Progress { get; set; }
        [XmlIgnore]
        public string ProgressFarm { get; set; }
        [XmlIgnore]
        public string ProgressHorse { get; set; }
        [XmlIgnore]
        public Settings Settings { get; set; }
        [XmlIgnore]
        public ObservableCollection<string> Notifications { get; set; } = new ObservableCollection<string>();
        [XmlIgnore]
        public string OREND { get; set; }
        [XmlIgnore]
        public bool IsEquMessageShown { get; set; } = false;
        [XmlIgnore]
        public string Pass { get; set; }
        #endregion

        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string Login { get; set; }
        public string Password { get; set; }
        public Server Server { get; set; } = Server.Russia;
        public AccountType Type { get; set; } = AccountType.Normal;
        public Settings PrivateSettings { get; set; } = new Settings();
        public string LoginCo { get; set; }
        public string ProxyIP { get; set; } = string.Empty;
        public string ProxyLogin { get; set; } = string.Empty;
        public string ProxyPassword { get; set; } = string.Empty;

        public List<string> FarmsQueue { get; set; } = new List<string>();

        public virtual async Task<bool> LogIn(bool load)
        {
            string answer;
            string baseAddress = Servers[Server];
            WebProxy proxy = null;
            if (ProxyIP != string.Empty)
            {
                proxy = new WebProxy($"http://{ProxyIP}/", true);
                if (ProxyLogin != string.Empty && ProxyPassword != string.Empty)
                {
                    proxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPassword);
                }
            }
            if (Server == Server.FranceGaia || Server == Server.FranceOuranos)
            {
                Client = new Client("https://www.equideow.com", proxy);
                answer = await Client.GetAsync("/");
                var document = Parser.ParseDocument(answer);
                string name = document.GetElementById("authentification").Children[1].GetAttribute("name").ToLower();
                string value = document.GetElementById("authentification").Children[1].GetAttribute("value").ToLower();
                string server;
                if (Server == Server.FranceOuranos)
                {
                    server = "ouranos";
                }
                else
                {
                    server = "gaia";
                }
                string postData = $"{name}={value}&to={server}&login={HttpUtility.UrlEncode(Login)}&password={HttpUtility.UrlEncode(Password)}&redirection=&isBoxStyle=";
                answer = await Client.PostAsync("/site/doLogIn", postData);
                if (answer.Contains("?identification=1") || answer.Contains("\"errors\":[]"))
                {
                    string url = Regex.Match(answer, "\"redirection\":\"(.*?)\"}").Groups[1].Value.Replace("\\", "");
                    string query = url.Substring(baseAddress.Length);
                    Client = new Client(baseAddress, proxy);
                    await Client.GetAsync(query);
                    Client.SetSID();
                    if (load)
                    {
                        await LoadInfo();
                    }
                    return true;
                }
                return false;
            }
            else
            {
                Client = new Client(baseAddress, proxy);
                var document = Parser.ParseDocument(await Client.GetAsync("/"));
                string name = document.GetElementById("authentification").Children[1].GetAttribute("name").ToLower();
                string value = document.GetElementById("authentification").Children[1].GetAttribute("value").ToLower();
                string postData = $"{name}={value}&login={HttpUtility.UrlEncode(Login)}&password={HttpUtility.UrlEncode(Password)}&redirection=&isBoxStyle=";
                answer = await Client.PostAsync("/site/doLogIn", postData);
                if (answer.Contains("?identification=1") || answer.Contains("\"errors\":[]"))
                {
                    if (load)
                    {
                        await LoadInfo();
                    }
                    Client.SetSID();
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> LogInCo(string loginCo, bool load)
        {
            var document = Parser.ParseDocument(await Client.GetAsync("/member/account/?type=sharing"));
            IHtmlCollection<IElement> tables = document.QuerySelectorAll(".table--striped");
            IHtmlCollection<IElement> names = tables[1].QuerySelectorAll(".usergroup_2");
            IHtmlCollection<IElement> buttons = tables[1].QuerySelectorAll(".btn--primary");
            int i = 0;
            foreach (var item in names)
            {
                if (!buttons[i].ClassName.Contains("btn--disabled"))
                {
                    if (item.TextContent == loginCo)
                    {
                        var script = buttons[i].ParentElement.GetElementsByTagName("script")[0];
                        string id = Regex.Match(script.InnerHtml, "{'params':'user=(.*?)'}").Groups[1].Value;
                        string answer = await Client.PostAsync("/site/doLogInCoAccount", $"user={id}");
                        if (answer.Contains("shareModeAlreadyUsed") || answer.Contains("shareModeNotAuthorized"))
                        {
                            return false;
                        }
                        Type = AccountType.Co;
                        string login = Login;
                        Login = loginCo;
                        LoginCo = login;
                        if (load)
                        {
                            await LoadInfo();
                        }
                        return true;
                    }
                }
                i++;
            }
            return false;
        }

        public async Task LoadCoAccounts()
        {
            var document = Parser.ParseDocument(await Client.GetAsync("/member/account/?type=sharing"));
            var tables = document.QuerySelectorAll(".table--striped");
            if (tables.Length >= 2)
            {
                var names = tables[1].QuerySelectorAll(".usergroup_2");
                var buttons = tables[1].QuerySelectorAll(".btn--primary");
                if (names.Length == 0)
                {
                    throw new Exception();
                }
                else
                {
                    int i = 0;
                    int j = 0;
                    foreach (var item in names)
                    {
                        if (!buttons[i].ClassName.Contains("btn--disabled"))
                        {
                            CoAccounts.Add(item.TextContent);
                            j++;
                        }
                        i++;
                    }
                    if (j == 0)
                    {
                        throw new Exception();
                    }
                }
            }
            else
            {
                throw new Exception();
            }
        }

        public virtual async Task LogOut(AccountType type)
        {
            if (type == AccountType.Normal)
            {
                if (PrivateSettings.Sharing)
                {
                    await Client.PostAsync($"/member/account/doEnableShareMode", $"sid={Client.SID}");
                }
                else
                {
                    await Client.PostAsync($"/site/doLogOut", $"sid={Client.SID}");
                }
            }
            else
            {
                await Client.PostAsync($"/site/doLogOutCoAccount", $"sid={Client.SID}");
                Type = AccountType.Normal;
                Login = LoginCo;
                LoginCo = "";
                await LoadInfo();
            }
        }

        public virtual async Task LoadInfo()
        {
            string baseAddress = Servers[Server];
            Hay = new Product(baseAddress, ProductType.Hay, 1);
            Oat = new Product(baseAddress, ProductType.Oat, 1);
            Wheat = new Product(baseAddress, ProductType.Wheat, 1);
            Shit = new Product(baseAddress, ProductType.Shit, 3);
            Leather = new Product(baseAddress, ProductType.Leather, 5);
            Apples = new Product(baseAddress, ProductType.Apples, 3);
            Carrot = new Product(baseAddress, ProductType.Carrot, 10);
            Wood = new Product(baseAddress, ProductType.Wood, 5);
            Steel = new Product(baseAddress, ProductType.Steel, 5);
            Sand = new Product(baseAddress, ProductType.Sand, 5);
            Straw = new Product(baseAddress, ProductType.Straw, 10);
            Flax = new Product(baseAddress, ProductType.Flax, 25);
            OR = new Product(baseAddress, ProductType.OR, 0);
            SetMainProductToSell();
            SetSubProductToSell();
            await LoadProducts();
            await LoadFarms();
        }

        public void SetMainProductToSell()
        {
            switch (Settings.MainProductToSell)
            {
                case ProductType.Hay:
                    MainProductToSell = Hay;
                    break;
                case ProductType.Wheat:
                    MainProductToSell = Wheat;
                    break;
                case ProductType.Leather:
                    MainProductToSell = Leather;
                    break;
                case ProductType.Apples:
                    MainProductToSell = Apples;
                    break;
                case ProductType.Carrot:
                    MainProductToSell = Carrot;
                    break;
                case ProductType.Wood:
                    MainProductToSell = Wood;
                    break;
                case ProductType.Steel:
                    MainProductToSell = Steel;
                    break;
                case ProductType.Sand:
                    MainProductToSell = Sand;
                    break;
                case ProductType.Straw:
                    MainProductToSell = Straw;
                    break;
                case ProductType.Flax:
                    MainProductToSell = Flax;
                    break;
            }
        }

        public void SetSubProductToSell()
        {
            switch (Settings.SubProductToSell)
            {
                case ProductType.Hay:
                    SubProductToSell = Hay;
                    break;
                case ProductType.Wheat:
                    SubProductToSell = Wheat;
                    break;
                case ProductType.Leather:
                    SubProductToSell = Leather;
                    break;
                case ProductType.Apples:
                    SubProductToSell = Apples;
                    break;
                case ProductType.Carrot:
                    SubProductToSell = Carrot;
                    break;
                case ProductType.Wood:
                    SubProductToSell = Wood;
                    break;
                case ProductType.Steel:
                    SubProductToSell = Steel;
                    break;
                case ProductType.Sand:
                    SubProductToSell = Sand;
                    break;
                case ProductType.Straw:
                    SubProductToSell = Straw;
                    break;
                case ProductType.Flax:
                    SubProductToSell = Flax;
                    break;
            }
        }

        public async Task LoadProducts()
        {
            var document = Parser.ParseDocument(await Client.GetAsync("/marche/boutiqueVendre"));
            CheckValues(document, Hay);
            CheckValues(document, Oat);
            CheckValues(document, Wheat);
            CheckValues(document, Shit);
            CheckValues(document, Leather);
            CheckValues(document, Apples);
            CheckValues(document, Carrot);
            CheckValues(document, Wood);
            CheckValues(document, Steel);
            CheckValues(document, Sand);
            CheckValues(document, Straw);
            CheckValues(document, Flax);
            CheckValues(document, OR);
            Pass = document.QuerySelector("#pass").TextContent;
            if (document.QuerySelector("#reserve") != null)
            {
                Equ = Convert.ToInt32(document.QuerySelector("#reserve").GetAttribute("data-amount"));
            }
        }

        private void CheckValues(IHtmlDocument document, Product product)
        {
            if (document.QuerySelector($"#inventaire{product.Id}") != null)
            {
                product.Amount = Convert.ToInt32(document.QuerySelector($"#inventaire{product.Id}").TextContent);
            }
            else
            {
                product.Amount = 0;
            }
        }

        public async Task LoadFarms()
        {
            Farms.Clear();
            Queue.Clear();
            var document = Parser.ParseDocument(await Client.GetAsync("/elevage/chevaux/?elevage=all-horses"));
            var tabs = document.QuerySelectorAll(".tab-action-select");
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].GetAttribute("id") == "new-breeding" || (tabs[i].GetAttribute("alt") != null && tabs[i].GetAttribute("alt") == "+"))
                {
                    continue;
                }
                if (tabs[i].GetAttribute("id") == null)
                {
                    Farms.Add(new Farm(tabs[i].TextContent.Trim(), "", this));
                }
                else
                {
                    Farms.Add(new Farm(tabs[i].TextContent.Trim(), tabs[i].GetAttribute("id").Split('-')[2], this));
                }
            }
        }

        public void AddSavedFarms()
        {
            for (int j = 0; j < FarmsQueue.Count; j++)
            {
                for (int k = 0; k < Farms.Count; k++)
                {
                    if (Farms[k].Id == FarmsQueue[j])
                    {
                        Queue.Add(Farms[k]);
                    }
                }
            }
        }

        private void LogSettings()
        {
            logger.Info($"Аккаунт {Login} Настройки:\nСлучки ж:{Settings.HorsingFemale}\nЦена случки ж:{Settings.HorsingFemalePrice}\nЗаводчик:{Settings.Breeder}\nЧистокровные:{Settings.ClearBlood}\nСвои ж:{Settings.SelfMale}\nПокупать пшеницу:{Settings.BuyWheat}\nСлучки м:{Settings.HorsingMale}\nЦена случки м:{Settings.HorsingMalePrice}\nМорковь:{Settings.Carrot}\nИмя м:{Settings.MaleName}\nИмя ж:{Settings.FemaleName}\nАффикс:{Settings.Affix}\nРандомные имена:{Settings.RandomNames}\nКск длительность:{Settings.CentreDuration}\nКск фураж:{Settings.CentreHay}\nКск овес:{Settings.CentreOat}\nРезерв ID:{Settings.ReserveID}\nРезерв длительность:{Settings.ReserveDuration}\nПродливать:{Settings.Continue}\nДлительность продления:{Settings.ContinueDuration}\nСвой резерв:{Settings.SelfReserve}\nПисать в общие:{Settings.WriteToAll}\nПокупка фуража:{Settings.BuyHay}\nПокупка овса:{Settings.BuyOat}\nОсновной продукт:{Settings.MainProductToSell}\nВторостепенные продукт:{Settings.SubProductToSell}\nПродавать навоз:{Settings.SellShit}\nМиссия:{Settings.Mission}\nСтарые лошади:{Settings.OldHorses}\nЗдоровье:{Settings.HealthEdge}\nПропуск лошадей:{Settings.SkipIndex}\nЗагружать спящих:{Settings.LoadSleep}");
        }

        public async Task StartParallel(GlobalSettings globalSettings)
        {
            try
            {
                IsEquMessageShown = false;
                int OrBegin = OR.Amount;
                Progress = "0%";
                ClientSwitch(globalSettings);
                IsRunning = true;
                IsDone = false;
                Cts = new CancellationTokenSource();
                Client.Ct = Cts.Token;
                await LoadProducts().ConfigureAwait(false);
                await InitialActions().ConfigureAwait(false);
                foreach (var farm in Queue)
                {
                    ProgressFarm = farm.Name;
                    await farm.Run(globalSettings, Cts.Token).ConfigureAwait(false);
                }
                Progress = string.Empty;
                OREND = (OR.Amount - OrBegin).ToString();
                IsDone = true;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show($"{Properties.Resources.MainNotificationCancelMessage} \n{Properties.Resources.MainNotificationAccountMessage}: {Login}\n{Properties.Resources.MainNotificationFarmMessage}: {ProgressFarm}\n{Properties.Resources.MainNotificationHorseMessage}: {ProgressHorse}", "", MessageBoxButton.OK, MessageBoxImage.Information);
                Progress = Properties.Resources.MainProgressCancelMessage;
                Client.Ct = new CancellationToken();
                logger.Info($"Аккаунт: {Login} остановлен параллельный прогон");
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MainViewModel.GetInstance().Notifications.Add($"{Properties.Resources.MainNotificationCancelMessage} {Properties.Resources.MainNotificationAccountMessage}: {Login}; {Properties.Resources.MainNotificationFarmMessage}: {ProgressFarm}; {Properties.Resources.MainNotificationHorseMessage}: {ProgressHorse}");
                });
            }
            catch (Exception e)
            {
                logger.Info($"Аккаунт: {Login} ошибка параллельный прогон");
                MessageBox.Show($"{Properties.Resources.StartAllErrorMessage1} {Login} {Properties.Resources.StartAllErrorMessage2}: {ProgressFarm}; {Properties.Resources.MainNotificationHorseMessage}: {ProgressHorse}. {Properties.Resources.StartAllErrorMessage3}", "", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Error($"Аккаунт: {Login} Ошибка пареллельного: {e.Message}\n{e.StackTrace}");
                logger.Error($"Данные аккаунта {Login} Сервер:{Server}; Тип:{Type}; Экю:{Equ}; Фураж:{Hay.Amount}; Овес:{Oat.Amount}; Основной продукт {MainProductToSell.Type}:{MainProductToSell.Amount}; Саб продукт{SubProductToSell}:{SubProductToSell.Amount}");
                Progress = Properties.Resources.MainProgressErrorMessage;
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MainViewModel.GetInstance().Notifications.Add($"{Properties.Resources.MainNotificationErrorMessage} {Properties.Resources.MainNotificationAccountMessage}: {Login}; {Properties.Resources.MainNotificationFarmMessage}: {ProgressFarm}; {Properties.Resources.MainNotificationHorseMessage}: {ProgressHorse}");
                });
            }
            finally
            {
                logger.Info($"Аккаунт: {Login} окончен параллельный прогон");
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MainViewModel.GetInstance().Notifications.Add($"{Properties.Resources.MainNotificationAccountMessage}: {Login} {Properties.Resources.AccountNotificationEndMessage} {OREND}");
                });
                IsRunning = false;
                EndActions();
            }
        }

        public async Task StartOrder(CancellationToken ct, GlobalSettings globalSettings)
        {
            IsEquMessageShown = false;
            if (Type == AccountType.Co)
            {
                string login = LoginCo;
                string loginCo = Login;
                Login = login;
                await LogIn(false).ConfigureAwait(false);
                await LogInCo(loginCo, false).ConfigureAwait(false);
                Login = loginCo;
                LoginCo = login;
            }
            else
            {
                await LogIn(false).ConfigureAwait(false);
            }
            int OrBegin = OR.Amount;
            Progress = "0%";
            ClientSwitch(globalSettings);
            IsDone = false;
            Client.Ct = ct;
            await LoadProducts().ConfigureAwait(false);
            await InitialActions().ConfigureAwait(false);
            foreach (var farm in Queue)
            {
                ProgressFarm = farm.Name;
                await farm.Run(globalSettings, ct).ConfigureAwait(false);
            }
            IsDone = true;
            OREND = (OR.Amount - OrBegin).ToString();
            Application.Current.Dispatcher.Invoke(delegate
            {
                MainViewModel.GetInstance().Notifications.Add($"{Properties.Resources.MainNotificationAccountMessage}: {Login} {Properties.Resources.AccountNotificationEndMessage} {OREND}");
            });
            EndActions();
        }

        private async Task InitialActions()
        {
            logger.Info($"Аккаунт {Login} Заводы:");
            foreach (var farm in Queue)
            {
                logger.Info($"{Login}:{farm.Name}");
            }
            LogSettings();
            if (Settings.SellShit)
            {
                await Sell(Shit, Shit.Amount.ToString()).ConfigureAwait(false);
            }
            if (Settings.BuyWheat)
            {
                await Buy(Wheat, Math.Round((double)(Equ - 8900) / 2).ToString()).ConfigureAwait(false);
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                LoadProducts();
            }
        }

        private void EndActions()
        {
            MainViewModel.GetInstance().LoadCounts();
        }

        private void ClientSwitch(GlobalSettings globalSettings)
        {
            if (globalSettings.ClientType == ClientType.Old && Client is Client)
            {
                RequestClient requestClient = new RequestClient
                {
                    BaseAddress = (Client as Client).HttpClient.BaseAddress.OriginalString,
                    CookieContainer = (Client as Client).Handler.CookieContainer,
                    Proxy = (Client as Client).Handler.Proxy as WebProxy,
                };
                Client = requestClient;
            }
            else if (globalSettings.ClientType == ClientType.New && Client is RequestClient)
            {
                Client client = new Client((Client as RequestClient).BaseAddress, (Client as RequestClient).Proxy, (Client as RequestClient).CookieContainer, Client.SID);
                Client = client;
            }
        }

        public async Task Sell(Product product, string quantity)
        {
            await Client.PostAsync("/marche/vente", $"id={product.Id}&nombre={quantity}&mode=eleveur").ConfigureAwait(false);
        }

        public async Task Buy(Product product, string quantity)
        {
            int total = Convert.ToInt32(quantity);
            int count = total / 100000;
            for (int i = 0; i < count; i++)
            {
                await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre=100000&typeRedirection=&idElement=").ConfigureAwait(false);
                total -= 100000;
            }
            if (total / 10000 * 10000 > 0)
            {
                await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 10000 * 10000}&typeRedirection=&idElement=").ConfigureAwait(false);
                total -= total / 10000 * 10000;
            }
            if (total / 1000 * 1000 > 0)
            {
                await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 1000 * 1000}&typeRedirection=&idElement=").ConfigureAwait(false);
                total -= total / 1000 * 1000;
            }
            if (total / 100 * 100 > 0)
            {
                await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 100 * 100}&typeRedirection=&idElement=").ConfigureAwait(false);
                total -= total / 100 * 100;
            }
            if (total / 10 * 10 > 0)
            {
                await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 10 * 10}&typeRedirection=&idElement=").ConfigureAwait(false);
                total -= total / 10 * 10;
            }
            if (total > 0)
            {
                await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total}&typeRedirection=&idElement=").ConfigureAwait(false);
            }
        }

        public Account Copy()
        {
            return MemberwiseClone() as Account;
        }
    }
}
