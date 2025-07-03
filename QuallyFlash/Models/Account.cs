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
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace QuallyFlash
{
    public class Account : BaseModel
    {
        #region Ignorable Members
        [XmlIgnore]
        public Dictionary<Server, string> Servers = new Dictionary<Server, string>
        {
            { Server.Australia, "https://au.howrse.com"},
            { Server.England, "https://www.howrse.co.uk"},
            { Server.Arabic, "https://ar.howrse.com" },
            { Server.Bulgaria, "https://www.howrse.bg" },
            { Server.International, "https://www.howrse.com" },
            { Server.Spain, "https://www.caballow.com" },
            { Server.Canada,"https://ca.howrse.com" },
            { Server.Germany, "https://www.howrse.de" },
            { Server.Norway, "https://www.howrse.no" },
            { Server.Poland, "https://www.howrse.pl" },
            { Server.Russia, "https://www.lowadi.com" },
            { Server.Romain, "https://www.howrse.ro" },
            { Server.USA, "https://us.howrse.com" },
            { Server.FranceOuranos, "https://ouranos.equideow.com" },
            { Server.FranceGaia, "https://gaia.equideow.com" },
            { Server.Sweden, "https://www.howrse.se" }
        };
        [XmlIgnore]
        public IClient Client { get; private set; }
        #region Products
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
        public Product Mash { get; private set; }
        #endregion
        #region Equipment
        [XmlIgnore]
        public Equipment SaddleClassic1 { get; private set; }
        [XmlIgnore]
        public Equipment SaddleClassic2 { get; private set; }
        [XmlIgnore]
        public Equipment SaddleClassic3 { get; private set; }
        [XmlIgnore]
        public Equipment SaddleWestern1 { get; private set; }
        [XmlIgnore]
        public Equipment SaddleWestern2 { get; private set; }
        [XmlIgnore]
        public Equipment SaddleWestern3 { get; private set; }
        [XmlIgnore]
        public Equipment BridleClassic1 { get; private set; }
        [XmlIgnore]
        public Equipment BridleClassic2 { get; private set; }
        [XmlIgnore]
        public Equipment BridleClassic3 { get; private set; }
        [XmlIgnore]
        public Equipment BridleWestern1 { get; private set; }
        [XmlIgnore]
        public Equipment BridleWestern2 { get; private set; }
        [XmlIgnore]
        public Equipment BridleWestern3 { get; private set; }
        [XmlIgnore]
        public Equipment RampClassic { get; private set; }
        [XmlIgnore]
        public Equipment RampWestern { get; private set; }
        [XmlIgnore]
        public Equipment Bandages { get; private set; }
        [XmlIgnore]
        public Equipment Forehead { get; private set; }
        [XmlIgnore]
        public Equipment SaddleToShow { get; private set; }
        [XmlIgnore]
        public Equipment BridleToShow { get; private set; }
        [XmlIgnore]
        public Equipment RampToShow { get; private set; }
        #endregion
        [XmlIgnore]
        public Product MainProductToSell { get; private set; }
        [XmlIgnore]
        public Product SubProductToSell { get; private set; }
        [XmlIgnore]
        public int Equ { get; private set; }
        [XmlIgnore]
        public ObservableCollection<Farm> Farms { get; private set; } = new ObservableCollection<Farm>();
        [XmlIgnore]
        public ObservableCollection<string> CoAccounts { get; set; } = new ObservableCollection<string>();
        [XmlIgnore]
        public bool IsWorking { get; set; } = false;
        [XmlIgnore]
        public CancellationTokenSource Cts { get; set; }
        [XmlIgnore]
        public int HorsesCount { get; set; } = 0;
        #endregion

        #region Public Members
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string Login { get; set; }
        public string Password { get; set; }
        public Server Server { get; set; } = Server.Australia;
        public AccountType Type { get; set; } = AccountType.Normal;
        public string LoginCo { get; set; }
        public Settings Settings { get; set; } = new Settings();
        public HorseSex NextHorseSex { get; set; }
        public string ProxyIP { get; set; } = string.Empty;
        public string ProxyLogin { get; set; } = string.Empty;
        public string ProxyPassword { get; set; } = string.Empty;
        #endregion

        #region Private Members
        [XmlIgnore]
        public Horse MaleHorse { get; set; }
        [XmlIgnore]
        public Horse FemaleHorse { get; set; }
        private int limit;
        [XmlIgnore]
        public int CurrentValue { get; set; }
        private Task maleHorseTask;
        #endregion

        #region Account Actions
        public async Task<bool> LogIn()
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
                    await LoadInfo();
                    return true;
                }
                return false;
            }
            else
            {
                Client = new Client(baseAddress, proxy)
                {
                    Account = this
                };
                var document = Parser.ParseDocument(await Client.GetAsync("/"));
                string name = document.GetElementById("authentification").Children[1].GetAttribute("name").ToLower();
                string value = document.GetElementById("authentification").Children[1].GetAttribute("value").ToLower();
                string postData = $"{name}={value}&login={HttpUtility.UrlEncode(Login)}&password={HttpUtility.UrlEncode(Password)}&redirection=&isBoxStyle=";
                answer = await Client.PostAsync("/site/doLogIn", postData);
                if (answer.Contains("?identification=1") || answer.Contains("\"errors\":[]"))
                {
                    Client.SetSID();
                    await LoadInfo();
                    return true;
                }
                return false;
            }
        }

        public async Task<CookieContainer> ReLogIn()
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
                var client = new Client("https://www.equideow.com", proxy);
                answer = await client.GetAsync("/");
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
                answer = await client.PostAsync("/site/doLogIn", postData);
                if (answer.Contains("?identification=1") || answer.Contains("\"errors\":[]"))
                {
                    string url = Regex.Match(answer, "\"redirection\":\"(.*?)\"}").Groups[1].Value.Replace("\\", "");
                    string query = url.Substring(baseAddress.Length);
                    client = new Client(baseAddress, proxy);
                    await client.GetAsync(query);
                    return (client as Client).Handler.CookieContainer;
                }
            }
            else
            {
                var client = new Client(baseAddress, proxy)
                {
                    Account = this
                };
                var document = Parser.ParseDocument(await client.GetAsync("/"));
                string name = document.GetElementById("authentification").Children[1].GetAttribute("name").ToLower();
                string value = document.GetElementById("authentification").Children[1].GetAttribute("value").ToLower();
                string postData = $"{name}={value}&login={HttpUtility.UrlEncode(Login)}&password={HttpUtility.UrlEncode(Password)}&redirection=&isBoxStyle=";
                answer = await client.PostAsync("/site/doLogIn", postData);
                if (answer.Contains("?identification=1") || answer.Contains("\"errors\":[]"))
                {
                    return (client as Client).Handler.CookieContainer;
                }
            }
            return null;
        }

        public async Task<bool> LogInCo(string loginCo, bool load)
        {
            var document = Parser.ParseDocument(await Client.GetAsync("/member/account/?type=sharing"));
            IHtmlCollection<IElement> names;
            IHtmlCollection<IElement> buttons;
            IHtmlCollection<IElement> tables = document.QuerySelectorAll(".table--striped");
            names = tables[1].QuerySelectorAll(".usergroup_2");
            buttons = tables[1].QuerySelectorAll(".btn--primary");
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

        public async Task LogOut(AccountType type)
        {
            if (type == AccountType.Normal)
            {
                await Client.PostAsync($"/site/doLogOut", $"sid={Client.SID}");
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

        public async Task LoadInfo()
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
            Mash = new Product(baseAddress, ProductType.Mash, 0);
            SaddleClassic1 = new Equipment(baseAddress, EquipmentType.SaddleClassic1, 667 + 500);
            SaddleClassic2 = new Equipment(baseAddress, EquipmentType.SaddleClassic2, 667 + 500);
            SaddleClassic3 = new Equipment(baseAddress, EquipmentType.SaddleClassic3, 4167 + 500);
            SaddleWestern1 = new Equipment(baseAddress, EquipmentType.SaddleWestern1, 667 + 500);
            SaddleWestern2 = new Equipment(baseAddress, EquipmentType.SaddleWestern2, 770 + 500);
            SaddleWestern3 = new Equipment(baseAddress, EquipmentType.SaddleWestern3, 4414 + 500);
            BridleClassic1 = new Equipment(baseAddress, EquipmentType.BridleClassic1, 267 + 500);
            BridleClassic2 = new Equipment(baseAddress, EquipmentType.BridleClassic2, 495 + 500);
            BridleClassic3 = new Equipment(baseAddress, EquipmentType.BridleClassic3, 2130 + 500);
            BridleWestern1 = new Equipment(baseAddress, EquipmentType.BridleWestern1, 267 + 500);
            BridleWestern2 = new Equipment(baseAddress, EquipmentType.BridleWestern2, 535 + 500);
            BridleWestern3 = new Equipment(baseAddress, EquipmentType.BridleWestern3, 2137 + 500);
            RampClassic = new Equipment(baseAddress, EquipmentType.RampClassic, 267 + 500);
            RampWestern = new Equipment(baseAddress, EquipmentType.RampWestern, 532 + 500);
            Bandages = new Equipment(baseAddress, EquipmentType.Bandages, 267 + 500);
            Forehead = new Equipment(baseAddress, EquipmentType.Forehead, 267 + 500);
            SetMainProductToSell();
            SetSubProductToSell();
            await LoadProducts();
            await LoadEquipment();
            SetEquipmentToShow();
            await LoadFarms();
        }

        public void SetEquipmentToShow()
        {
            if (Settings.Specialization == Specialization.Classic)
            {
                switch (Settings.Amunition)
                {
                    case 1:
                        SaddleToShow = SaddleClassic1;
                        BridleToShow = BridleClassic1;
                        break;
                    case 2:
                        SaddleToShow = SaddleClassic2;
                        BridleToShow = BridleClassic2;
                        break;
                    case 3:
                        SaddleToShow = SaddleClassic3;
                        BridleToShow = BridleClassic3;
                        break;
                }
                RampToShow = RampClassic;
            }
            else
            {
                switch (Settings.Amunition)
                {
                    case 1:
                        SaddleToShow = SaddleWestern1;
                        BridleToShow = BridleWestern1;
                        break;
                    case 2:
                        SaddleToShow = SaddleWestern2;
                        BridleToShow = BridleWestern2;
                        break;
                    case 3:
                        SaddleToShow = SaddleWestern3;
                        BridleToShow = BridleWestern3;
                        break;
                }
                RampToShow = RampWestern;
            }
        }

        public void SetMainProductToSell()
        {
            switch (Settings.MainProductToSell)
            {
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
            var document = Parser.ParseDocument(await Client.GetAsync("/marche/boutiqueVendre").ConfigureAwait(false));
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
            CheckValues(document, Mash);
            Equ = Convert.ToInt32(document.QuerySelector("#reserve").GetAttribute("data-amount"));
            if (IsWorking && Settings.LimitType == Limit.OR && OR.Amount < Settings.Limit)
            {
                Cts.Cancel();
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, "Работа была остановлена! ОР опустилось ниже установленной отметки!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Работа была остановлена! ОР опустилось ниже установленной отметки!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
            }
            if (IsWorking && OR.Amount <= 0)
            {
                Cts.Cancel();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    MessageBox.Show(Application.Current.MainWindow, "Работа была остановлена! Закончилось ОР!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }));
            }
        }

        public async Task LoadEquipment()
        {
            var document = Parser.ParseDocument(await Client.GetAsync("/marche/boutiqueVendre").ConfigureAwait(false));
            CheckValues(document, SaddleClassic1);
            CheckValues(document, SaddleClassic2);
            CheckValues(document, SaddleClassic3);
            CheckValues(document, SaddleWestern1);
            CheckValues(document, SaddleWestern2);
            CheckValues(document, SaddleWestern3);
            CheckValues(document, BridleClassic1);
            CheckValues(document, BridleClassic2);
            CheckValues(document, BridleClassic3);
            CheckValues(document, BridleWestern1);
            CheckValues(document, BridleWestern2);
            CheckValues(document, BridleWestern3);
            CheckValues(document, RampClassic);
            CheckValues(document, RampWestern);
            CheckValues(document, Bandages);
            CheckValues(document, Forehead);
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

        private void CheckValues(IHtmlDocument document, Equipment equipment)
        {
            if (document.QuerySelector($"#inventaire{equipment.Id}") != null)
            {
                equipment.Amount = Convert.ToInt32(document.QuerySelector($"#inventaire{equipment.Id}").TextContent);
            }
            else
            {
                equipment.Amount = 0;
            }
        }

        public async Task LoadFarms()
        {
            Farms.Clear();
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

        public async Task Sell(Product product, string quantity)
        {
            await Client.PostAsync("/marche/vente", $"id={product.Id}&nombre={quantity}&mode=eleveur");
        }

        public async Task Buy(Product product, string quantity)
        {
            int total = Convert.ToInt32(quantity);
            for (int i = 0; i < total / 100000; i++)
            {
                await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre=100000&typeRedirection=&idElement=");
                total -= 100000;
            }
            await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 10000 * 10000}&typeRedirection=&idElement=");
            total -= total / 10000 * 10000;
            await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 1000 * 1000}&typeRedirection=&idElement=");
            total -= total / 1000 * 1000;
            await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 100 * 100}&typeRedirection=&idElement=");
            total -= total / 100 * 100;
            await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total / 10 * 10}&typeRedirection=&idElement=");
            total -= total / 10 * 10;
            await Client.PostAsync("/marche/achat", $"id={product.Id}&mode=eleveur&nombre={total}&typeRedirection=&idElement=");
        }

        public async Task Buy(Equipment equipment, string nombre)
        {
            await Client.PostAsync("/marche/achat", $"id={equipment.Id}&mode=eleveur&nombre={nombre}&typeRedirection=&idElement=");
        }

        private void ClientSwitch()
        {
            if (Client is Client)
            {
                RequestClient requestClient = new RequestClient
                {
                    BaseAddress = (Client as Client).HttpClient.BaseAddress.OriginalString,
                    CookieContainer = (Client as Client).Handler.CookieContainer,
                    Proxy = (Client as Client).Handler.Proxy as WebProxy,
                    Account = this,
                };
                requestClient.SetSID();
                Client = requestClient;
            }
        }

        private void LogSettings()
        {
            logger.Info($"Настройки:\nИгра:{Settings.Game}\nМиссия после 2:{Settings.MissionAfter2}\nМиссия после трен.:{Settings.MissionAfterTrain}\nАборты:{Settings.Abortion}\nТип лимита:{Settings.LimitType}\nЛимит:{Settings.Limit}\nСлучки с:{Settings.HorsingEdge}\nЗдоровье:{Settings.HealthEdge}\nСпециализация:{Settings.Specialization}\nАмуниция:{Settings.Amunition}\nНалобник:{Settings.Headrest}\nБинты:{Settings.Bandages}\nХлыст:{Settings.Whip}\nТяжеловозы:{Settings.Heavy}\nСхема:{Settings.SchemeType}\nПараллельная пара:{Settings.ParallelPair}\nТип тренировки:{Settings.TrainType}\nКол-во случек:{Settings.HorsingNum}\nИмя м:{Settings.MaleName}\nИмя ж:{Settings.FemaleName}\nАффикс:{Settings.Affix}\nЗавод:{Settings.Farm}\nИмя скилла:{Settings.NameSkill}\nКск длительность:{Settings.Duration}\nКск резерв:{Settings.Reserve}\nСвой резерв:{Settings.SelfReserve}\nМорковь:{Settings.Carrot}\nСмесь:{Settings.Mash}\nСедла:{Settings.Saddle}\nУздечки:{Settings.Bridle}\nЛес:{Settings.Forest}\nГоры:{Settings.Mountain}\nПляж:{Settings.Beach}\nВальтрап:{Settings.Ramp}\nДуш:{Settings.Shower}\nПоилка:{Settings.Bowl}\nВыписывать:{Settings.WriteOut}\nПокупка еды:{Settings.BuyFood}\nПокупка глав.:{Settings.MainProductToSell}\nПокупка саб:{Settings.SubProductToSell}\nПокупка морковь/Смесь:{Settings.BuyCarrotMash}");
            string scheme = "";
            foreach (var training in Settings.Scheme)
            {
                scheme += training.Name + "\n";
            }
            logger.Info("Схема:" + scheme);
        }
        #endregion

        #region Training
        public async Task StartTraining(ObservableCollection<Horse> horses)
        {
            if (Settings.WriteOut && (!Settings.Reserve || !Settings.SelfReserve))
            {
                MessageBox.Show(Properties.Resources.AccountMessage1, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            LogSettings();
            Cts = new CancellationTokenSource();
            ClientSwitch();
            Client.Ct = Cts.Token;
            IsWorking = true;
            HorsesCount = 0;
            if (Settings.SchemeType == SchemeType.HalfPair)
            {
                if (horses[0].Sex == HorseSex.Male)
                {
                    MaleHorse = horses[0];
                    await TrainHalfPair(MaleHorse).ConfigureAwait(false);
                }
                else
                {
                    FemaleHorse = horses[0];
                    await TrainHalfPair(FemaleHorse).ConfigureAwait(false);
                }
            }
            else
            {
                if (horses[0].Sex == HorseSex.Male)
                {
                    MaleHorse = horses[0];
                    FemaleHorse = horses[1];
                }
                else
                {
                    FemaleHorse = horses[0];
                    MaleHorse = horses[1];
                }
                await TrainPair().ConfigureAwait(false);
            }
            IsWorking = false;
        }

        public async Task TrainHalfPair(Horse horse)
        {
            logger.Info("Старт полупары");
            CurrentValue = (Settings.LimitType == Limit.OR) ? Settings.Limit : 0;
            limit = (Settings.LimitType == Limit.OR) ? OR.Amount : Settings.Limit;
            NextHorseSex = (horse.Sex == HorseSex.Male) ? HorseSex.Female : HorseSex.Male;
            while (limit > CurrentValue)
            {
                horse = await TrainHalf(horse).ConfigureAwait(false);
            }
        }

        public async Task TrainPair()
        {
            logger.Info("Старт пары");
            CurrentValue = (Settings.LimitType == Limit.OR) ? Settings.Limit : 0;
            limit = (Settings.LimitType == Limit.OR) ? OR.Amount : Settings.Limit;
            while (limit > CurrentValue)
            {
                if (!Settings.ParallelPair)
                {
                    await TrainHalf(MaleHorse).ConfigureAwait(false);
                    await TrainHalf(FemaleHorse).ConfigureAwait(false);
                }
                else
                {
                    maleHorseTask = TrainHalf(MaleHorse);
                    await TrainHalf(FemaleHorse).ConfigureAwait(false);
                }
            }
        }

        public async Task<Horse> TrainHalf(Horse horse)
        {
            var document = await horse.GetDoc().ConfigureAwait(false);
            document = await TurnOffAuto(document, horse).ConfigureAwait(false);
            if (CheckAuto(document))
            {
                int horsingLimit = (Settings.HorsingEdge == HorsingEdge.NLNP) ? 60 : 120;
                double horsingValue = (Settings.HorsingEdge == HorsingEdge.NLNP) ? horse.NLNP : horse.Age;
                document = await horse.GetDoc().ConfigureAwait(false);
                var a = document.QuerySelector("#competition-body-content").QuerySelectorAll("a");
                if (horse.Age > 120 && a[0].ClassName.Contains("disabled"))
                {
                    Cts.Cancel();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, Properties.Resources.AccountStopMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
                if (Settings.SelfReserve && Settings.WriteOut && horse.Age >= 6 && horse.Age <= 96 && document.QuerySelector("#cheval-inscription") == null && document.QuerySelectorAll("#center-tab-main .button-text-3").Length == 0)
                {
                    Cts.Cancel();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, Properties.Resources.AccountStopMessage1, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
                horse.LoadInfo(document);
                await horse.Rename(document).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                LoadEquipment().ConfigureAwait(false);
                if (Settings.Whip)
                {
                    await horse.GiveWhip().ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                }
                while (horsingLimit > horsingValue)
                {
                    document = await TrainingCycle(document, horse, false).ConfigureAwait(false);
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                    LoadProducts().ConfigureAwait(false);
                    horsingLimit = (Settings.HorsingEdge == HorsingEdge.NLNP) ? 60 : 120;
                    horsingValue = (Settings.HorsingEdge == HorsingEdge.NLNP) ? horse.NLNP : horse.Age;
                }
                if (horse.Sex == HorseSex.Male)
                {
                    if (Settings.TrainType == TrainType.Team)
                    {
                        horse = await HorsingMale(horse).ConfigureAwait(false);
                    }
                    else
                    {
                        if (Settings.SchemeType == SchemeType.HalfPair)
                        {
                            horse = await horse.GetNextHorse(document).ConfigureAwait(false);
                        }
                    }
                    HorsesCount++;
                    if (Settings.SchemeType == SchemeType.HalfPair)
                    {
                        FemaleHorse = horse;
                    }
                    if (Settings.LimitType == Limit.Horses)
                    {
                        CurrentValue++;
                    }
                }
                else
                {
                    if (Settings.Heavy)
                    {
                        try
                        {
                            heavyHorsing = Convert.ToInt32(document.QuerySelectorAll("#reproduction-tab-1 .first.dashed.last.col-0")[0].QuerySelectorAll("strong")[0].TextContent);
                        }
                        catch
                        {
                            heavyHorsing = 0;
                        }
                    }
                    if (Settings.SchemeType == SchemeType.HalfPair)
                    {
                        horse = await BornCertain(document, horse).ConfigureAwait(false);
                        if (NextHorseSex == HorseSex.Male)
                        {
                            FemaleHorse = horse;
                        }
                        else
                        {
                            MaleHorse = horse;
                        }
                    }
                    else
                    {
                        if (Settings.ParallelPair)
                        {
                            await maleHorseTask;
                        }
                        Horse babyHorse1 = await BornAny(document, horse).ConfigureAwait(false);
                        NextHorseSex = babyHorse1.Sex == HorseSex.Male ? HorseSex.Female : HorseSex.Male;
                        document = await horse.GetDoc().ConfigureAwait(false);
                        Horse babyHorse2 = await BornCertain(document, horse).ConfigureAwait(false);
                        if (babyHorse1.Sex == HorseSex.Male)
                        {
                            MaleHorse = babyHorse1;
                        }
                        else
                        {
                            FemaleHorse = babyHorse1;
                        }
                        if (babyHorse2.Sex == HorseSex.Male)
                        {
                            MaleHorse = babyHorse2;
                        }
                        else
                        {
                            FemaleHorse = babyHorse2;
                        }
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            MainViewModel.GetInstance().Horses[0] = MaleHorse;
                            MainViewModel.GetInstance().Horses[1] = FemaleHorse;
                            MainViewModel.GetInstance().SelectedSexHorse = FemaleHorse;
                        });
                    }
                }
                if (Settings.SchemeType == SchemeType.HalfPair)
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MainViewModel.GetInstance().Horses[0] = horse;
                        MainViewModel.GetInstance().SelectedSexHorse = horse;
                    });
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.AccountStopMessage2, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                Cts.Cancel();
            }
            return horse;
        }

        public async Task<IHtmlDocument> TurnOffAuto(IHtmlDocument document, Horse horse)
        {
            try
            {
                if (document.QuerySelector("#walk-head-title").QuerySelectorAll("a").Length > 0)
                {
                    if (document.QuerySelector("#walk-head-title").QuerySelectorAll("a")[0].ClassName == "widget-action config on vip")
                    {
                        await Client.PostAsync("/elevage/chevaux/doConfigureGlobal", $"baladeSimplifie=1&id={horse.Id}").ConfigureAwait(false);
                    }
                }
                if (document.GetElementById("training-head-title").GetElementsByTagName("a")[0].ClassName == "widget-action config on")
                {
                    await Client.PostAsync("/elevage/chevaux/doConfigureGlobal", $"entrainementSimplifie=1&id={horse.Id}").ConfigureAwait(false);
                }
            }
            catch { }//TODO проблема с авто выключением
            return await horse.GetDoc().ConfigureAwait(false);
        }

        public bool CheckAuto(IHtmlDocument document)
        {
            if (document.QuerySelector("#walk-head-title").QuerySelectorAll("a").Length > 0)
            {
                if (document.QuerySelector("#walk-head-title").QuerySelectorAll("a")[0].ClassName == "widget-action config on vip")
                {
                    return false;
                }
            }
            if (document.QuerySelector("#training-head-title").QuerySelectorAll("a")[0].ClassName == "widget-action config on")
            {
                return false;
            }
            return true;
        }

        private async Task Training(Horse horse, IHtmlDocument document)
        {
            if (horse.Age < 18 && Settings.Game)
            {
                await horse.Game(document).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
            }
            else if (horse.Age >= 18)
            {
                if (horse.Age >= 36 && !horse.IsSpecialized && document.QuerySelector("#specialisationClassique") != null)
                {
                    await horse.Specialization().ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    await LoadEquipment().ConfigureAwait(false);
                    await horse.Equipment().ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                }
                else if (horse.Age >= 36 && !horse.IsEquiped)
                {
                    await horse.Equipment().ConfigureAwait(false);
                }
                await horse.Training(document).ConfigureAwait(false);
            }
        }

        private async Task<IHtmlDocument> TrainingCycle(IHtmlDocument document, Horse horse, bool simple)
        {
            await horse.Action(document, "form-do-groom", "#boutonPanser", "doGroom").ConfigureAwait(false);
            await Pause().ConfigureAwait(false);
            if (horse.Age >= 6 && document.QuerySelector("#cheval-inscription") != null)
            {
                if (Settings.Reserve)
                {
                    await horse.CentreReserve().ConfigureAwait(false);
                }
                else
                {
                    await horse.Centre().ConfigureAwait(false);
                }
                await Pause().ConfigureAwait(false);
                document = await horse.GetDoc().ConfigureAwait(false);
            }
            if (Settings.MissionAfter2 || (Settings.MissionAfterTrain && horse.Scheme.All(t => t.IsDone == true)))
            {
                await horse.Mission().ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
            }
            if (horse.Age >= 8 && !horse.Scheme.All(t => t.IsDone == true) && !simple)
            {
                await Training(horse, document).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await horse.Action(document, "form-do-stroke", "#boutonCaresser", "doStroke").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await horse.Action(document, "form-do-drink", "#boutonBoire", "doDrink").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                if (Carrot.Amount < 100 && Settings.BuyCarrotMash)
                {
                    if (Equ < 21000)
                    {
                        if (Settings.MainProductToSell != ProductType.None && MainProductToSell.Amount * MainProductToSell.SellPrice > 21000)
                        {
                            await Sell(MainProductToSell, (21000 / MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            await Pause().ConfigureAwait(false);
                        }
                        else if (Settings.SubProductToSell != ProductType.None && SubProductToSell.Amount * SubProductToSell.SellPrice > 21000)
                        {
                            await Sell(SubProductToSell, (21000 / MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            await Pause().ConfigureAwait(false);
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {Login} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }));
                        }
                    }
                    await Buy(Carrot, "1000").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                }
                await horse.Action(document, "form-do-eat-treat-carotte", "#boutonCarotte", "doEatTreat").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                if (horse.Age >= 24 && horse.CurrentTraining != null)
                {
                    if (horse.CurrentTraining.Value != "plege" && horse.CurrentTraining.Value != "lynxcompet" && horse.CurrentTraining.Value != "galopcompet" && horse.CurrentTraining.Value != "cross" && horse.CurrentTraining.Value != "concur" && horse.CurrentTraining.Value != "barell" && horse.CurrentTraining.Value != "cutting" && horse.CurrentTraining.Value != "dressage")
                    {
                        if (Mash.Amount < 100 && Settings.BuyCarrotMash)
                        {
                            if (Equ < 71000)
                            {
                                if (Settings.MainProductToSell != ProductType.None && MainProductToSell.Amount * MainProductToSell.SellPrice > 68000)
                                {
                                    await Sell(MainProductToSell, (68000 / MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                                    await Pause().ConfigureAwait(false);
                                }
                                else if (Settings.SubProductToSell != ProductType.None && SubProductToSell.Amount * SubProductToSell.SellPrice > 68000)
                                {
                                    await Sell(SubProductToSell, (68000 / MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                                    await Pause().ConfigureAwait(false);
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                    {
                                        MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {Login} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }));
                                }
                            }
                            await Buy(Mash, "1000").ConfigureAwait(false);
                            await Pause().ConfigureAwait(false);
                        }
                        await horse.Action(document, "form-do-eat-treat-mash", "#boutonMash", "doEatTreat").ConfigureAwait(false);
                        await Pause().ConfigureAwait(false);
                    }
                }
                document = await horse.GetDoc().ConfigureAwait(false);
                await horse.Feeding(document).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await Training(horse, document).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
            }
            else
            {
                document = await horse.GetDoc().ConfigureAwait(false);
                await horse.Feeding(document).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
            }
            //document = await horse.GetDoc().ConfigureAwait(false);
            await horse.Action(document, "form-do-night", "#boutonCoucher", "doNight").ConfigureAwait(false);
            await Pause().ConfigureAwait(false);
            document = await horse.GetDoc().ConfigureAwait(false);
            await horse.Action(document, "age", "#boutonVieillir", "doAge").ConfigureAwait(false);
            await Pause().ConfigureAwait(false);
            document = await horse.GetDoc().ConfigureAwait(false);
            horse.NLNP = Convert.ToDouble(document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[3].TextContent);
            horse.Age += 2;
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            LoadProducts().ConfigureAwait(false);
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            return document;
        }

        public async Task<Horse> HorsingMale(Horse horse)
        {
            int horsingNum = Settings.Heavy ? 3 : Settings.HorsingNum;
            IHtmlDocument document = await horse.GetDoc().ConfigureAwait(false);
            while (horsingNum > 0)
            {
                document = await horse.GetDoc().ConfigureAwait(false);
                await horse.Action(document, "form-do-groom", "#boutonPanser", "doGroom").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                horsingNum = await horse.HorsingMale(horsingNum).ConfigureAwait(false);
                if (horsingNum > 0)
                {
                    await horse.Action(document, "form-do-stroke", "#boutonCaresser", "doStroke").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    await horse.Action(document, "form-do-drink", "#boutonBoire", "doDrink").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    await horse.Action(document, "form-do-eat-treat-carotte", "#boutonCarotte", "doEatTreat").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    await horse.Action(document, "form-do-eat-treat-mash", "#boutonMash", "doEatTreat").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    document = await horse.GetDoc().ConfigureAwait(false);
                    await horse.Feeding(document).ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    horsingNum = await horse.HorsingMale(horsingNum).ConfigureAwait(false);
                    await horse.Action(document, "form-do-night", "#boutonCoucher", "doNight").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    if (horsingNum > 0)
                    {
                        document = await horse.GetDoc().ConfigureAwait(false);
                        await horse.Action(document, "age", "#boutonVieillir", "doAge").ConfigureAwait(false);
                        await Pause().ConfigureAwait(false);
                        horse.Age += 2;
                    }
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                    LoadProducts().ConfigureAwait(false);
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                }
            }
            if (Settings.SchemeType == SchemeType.HalfPair)
            {
                horse = await horse.GetNextHorse(document).ConfigureAwait(false);
            }
            return horse;
        }

        public async Task RepeatHorsingMale(string femaleId)
        {
            var document = await MaleHorse.GetDoc().ConfigureAwait(false);
            MaleHorse.LoadInfo(document);
            if (MaleHorse.Energy - 25 > 20)
            {
                await MaleHorse.HorsingMaleWithoutTeam(femaleId).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
            }
            else if (document.QuerySelector("#boutonCoucher").ClassName.Contains("action-disabled"))
            {
                await MaleHorse.Action(document, "age", "#boutonVieillir", "doAge").ConfigureAwait(false);
                MaleHorse.Age += 2;
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                LoadProducts().ConfigureAwait(false);
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                await MaleHorse.HorsingMaleWithoutTeam(femaleId).ConfigureAwait(false);
            }
            else
            {
                await MaleHorse.Action(document, "form-do-groom", "#boutonPanser", "doGroom").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await MaleHorse.Action(document, "form-do-stroke", "#boutonCaresser", "doStroke").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await MaleHorse.Action(document, "form-do-drink", "#boutonBoire", "doDrink").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await MaleHorse.Action(document, "form-do-eat-treat-carotte", "#boutonCarotte", "doEatTreat").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await MaleHorse.Action(document, "form-do-eat-treat-mash", "#boutonMash", "doEatTreat").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                document = await MaleHorse.GetDoc().ConfigureAwait(false);
                await MaleHorse.Feeding(document).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await MaleHorse.HorsingMaleWithoutTeam(femaleId).ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
                await MaleHorse.Action(document, "form-do-night", "#boutonCoucher", "doNight").ConfigureAwait(false);
                await Pause().ConfigureAwait(false);
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                LoadProducts().ConfigureAwait(false);
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            }
        }

        private async Task Pause()
        {
            Random rnd = new Random();
            if (Settings.Pause)
            {
                int pause = rnd.Next(100, 500);
                await Task.Delay(pause).ConfigureAwait(false);
            }
        }

        #region Female Functions
        private async Task<Horse> BornCertain(IHtmlDocument document, Horse momHorse)
        {
            Horse babyHorse = null;
            HorseSex babyHorseSex;
            do
            {
                if (document.QuerySelectorAll("#reproduction-tab-0 a").Last().Id != "boutonEchographie" && document.QuerySelectorAll("#reproduction-tab-0 a").Last().ClassName.Contains("disabled"))
                {
                    document = await AgeToHorsing(document, momHorse).ConfigureAwait(false);
                    if (Settings.TrainType == TrainType.Team)
                    {
                        await momHorse.HorsingFemale().ConfigureAwait(false);
                    }
                    else
                    {
                        await momHorse.HorsingFemaleWithoutTeam(document).ConfigureAwait(false);
                    }
                    document = await AgeToEcho(document, momHorse).ConfigureAwait(false);
                    babyHorseSex = await momHorse.Echography().ConfigureAwait(false);
                }
                else if (document.QuerySelectorAll("#reproduction-tab-0 a").Last().Id != "boutonEchographie" && !document.QuerySelectorAll("#reproduction-tab-0 a").Last().ClassName.Contains("disabled"))
                {
                    if (Settings.TrainType == TrainType.Team)
                    {
                        await momHorse.HorsingFemale().ConfigureAwait(false);
                    }
                    else
                    {
                        await momHorse.HorsingFemaleWithoutTeam(document).ConfigureAwait(false);
                    }
                    document = await AgeToEcho(document, momHorse).ConfigureAwait(false);
                    babyHorseSex = await momHorse.Echography().ConfigureAwait(false);
                }
                else if (document.QuerySelector("#boutonEchographie") != null && document.QuerySelector("#boutonEchographie").ClassName.Contains("disabled") && document.QuerySelectorAll("#reproduction-tab-1 .col-1").Length > 0)
                {
                    babyHorseSex = momHorse.Echography(document);
                }
                else
                {
                    document = await momHorse.GetDoc().ConfigureAwait(false);
                    momHorse.LoadInfo(document);
                    document = await AgeToEcho(document, momHorse).ConfigureAwait(false);
                    babyHorseSex = await momHorse.Echography().ConfigureAwait(false);
                }
                document = await AgeToBirth(document, momHorse).ConfigureAwait(false);
                if (babyHorseSex == NextHorseSex || !Settings.Abortion)
                {
                    babyHorse = await momHorse.Birth().ConfigureAwait(false);
                }
                else
                {
                    await Abortion(document, momHorse).ConfigureAwait(false);
                }
                document = await momHorse.GetDoc().ConfigureAwait(false);
                heavyHorsing++;
            } while ((babyHorseSex != NextHorseSex && !Settings.Heavy) || (Settings.Heavy && heavyHorsing < 3 && babyHorseSex != NextHorseSex));
            if (Settings.Heavy && heavyHorsing >= 3 && babyHorseSex != NextHorseSex)
            {
                Cts.Cancel();
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(Application.Current.MainWindow, Properties.Resources.AccountHorsingStopMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, Properties.Resources.AccountHorsingStopMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
            }
            heavyHorsing = 0;
            if (babyHorseSex == HorseSex.Female)
            {
                HorsesCount++;
            }
            if (Settings.LimitType == Limit.Horses && babyHorseSex == HorseSex.Female && Settings.SchemeType == SchemeType.HalfPair)
            {
                CurrentValue++;
            }
            else if (Settings.LimitType == Limit.Horses && Settings.SchemeType == SchemeType.Pair)
            {
                CurrentValue++;
            }
            NextHorseSex = (babyHorseSex == HorseSex.Male) ? HorseSex.Female : HorseSex.Male;
            return babyHorse;
        }

        int heavyHorsing = 0;
        private async Task<Horse> BornAny(IHtmlDocument document, Horse momHorse)
        {
            if (document.QuerySelectorAll("#reproduction-tab-0 a").Last().Id != "boutonEchographie" && document.QuerySelectorAll("#reproduction-tab-0 a").Last().ClassName.Contains("disabled"))
            {
                document = await AgeToHorsing(document, momHorse).ConfigureAwait(false);
                if (Settings.TrainType == TrainType.Team)
                {
                    await momHorse.HorsingFemale().ConfigureAwait(false);
                }
                else
                {
                    await momHorse.HorsingFemaleWithoutTeam(document).ConfigureAwait(false);
                }
            }
            else if (document.QuerySelectorAll("#reproduction-tab-0 a").Last().Id != "boutonEchographie" && !document.QuerySelectorAll("#reproduction-tab-0 a").Last().ClassName.Contains("disabled"))
            {
                if (Settings.TrainType == TrainType.Team)
                {
                    await momHorse.HorsingFemale().ConfigureAwait(false);
                }
                else
                {
                    await momHorse.HorsingFemaleWithoutTeam(document).ConfigureAwait(false);
                }
            }
            await AgeToBirth(document, momHorse).ConfigureAwait(false);
            Horse babyHorse = await momHorse.Birth().ConfigureAwait(false);
            if (Settings.Heavy)
            {
                heavyHorsing++;
            }
            return babyHorse;
        }

        private async Task<IHtmlDocument> AgeToHorsing(IHtmlDocument document, Horse momHorse)
        {
            do
            {
                document = await TrainingCycle(document, momHorse, false).ConfigureAwait(false);
            } while (document.QuerySelectorAll("#reproduction-tab-0 a").Last().Id != "boutonEchographie" && document.QuerySelectorAll("#reproduction-tab-0 a").Last().ClassName.Contains("disabled"));
            return document;
        }

        private async Task<IHtmlDocument> AgeToEcho(IHtmlDocument document, Horse momHorse)
        {
            do
            {
                document = await TrainingCycle(document, momHorse, false).ConfigureAwait(false);
            } while (document.QuerySelector("#boutonEchographie").ClassName.Contains("action-disabled"));
            return document;
        }

        private async Task<IHtmlDocument> AgeToBirth(IHtmlDocument document, Horse momHorse)
        {
            do
            {
                document = await TrainingCycle(document, momHorse, true).ConfigureAwait(false);
            } while (document.QuerySelector("#lienVeterinaire") == null);
            return document;
        }

        private async Task<IHtmlDocument> Abortion(IHtmlDocument document, Horse momHorse)
        {
            momHorse.Status = Properties.Resources.HorseStatusAbortion;
            await momHorse.Action(document, "form-do-groom", "#boutonPanser", "doGroom").ConfigureAwait(false);
            if (Settings.MissionAfter2)
            {
                await momHorse.Mission().ConfigureAwait(false);
            }
            document = await momHorse.GetDoc().ConfigureAwait(false);
            await momHorse.Feeding(document).ConfigureAwait(false);
            await momHorse.Action(document, "age", "#boutonVieillir", "doAge").ConfigureAwait(false);
            document = await momHorse.GetDoc().ConfigureAwait(false);
            momHorse.Age += 2;
            return document;
        }
        #endregion
        #endregion
    }
}
