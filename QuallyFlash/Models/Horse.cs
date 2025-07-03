using AngleSharp.Html.Dom;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace QuallyFlash
{
    public class Horse : BaseModel, ICloneable
    {
        #region Public Members
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string Id { get; private set; }
        public string Name { get; private set; }
        public double Health
        {
            get => health;
            private set
            {
                if (value >= 0 && value <= 100)
                {
                    if (acc.IsWorking && value < acc.Settings.HealthEdge)
                    {
                        acc.Cts.Cancel();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, "Работа была остановлена! Здоровье лошади опустилось ниже указанной отметки", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                    health = value;
                }
                else
                {
                    throw new Exception("Ошибка в здоровье" + value);
                }
            }
        }
        public double Energy
        {
            get => energy;
            private set
            {
                if (value >= 0 && value <= 100)
                {
                    energy = value;
                }
                else
                {
                    throw new Exception("Ошибка в энергии" + value);
                }
            }
        }
        public HorseSex Sex { get; private set; }
        public int Age
        {
            get => age;
            set
            {
                age = value;
                if (value >= 348)
                {
                    acc.Cts.Cancel();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Работа была остановлена! Лошадь выросла до 29 лет и не родила нужного жеребенка!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
            }
        }
        public string Breed { get; set; }
        public double SkillVitality { get; set; }
        public double SkillSpeed { get; set; }
        public double SkillDressage { get; set; }
        public double SkillGalop { get; set; }
        public double SkillLynx { get; set; }
        public double SkillJump { get; set; }
        public double SkillTotal { get; set; }
        public double NLNP { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsSpecialized { get; set; } = false;
        public bool IsEquiped { get; set; } = false;
        public ObservableCollection<Training> Scheme { get; set; } = new ObservableCollection<Training>();
        public Training CurrentTraining { get; set; }
        public string Status { get; set; }
        #endregion

        #region Private Members
        private readonly Account acc;
        private double health;
        private double energy;
        private int age;
        private double energyForest = 8.1;
        private double energyMountain = 8.1;
        private double energyVitality = 7.2;
        private double energySpeed = 7.2;
        private double energyDressage = 4.5;
        private double energyGalop = 6.3;
        private double energyLynx = 6.3;
        private double energyJump = 6.3;
        private bool isWritedOut = false;
        #endregion

        public object Clone()
        {
            var horse = new Horse(Id, Name, acc)
            {
                Health = Health,
                Energy = Energy,
                Sex = Sex,
                Age = Age,
                Scheme = new ObservableCollection<Training>(),
                SkillVitality = SkillVitality,
                SkillSpeed = SkillSpeed,
                SkillDressage = SkillDressage,
                SkillGalop = SkillGalop,
                SkillLynx = SkillLynx,
                SkillJump = SkillJump,
                SkillTotal = SkillTotal,
                NLNP = NLNP,
                Time = Time,
                IsSpecialized = IsSpecialized,
                IsEquiped = IsEquiped,
                energyForest = energyForest,
                energyMountain = energyMountain,
                energyVitality = energyVitality,
                energySpeed = energySpeed,
                energyDressage = energyDressage,
                energyGalop = energyGalop,
                energyLynx = energyLynx,
                energyJump = energyJump,
            };
            foreach (var training in Scheme)
            {
                horse.Scheme.Add(training.Clone() as Training);
            }
            return horse;
        }

        public async Task<IHtmlDocument> GetDoc()
        {
            return Parser.ParseDocument(await acc.Client.GetAsync($"/elevage/chevaux/cheval?id={Id}").ConfigureAwait(false));
        }

        public Horse(string id, string name, Account acc)
        {
            Id = id;
            this.acc = acc;
            Name = name;
        }

        public Horse()
        {

        }

        #region Simple Action
        public bool LoadInfo(IHtmlDocument document)
        {
            if (document.GetElementsByClassName("h2").Length != 0)
            {
                return false;
            }
            else
            {
                var scripts = document.QuerySelectorAll(".content__middle script");
                string script = "";
                foreach (var item in scripts)
                {
                    if (item.InnerHtml.Contains("var chevalId"))
                    {
                        script = item.InnerHtml;
                        break;
                    }
                }
                Age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
                string sex = Regex.Match(script, "var chevalSexe = (.*?);").Groups[1].Value.Substring(1);
                Sex = (sex.Substring(0, sex.Length - 1) == "feminin") ? HorseSex.Female : HorseSex.Male;
                Energy = Convert.ToByte(document.QuerySelector("#energie").TextContent);
                Health = Convert.ToByte(document.QuerySelector("#sante").TextContent);
                string time = document.QuerySelectorAll(".hour")[0].TextContent;
                Time = new TimeSpan(Convert.ToInt32(time.Split(':')[0]), Convert.ToInt32(time.Split(':')[1]), 0);
                Breed = document.QuerySelector("#characteristics-body-content").QuerySelectorAll(".color-style-0")[0].TextContent;
                string test = document.QuerySelector("#enduranceValeur").TextContent;
                SkillVitality = Convert.ToDouble(document.QuerySelector("#enduranceValeur").TextContent);
                SkillSpeed = Convert.ToDouble(document.QuerySelector("#vitesseValeur").TextContent);
                SkillDressage = Convert.ToDouble(document.QuerySelector("#dressageValeur").TextContent);
                SkillGalop = Convert.ToDouble(document.QuerySelector("#galopValeur").TextContent);
                SkillLynx = Convert.ToDouble(document.QuerySelector("#trotValeur").TextContent);
                SkillJump = Convert.ToDouble(document.QuerySelector("#sautValeur").TextContent);
                SkillTotal = Convert.ToDouble(document.QuerySelector("#competencesValeur").TextContent);
                NLNP = Convert.ToDouble(document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[3].TextContent);
                if (Age >= 36)
                {
                    if (document.QuerySelector("#specialisationClassique") == null)
                    {
                        IsSpecialized = true;
                    }
                    if (document.QuerySelectorAll(".spacer-small-top.action.action-style-2").Length == 0)
                    {
                        IsEquiped = true;
                    }
                }
                var a = document.QuerySelector("#competition-body-content").QuerySelectorAll("a");
                int lastindex = (a.Length == 6) ? 5 : 4;
                foreach (var training in Scheme)
                {
                    training.LoadTraining(script, a, lastindex, this);
                }
                return true;
            }
        }

        public async Task Rename(IHtmlDocument document)
        {
            try
            {
                Status = "переименование";
                string affixValue = GetRenameValue("horseNameAffixe", document, acc.Settings.Affix);
                string farmValue = GetRenameValue("horseNameElevage", document, acc.Settings.Farm);
                string nameSkill;
                switch (acc.Settings.NameSkill)
                {
                    case "vitality": nameSkill = "/" + document.QuerySelector("#enduranceGenetique").TextContent.Split('.')[1]; break;
                    case "speed": nameSkill = "/" + document.QuerySelector("#vitesseGenetique").TextContent.Split('.')[1]; break;
                    case "dressage": nameSkill = "/" + document.QuerySelector("#dressageGenetique").TextContent.Split('.')[1]; break;
                    case "galop": nameSkill = "/" + document.QuerySelector("#galopGenetique").TextContent.Split('.')[1]; break;
                    case "lynx": nameSkill = "/" + document.QuerySelector("#trotGenetique").TextContent.Split('.')[1]; break;
                    case "jump": nameSkill = "/" + document.QuerySelector("#sautGenetique").TextContent.Split('.')[1]; break;
                    default: nameSkill = string.Empty; break;
                }
                var globalSettings = GlobalSettings.GetInstance();
                if (Sex == HorseSex.Female)
                {
                    if (acc.Settings.RandomNames && globalSettings.FemaleNames.Count == 0)
                    {
                        Name = $"{Properties.Resources.FemaleNames.Split(',')[new Random().Next(0, Properties.Resources.FemaleNames.Split(',').Length)]} {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                    else if (acc.Settings.RandomNames && !(globalSettings.FemaleNames.Count == 0))
                    {
                        Name = $"{globalSettings.FemaleNames[new Random().Next(0, globalSettings.FemaleNames.Count - 1)]} {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                    else if (!acc.Settings.RandomNames && string.IsNullOrEmpty(acc.Settings.FemaleName))
                    {
                        Name = $"Жен {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                    else
                    {
                        Name = $"{acc.Settings.FemaleName} {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                }
                else if (Sex == HorseSex.Male)
                {
                    if (acc.Settings.RandomNames && globalSettings.MaleNames.Count == 0)
                    {
                        Name = $"{Properties.Resources.MaleNames.Split(',')[new Random().Next(0, Properties.Resources.MaleNames.Split(',').Length)]} {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                    else if (acc.Settings.RandomNames && !(globalSettings.MaleNames.Count == 0))
                    {
                        Name = $"{globalSettings.MaleNames[new Random().Next(0, globalSettings.MaleNames.Count - 1)]} {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                    else if (!acc.Settings.RandomNames && string.IsNullOrEmpty(acc.Settings.MaleName))
                    {
                        Name = $"Муж {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                    else
                    {
                        Name = $"{acc.Settings.MaleName} {(!acc.Settings.DisableGP ? "" : document.QuerySelector("#genetic-body-content").QuerySelectorAll("strong")[1].TextContent.Split(':')[1].Trim())}{nameSkill}";
                    }
                }
                await acc.Client.PostAsync("/elevage/chevaux/doUpdateProfil", $"id={Id}&name={Name}&affixe={affixValue}&elevage={farmValue}");
            }
            catch (Exception e)
            {
                logger.Error($"Ошибка переименования: {e.Message}\n {e.StackTrace}");
            }
        }

        private string GetRenameValue(string selector, IHtmlDocument document, string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    foreach (var option in document.QuerySelectorAll($"#{selector} option"))
                    {
                        if (!option.HasAttribute("disabled"))
                        {
                            string str1 = option.TextContent.Trim().Replace(" ", "");
                            string str2 = value.Trim().Replace(" ", "");
                            if (string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase))
                            {
                                return option.GetAttribute("value").Trim();
                            }
                        }
                    }
                }
                else
                {
                    return document.QuerySelectorAll($"#{selector} option").Where(e => e.HasAttribute("selected")).ToList()[0].GetAttribute("value");
                }
            }
            catch
            {

            }
            return string.Empty;
        }

        public async Task<string> Action(IHtmlDocument document, string formId, string selector, string action)
        {
            string answer = "";
            if (!document.QuerySelector(selector).ClassName.Contains("action-disabled"))
            {
                switch (action)
                {
                    case "doGroom": Status = Properties.Resources.HorseStatusGroom; break;
                    case "doNight": Status = Properties.Resources.HorseStatusSleep; break;
                    case "doAge": Status = Properties.Resources.HorseStatusAge; break;
                    case "doStroke": Status = Properties.Resources.HorseStatusStroke; break;
                    case "doDrink": Status = Properties.Resources.HorseStatusDrink; break;
                    case "doEatTreat":
                        if (formId.Contains("carotte"))
                        {
                            Status = Properties.Resources.HorseStatusCarrot;
                        }
                        else
                        {
                            Status = Properties.Resources.HorseStatusMash;
                        }
                        break;
                }
                string postData = ParseForm(document, formId);
                answer = await acc.Client.PostAsync($"/elevage/chevaux/{action}", postData).ConfigureAwait(false);
                if (action != "doNight" && action != "doAge")
                {
                    ParseStats(answer);
                    ParseEnergies(answer);
                }
            }
            return answer;
        }

        private string ParseForm(IHtmlDocument document, string formId)
        {
            var form = document.GetElementById(formId);
            string value = form.Children[1].GetAttribute("value").ToLower();
            string name1 = form.Children[1].GetAttribute("name").ToLower();
            string postData;
            if (formId.Contains("eat-treat"))
            {
                if (formId.Contains("carotte"))
                {
                    postData = $"{name1}={value}&id={Id}&friandise=carotte";
                }
                else
                {
                    postData = $"{name1}={value}&id={Id}&friandise=mash";
                }
                return postData;
            }
            else
            {
                postData = $"{name1}={value}";
            }
            string name2 = form.Children[2].GetAttribute("name").Substring(formId.Length).ToLower();
            postData += $"&{name2}={Id}";
            if (formId != "form-do-suckle")
            {
                if (formId == "age")
                {
                    string name3 = form.Children[3].GetAttribute("name").Substring(formId.Length).ToLower();
                    string name4 = form.Children[4].GetAttribute("name").Substring(formId.Length).ToLower();
                    string name5 = form.Children[5].GetAttribute("name").Substring(formId.Length).ToLower();
                    postData += $"&{name3}={Age}&{name4}={new Random().Next(20, 60)}&{name5}={new Random().Next(20, 60)}";
                }
                else
                {
                    string name3 = form.Children[3].GetAttribute("name").Substring(formId.Length).ToLower();
                    string name4 = form.Children[4].GetAttribute("name").Substring(formId.Length).ToLower();
                    postData += $"&{name3}={new Random().Next(20, 60)}&{name4}={new Random().Next(20, 60)}";
                }
            }
            return postData;
        }

        public async Task Mission()
        {
            Status = Properties.Resources.HorseStatusMission;
            string json = await acc.Client.PostAsync("/elevage/chevaux/doCentreMission", $"id={Id}").ConfigureAwait(false);
            ParseStats(json);
        }

        public async Task Feeding(IHtmlDocument document)
        {
            string postData;
            Status = Properties.Resources.HorseStatusFeed;
            if (Age < 6)
            {
                await Action(document, "form-do-suckle", "#boutonAllaiter", "doSuckle").ConfigureAwait(false);
            }
            else if (Age < 18)
            {
                ParseFood(document, "hay", out string hayName, out int hay);
                postData = ParseForm(document, "feeding") + $"&{hayName}={hay.ToString()}";
                if (hay != 0)
                {
                    if (acc.Hay.Amount < 100 && acc.Settings.BuyFood)
                    {
                        if (acc.Equ < 2500)
                        {
                            if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > 2100)
                            {
                                await acc.Sell(acc.MainProductToSell, (2100 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            }
                            else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > 2100)
                            {
                                await acc.Sell(acc.SubProductToSell, (2100 / acc.SubProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }));
                            }
                        }
                        await acc.Buy(acc.Hay, "1000");
                    }
                    else if (acc.Hay.Amount < 100 && !acc.Settings.BuyFood)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage3}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                    string json = await acc.Client.PostAsync("/elevage/chevaux/doEat", postData);
                    ParseStats(json);
                    ParseEnergies(json);
                }
            }
            else
            {
                ParseFood(document, "hay", out string hayName, out int hay);
                ParseFood(document, "oats", out string oatName, out int oat);
                postData = ParseForm(document, "feeding") + $"&{hayName}={hay.ToString()}&{oatName}={(oat + 3).ToString()}";
                if (hay != 0 || oat != 0)
                {
                    if (acc.Hay.Amount < 100 && acc.Settings.BuyFood)
                    {
                        if (acc.Equ < 2500)
                        {
                            if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > 2100)
                            {
                                await acc.Sell(acc.MainProductToSell, (2100 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            }
                            else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > 2100)
                            {
                                await acc.Sell(acc.SubProductToSell, (2100 / acc.SubProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }));
                            }
                        }
                        await acc.Buy(acc.Hay, "1000");
                    }
                    else if (acc.Hay.Amount < 100 && !acc.Settings.BuyFood)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage3}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                    if (acc.Oat.Amount < 100 && acc.Settings.BuyFood)
                    {
                        if (acc.Equ < 2500)
                        {
                            if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > 2100)
                            {
                                await acc.Sell(acc.MainProductToSell, (2100 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            }
                            else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > 2100)
                            {
                                await acc.Sell(acc.SubProductToSell, (2100 / acc.SubProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }));
                            }
                        }
                        await acc.Buy(acc.Oat, "1000");
                    }
                    else if (acc.Oat.Amount < 100 && !acc.Settings.BuyFood)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage3}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                    string answer = await acc.Client.PostAsync("/elevage/chevaux/doEat", postData).ConfigureAwait(false);
                    ParseStats(answer);
                    ParseEnergies(answer);
                }
            }
        }

        private void ParseFood(IHtmlDocument document, string foodType, out string foodName, out int foodFinal)
        {
            string[] foodString;
            if (foodType == "hay")
            {
                foodString = document.QuerySelectorAll(".float-right.section-fourrage.section-fourrage-quantity").First().TextContent.Trim().Split('/');
            }
            else
            {
                foodString = document.QuerySelectorAll(".float-right.section-avoine.section-avoine-quantity").First().TextContent.Trim().Split('/');
            }
            foodName = document.GetElementById(foodType + "Slider-sliderHidden").GetAttribute("name").Substring(7).ToLower();
            string foodToGive = foodString[1];
            var feedForm = document.QuerySelector("#feeding");
            if (foodType == "hay" && feedForm.GetElementsByTagName("div")[0].Id == "messageBoxInline")
            {
                if (feedForm.GetElementsByTagName("div")[0].InnerHtml.Contains("20"))
                {
                    foodToGive = "20";
                }
                else if (!feedForm.GetElementsByTagName("div")[0].InnerHtml.Contains("20"))
                {
                    foodToGive = "0";
                }
            }
            int foodGiven = Convert.ToInt32(foodString[0]);
            int foodGive = Convert.ToInt32(foodToGive);
            foodFinal = foodGive - foodGiven;
            if (foodFinal < 0)
            {
                foodFinal = 0;
            }
        }
        #endregion

        #region Centre
        public async Task Centre()
        {
            string answer;
            do
            {
                Status = Properties.Resources.HorseStatusCenter;
                await MoneyCheck();
                string selles = acc.Settings.Saddle ? "1" : "2";
                string brides = acc.Settings.Ramp ? "1" : "2";
                string tapis = acc.Settings.Bridle ? "1" : "2";
                string carrot = acc.Settings.Carrot ? "1" : "2";
                string mash = acc.Settings.Mash ? "1" : "2";
                string forest = acc.Settings.Forest ? "1" : "2";
                string mountain = acc.Settings.Mountain ? "1" : "2";
                string shower = acc.Settings.Shower ? "1" : "2";
                string bowl = acc.Settings.Bowl ? "1" : "2";
                int selectTarif = 2;
                if (selectTarif < 0)
                {
                    selectTarif = 2;
                }
                string postData = $"cheval={Id}&elevage=&cheval={Id}&competence={SkillTotal}&tri=tarif{selectTarif}&sens=ASC&tarif=&leconsPrix=&foret={forest}&montagne={mountain}&plage=2&classique=2&western=2&fourrage=2&avoine=2&carotte={carrot}&mash={mash}&hasSelles={selles}&hasBrides={brides}&hasTapis={tapis}&hasBonnets=2&hasBandes=2&abreuvoir={bowl}&douche={shower}&centre=&centreBox=0&directeur=&prestige=&bonus=&boxType=&boxLitiere=&prePrestige=&prodSelles=&prodBrides=&prodTapis=&prodBonnets=&prodBandes=";
                string json;
                do
                {
                    json = await acc.Client.PostAsync("/elevage/chevaux/centreSelection", postData).ConfigureAwait(false);
                } while (!json.Contains("{\"content\":"));
                var doc = Parser.ParseDocument(json, "content");
                var scripts = doc.QuerySelectorAll("#table-0 tbody tr")[0].QuerySelectorAll("script");
                for (int i = 0; i < scripts.Length; i++)
                {
                    if (scripts[i].InnerHtml.Contains("duree=" + acc.Settings.Duration + "&elevage="))
                    {
                        postData = Regex.Match(scripts[i].InnerHtml, "{'params': '(.*?)'}").Groups[1].Value;
                        break;
                    }
                }
                answer = await acc.Client.PostAsync("/elevage/chevaux/doCentreInscription", postData).ConfigureAwait(false);
            } while (!answer.Contains("message=centreOk"));
        }

        public async Task CentreReserve()
        {
            string answer;
            do
            {
                Status = Properties.Resources.HorseStatusReserveCenter;
                await MoneyCheck();
                string id = acc.Settings.ReserveID;
                if (acc.Settings.SelfReserve)
                {
                    var document = Parser.ParseDocument(await acc.Client.GetAsync("/centre/bureau/").ConfigureAwait(false));
                    id = document.QuerySelectorAll("#page-contents .action.action-style-2")[0].GetAttribute("href").Split('&')[0].Split('=')[1];
                }
                string postData = await acc.Client.PostAsync($"/elevage/chevaux/boxReserveSelection", $"cheval={Id}");
                JObject jobj = JObject.Parse(postData);
                if ((bool)jobj["hasBox"])
                {
                    var document1 = Parser.ParseDocument(await acc.Client.PostAsync($"/elevage/chevaux/boxReserveSelection", $"cheval={Id}").ConfigureAwait(false), "content");
                    if (document1.QuerySelectorAll(".pageNumbering").Length > 0)
                    {
                        int last = Convert.ToInt32(document1.QuerySelectorAll(".pageNumbering a").Last().GetAttribute("data-page")) + 1;
                        for (int i = 0; i < last; i++)
                        {
                            document1 = Parser.ParseDocument(await acc.Client.PostAsync($"/elevage/chevaux/boxReserveSelection", $"cheval={Id}&page={i}").ConfigureAwait(false), "content");
                            postData = ScriptSearch(document1, id);
                            if (postData != default)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        postData = ScriptSearch(document1, id);
                    }
                    if (postData != string.Empty)
                    {
                        answer = await acc.Client.PostAsync($"/elevage/chevaux/doCentreInscription", postData).ConfigureAwait(false);
                    }
                    else
                    {
                        answer = "0";
                    }
                }
                else
                {
                    answer = "0";
                }
            } while (!answer.Contains("message=centreOk") && answer != "0");
            if (answer == "0")
            {
                await Centre().ConfigureAwait(false);
            }
        }

        private string ScriptSearch(IHtmlDocument document, string id)
        {
            string postData = "";
            var scripts = document.QuerySelectorAll("#table-0 tbody")[0].QuerySelectorAll("script");
            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i].InnerHtml.Contains($"id={Id}&centre={id}") && scripts[i].InnerHtml.Contains($"duree={acc.Settings.Duration}"))
                {
                    postData = Regex.Match(scripts[i].InnerHtml, "{'params': '(.*?)'}").Groups[1].Value;
                    break;
                }
            }
            return postData;
        }

        private async Task MoneyCheck()
        {
            int min = 0;
            switch (acc.Settings.Duration)
            {
                case "1":
                    min = 200;
                    break;
                case "3":
                    min = 600;
                    break;
                case "10":
                    min = 2000;
                    break;
                case "30":
                    min = 6000;
                    break;
                case "60":
                    min = 12000;
                    break;
            }
            if (acc.Equ < min + 100)
            {
                if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > min + 100)
                {
                    await acc.Sell(acc.MainProductToSell, (min + 1000 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                }
                else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > min + 100)
                {
                    await acc.Sell(acc.SubProductToSell, (min + 1000 / acc.SubProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
                await acc.LoadProducts().ConfigureAwait(false);
            }
        }

        private async Task CentreCancel()
        {
            Status = Properties.Resources.HorseStatusCenterCancel;
            await acc.Client.PostAsync("/elevage/chevaux/doCenterCancel", $"id={Id}").ConfigureAwait(false); ;
        }
        #endregion

        #region Trainings
        public async Task Game(IHtmlDocument document)
        {
            double playEnergy = 5.4;
            switch (Age)
            {
                case 8: playEnergy = 5.4; break;
                case 10: playEnergy = 4.95; break;
                case 12: playEnergy = 4.5; break;
                case 14: playEnergy = 4.05; break;
                case 16: playEnergy = 3.6; break;
            }
            var form = document.QuerySelector("#formCenterPlay");
            string name1 = form.Children[1].GetAttribute("name").ToLower();
            string value = form.Children[1].GetAttribute("value").ToLower();
            string name2 = form.Children[2].GetAttribute("name").Substring(14).ToLower();
            string name3 = document.QuerySelector("#centerPlaySlider-sliderHidden").GetAttribute("name").Substring(14).ToLower();
            int energyPoint = Convert.ToInt32(Math.Floor((Energy - 20) / playEnergy));
            int currentTime = (int)Time.TotalMinutes;
            int edgeTime = 1320;
            int timePoint = (edgeTime - currentTime) / 30;
            int count = energyPoint > timePoint ? timePoint : energyPoint;
            if (count > 0)
            {
                Status = Properties.Resources.HorseStatusGame;
                string postData = $"{name1}={value}&{name2}={Id}&playEndurance={SkillVitality}&playVitesse={SkillSpeed}&playDressage={SkillDressage}&playGalop={SkillGalop}&playTrot={SkillLynx}&playSaut={SkillJump}&{name3}={count}&playAge={Age}&playForm=formCenterPlay&playDoucheGain=10&playMaxCompetence=10&playCompetenceGain=0.2";
                string json = await acc.Client.PostAsync("/elevage/chevaux/doPlay", postData).ConfigureAwait(false);
                ParseStats(json);
                ParseSkills(json);
                ParseEnergies(json);
            }
        }

        private void ParseStats(string json)
        {
            double energy = Convert.ToDouble(Parser.ParseAnswer(json, "chevalEnergie"));
            Energy = energy == 0 ? Energy : energy;
            double health = Convert.ToDouble(Parser.ParseAnswer(json, "chevalSante"));
            Health = health == 0 ? Health : health;
            Time = TimeSpan.FromSeconds(Convert.ToDouble(Parser.ParseAnswer(json, "chevalTemps")));
        }

        private void ParseSkills(string json)
        {
            SkillVitality = Convert.ToDouble(Parser.ParseAnswer(json, "enduranceValeur"));
            SkillSpeed = Convert.ToDouble(Parser.ParseAnswer(json, "vitesseValeur"));
            SkillDressage = Convert.ToDouble(Parser.ParseAnswer(json, "dressageValeur"));
            SkillGalop = Convert.ToDouble(Parser.ParseAnswer(json, "galopValeur"));
            SkillLynx = Convert.ToDouble(Parser.ParseAnswer(json, "trotValeur"));
            SkillJump = Convert.ToDouble(Parser.ParseAnswer(json, "sautValeur"));
            SkillTotal = SkillVitality + SkillSpeed + SkillDressage + SkillGalop + SkillLynx + SkillJump;
        }

        private void ParseTrains(string json)
        {
            double b3 = Convert.ToDouble(Parser.ParseAnswer(json, "varsB3"));
            double b1 = Convert.ToDouble(Parser.ParseAnswer(json, "varsB1"));
            double e1 = Convert.ToDouble(Parser.ParseAnswer(json, "varsE1"));
            double e2 = Convert.ToDouble(Parser.ParseAnswer(json, "varsE2"));
            double e3 = Convert.ToDouble(Parser.ParseAnswer(json, "varsE3"));
            double e4 = Convert.ToDouble(Parser.ParseAnswer(json, "varsE4"));
            double e5 = Convert.ToDouble(Parser.ParseAnswer(json, "varsE5"));
            double e6 = Convert.ToDouble(Parser.ParseAnswer(json, "varsE6"));
            foreach (var training in Scheme)
            {
                training.ParseSkill(b3, b1, e1, e2, e3, e4, e5, e6);
            }
        }

        private void ParseEnergies(string json)
        {
            energyVitality = Convert.ToDouble(Parser.ParseAnswer(json, "coefficientEnergieEndurance")) / 2;
            if (energyVitality == 0)
                energyVitality = 7.2;
            energySpeed = Convert.ToDouble(Parser.ParseAnswer(json, "coefficientEnergievitesse")) / 2;
            if (energySpeed == 0)
                energySpeed = 7.2;
            energyDressage = Convert.ToDouble(Parser.ParseAnswer(json, "coefficientEnergieDressage")) / 2;
            if (energyDressage == 0)
                energyDressage = 4.5;
            energyGalop = Convert.ToDouble(Parser.ParseAnswer(json, "coefficientEnergieGalop")) / 2;
            if (energyGalop == 0)
                energyGalop = 6.3;
            energyLynx = Convert.ToDouble(Parser.ParseAnswer(json, "coefficientEnergieTrot")) / 2;
            if (energyLynx == 0)
                energyLynx = 6.3;
            energyJump = Convert.ToDouble(Parser.ParseAnswer(json, "coefficientEnergieSaut")) / 2;
            if (energyJump == 0)
                energyJump = 6.3;
        }

        public async Task Walk(IHtmlDocument document, Training training)
        {
            if (Age >= 18)
            {
                string json = "0";
                do
                {
                    double walkEnergy = training.Value == "forest" ? energyForest : energyMountain;
                    string selector = training.Value == "forest" ? "Foret" : "Montagne";
                    var form = document.QuerySelector($"#formbalade{selector}");
                    string name = form.Children[1].GetAttribute("name").ToLower();
                    string value = form.Children[1].GetAttribute("value").ToLower();
                    string name2 = form.Children[2].GetAttribute("name").Substring($"formbalade{selector}".Length).ToLower();
                    string name3 = form.Children[3].GetAttribute("name").Substring($"formbalade{selector}".Length).ToLower();
                    string name4 = document.QuerySelector($"#walk{selector.ToLower()}Slider-sliderHidden").GetAttribute("name").Substring(($"formbalade{selector}").Length).ToLower();
                    int energyPoint = Convert.ToInt32(Math.Floor((Energy - 20) / walkEnergy));
                    int currentTime = (int)Time.TotalMinutes;
                    int edgeTime = 1320;
                    int timePoint = (edgeTime - currentTime) / 30;
                    int count = energyPoint > timePoint ? timePoint : energyPoint;
                    if (count > 0)
                    {
                        Status = Properties.Resources.HorseStatusWalk + " " + training.Name;
                        string postData = $"{name}={value}&{name2}={Id}&{name3}={selector.ToLower()}&{name4}={count}";
                        json = await acc.Client.PostAsync("/elevage/chevaux/doWalk", postData).ConfigureAwait(false);
                        try
                        {
                            ParseStats(json);
                            ParseSkills(json);
                            ParseTrains(json);
                            ParseEnergies(json);
                        }
                        catch
                        {

                        }
                    }
                } while (string.IsNullOrEmpty(json));
                if (training.IsDone && acc.Settings.WriteOut && !isWritedOut && document.QuerySelectorAll("#center-tab-main .button-text-3")[0] != null)
                {
                    await CentreCancel().ConfigureAwait(false);
                    await Centre().ConfigureAwait(false);
                    isWritedOut = true;
                }
            }
        }

        public async Task Train(IHtmlDocument document, Training training)
        {
            if (Age >= 24)
            {
                double energy = default;
                string selector = default;
                switch (training.Value)
                {
                    case "vitality": energy = energyVitality; selector = "Endurance"; break;
                    case "speed": energy = energySpeed; selector = "Vitesse"; break;
                    case "dressage": energy = energyDressage; selector = "Dressage"; break;
                    case "galop": energy = energyGalop; selector = "Galop"; break;
                    case "lynx": energy = energyLynx; selector = "Trot"; break;
                    case "jump": energy = energyJump; selector = "Saut"; break;
                }
                int energyPoint = Convert.ToInt32(Math.Floor((Energy - 20) / energy));
                int currentTime = (int)Time.TotalMinutes;
                int edgeTime = 1320;
                int timePoint = (edgeTime - currentTime) / 30;
                int count = energyPoint > timePoint ? timePoint : energyPoint;
                if (count > 0)
                {
                    Status = Properties.Resources.HorseStatusTraining + " " + training.Name;
                    var form = document.QuerySelector($"#entrainement{selector}");
                    string name = form.Children[1].GetAttribute("name").ToLower();
                    string value = form.Children[1].GetAttribute("value").ToLower();
                    string postData = $"{name}={value}&id={Id}&competence={selector.ToLower()}&duration={count}";
                    string json = await acc.Client.PostAsync("/elevage/chevaux/doTraining", postData).ConfigureAwait(false);
                    ParseStats(json);
                    ParseSkills(json);
                    ParseTrains(json);
                    ParseEnergies(json);
                }
            }
        }

        public async Task Competition(Training training)
        {
            if (Age >= 36)
            {
                int edgeTime = 1320;
                double energy = 100;
                string competitionType = default;
                switch (training.Value)
                {
                    case "lynxcompet": competitionType = "trot"; break;
                    case "galopcompet": competitionType = "galop"; break;
                    case "dressagecompet": competitionType = "dressage"; break;
                    case "cross": competitionType = "cross"; break;
                    case "concur": competitionType = "cso"; break;
                    case "barell": competitionType = "barrel"; break;
                    case "cutting": competitionType = "cutting"; break;
                    case "trail": competitionType = "trailClass"; break;
                    case "raining": competitionType = "reining"; break;
                    case "plege": competitionType = "westernPleasure"; break;
                }
                string postData;
                if (competitionType == "trot" || competitionType == "galop")
                {
                    postData = $"type={competitionType}&id={Id}&course={competitionType}&sort=inscrits-d";
                }
                else if (competitionType == "cso")
                {
                    postData = $"type={competitionType}&id={Id}&longueur=2&sort=inscrits-d";
                }
                else if (competitionType == "cross")
                {
                    postData = $"type={competitionType}&id={Id}&longueur=5&sort=inscrits-d";
                }
                else
                {
                    postData = $"type={competitionType}&id={Id}&sort=inscrits-d";
                }
                do
                {
                    string answer = string.Empty;
                    do
                    {
                        string json = await acc.Client.PostAsync("/elevage/competition/liste", postData).ConfigureAwait(false);
                        var doc = Parser.ParseDocument(json, "content");
                        if (doc.QuerySelector("#messageBoxInline") == null && (int)Time.TotalMinutes + 120 < edgeTime)
                        {
                            Status = Properties.Resources.HorseStatusCompetition + " " + training.Name;
                            var energies = doc.QuerySelectorAll("td[class=\"width-20 align-center\"]");
                            energy = 100;
                            int count = -1;
                            int i = 0;
                            foreach (var strong in energies)
                            {
                                if (energy > Convert.ToDouble(strong.TextContent.Substring(0, strong.TextContent.Length - 4)))
                                {
                                    energy = Convert.ToDouble(strong.TextContent.Substring(0, strong.TextContent.Length - 4));
                                    count = i;
                                }
                                i++;
                            }
                            if (count > -1)
                            {
                                var scripts = doc.QuerySelectorAll("td[class=\"width-10 align-center\"] > script");
                                string postDataCompet = Regex.Match(scripts[count].InnerHtml, "'/(.*?)',").Groups[1].Value;
                                answer = await acc.Client.GetAsync("/" + postDataCompet).ConfigureAwait(false);
                                LoadInfo(await GetDoc().ConfigureAwait(false));
                            }
                        }
                    } while (!answer.Contains("redirection") && (int)Time.TotalMinutes + 120 < edgeTime);
                } while (Energy - energy > 20 && (int)Time.TotalMinutes + 120 < edgeTime && !training.IsDone);
            }
        }

        public async Task Training(IHtmlDocument document)
        {
            foreach (var training in Scheme)
            {
                if (!training.IsDone)
                {
                    CurrentTraining = training;
                    switch (training.Value)
                    {
                        //Прогулки
                        case "forest": await Walk(document, training).ConfigureAwait(false); return;
                        case "mountain": await Walk(document, training).ConfigureAwait(false); return;
                        //Тренировки
                        case "vitality": await Train(document, training).ConfigureAwait(false); return;
                        case "speed": await Train(document, training).ConfigureAwait(false); return;
                        case "dressage": await Train(document, training).ConfigureAwait(false); return;
                        case "galop": await Train(document, training).ConfigureAwait(false); return;
                        case "lynx": await Train(document, training).ConfigureAwait(false); return;
                        case "jump": await Train(document, training).ConfigureAwait(false); return;
                        //Классика
                        case "lynxcompet": await Competition(training).ConfigureAwait(false); return;
                        case "galopcompet": await Competition(training).ConfigureAwait(false); return;
                        case "dressagecompet": await Competition(training).ConfigureAwait(false); return;
                        case "cross": await Competition(training).ConfigureAwait(false); return;
                        case "concur": await Competition(training).ConfigureAwait(false); return;
                        //Вестерн
                        case "barell": await Competition(training).ConfigureAwait(false); return;
                        case "cutting": await Competition(training).ConfigureAwait(false); return;
                        case "trail": await Competition(training).ConfigureAwait(false); return;
                        case "raining": await Competition(training).ConfigureAwait(false); return;
                        case "plege": await Competition(training).ConfigureAwait(false); return;
                    }
                }
            }
        }

        public async Task Specialization()
        {
            Status = Properties.Resources.HorseStatusSpecialization;
            string specialization = (acc.Settings.Specialization == QuallyFlash.Specialization.Classic) ? "classique" : "western";
            string postData = $"id={Id}&specialisation={specialization}";
            await acc.Client.PostAsync("/elevage/chevaux/doSpecialise", postData).ConfigureAwait(false);
            IsSpecialized = true;
        }

        public async Task Equipment()
        {
            Status = Properties.Resources.HorseStatusEquipment;
            string specialization = (acc.Settings.Specialization == QuallyFlash.Specialization.Classic) ? "classique" : "western";
            await CheckEquipment(acc.RampClassic).ConfigureAwait(false);
            await CheckEquipment(acc.RampWestern).ConfigureAwait(false);
            await CheckEquipment(acc.Forehead).ConfigureAwait(false);
            await CheckEquipment(acc.Bandages).ConfigureAwait(false);
            string amunitionLevel;
            switch (acc.Settings.Amunition)
            {
                case 1:
                    await CheckEquipment(acc.SaddleClassic1).ConfigureAwait(false);
                    await CheckEquipment(acc.BridleClassic1).ConfigureAwait(false);
                    await CheckEquipment(acc.SaddleWestern1).ConfigureAwait(false);
                    await CheckEquipment(acc.BridleWestern1).ConfigureAwait(false);
                    amunitionLevel = "1x";
                    break;
                case 2:
                    await CheckEquipment(acc.SaddleClassic2).ConfigureAwait(false);
                    await CheckEquipment(acc.BridleClassic2).ConfigureAwait(false);
                    await CheckEquipment(acc.SaddleWestern2).ConfigureAwait(false);
                    await CheckEquipment(acc.BridleWestern2).ConfigureAwait(false);
                    amunitionLevel = "2x";
                    break;
                case 3:
                    await CheckEquipment(acc.SaddleClassic3).ConfigureAwait(false);
                    await CheckEquipment(acc.BridleClassic3).ConfigureAwait(false);
                    await CheckEquipment(acc.SaddleWestern3).ConfigureAwait(false);
                    await CheckEquipment(acc.BridleWestern3).ConfigureAwait(false);
                    amunitionLevel = "3x";
                    break;
                default: amunitionLevel = "1x"; break;
            }
            string bandages = (acc.Settings.Bandages) ? "modele-bande-1x" : string.Empty;
            string forehead = (acc.Settings.Headrest) ? "modele-bonnet-1x" : string.Empty;
            string size = "3";
            if (bandages != string.Empty)
            {
                size = "4";
                if (forehead != string.Empty)
                {
                    size = "5";
                }
            }
            if (forehead != string.Empty)
            {
                size = "4";
                if (bandages != string.Empty)
                {
                    size = "5";
                }
            }
            string postData = $"id={Id}&force=0&items=modele-tapis-{specialization}-1xmodele-selle-{specialization}-{amunitionLevel}modele-bride-{specialization}-{amunitionLevel}{bandages}{forehead}&size={size}";
            await acc.Client.PostAsync("/elevage/chevaux/getChoisirEquipement", postData).ConfigureAwait(false);
            bandages = (acc.Settings.Bandages) ? "bande-1x" : "";
            forehead = (acc.Settings.Headrest) ? "bonnet-1x" : "";
            postData = $"id={Id}&tapis=tapis-{specialization}-1x&tapisCourant=&tapisCouleur=0&tapisCouleurCourant=0&selle=selle-{specialization}-{amunitionLevel}&selleCourant=&bride=bride-{specialization}-{amunitionLevel}&brideCourant=&bande={bandages}&bandeCourant=&bandeCouleur=0&bandeCouleurCourant=0&bonnet={forehead}&bonnetCourant=&bonnetCouleur=0&bonnetCouleurCourant=0";
            string json = await acc.Client.PostAsync("/elevage/chevaux/doHarness", postData).ConfigureAwait(false);
            //var jobj = JObject.Parse(json);
            IsEquiped = true;
        }

        private async Task CheckEquipment(Equipment equipment)
        {
            if (equipment.Amount <= 0)
            {
                if (acc.Equ <= equipment.Price * 2 + 1000)
                {
                    if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > equipment.Price * 2 + 1000)
                    {
                        await acc.Sell(acc.MainProductToSell, (equipment.Price * 2 + 1000 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                    }
                    else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > equipment.Price * 2 + 1000)
                    {
                        await acc.Sell(acc.SubProductToSell, (equipment.Price * 2 + 1000 / acc.SubProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                }
                if (acc.Settings.ParallelPair)
                {
                    await acc.Buy(equipment, "2").ConfigureAwait(false);
                } else
                {
                    await acc.Buy(equipment, "1").ConfigureAwait(false);
                }
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                acc.LoadEquipment().ConfigureAwait(false);
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            }
        }

        public async Task GiveWhip()
        {
            Status = Properties.Resources.HorseStatusWhip;
            string bonus;
            switch ((acc.Client as RequestClient).BaseAddress)
            {
                case "https://au.howrse.com": bonus = "57"; break;
                case "https://www.howrse.co.uk": bonus = "57"; break;
                case "https://ar.howrse.com": bonus = "57"; break;
                case "https://www.howrse.bg": bonus = "57"; break;
                case "https://www.howrse.com": bonus = "57"; break;
                case "https://www.caballow.com": bonus = "52"; break;
                case "https://ca.howrse.com": bonus = "57"; break;
                case "https://www.howrse.de": bonus = "57"; break;
                case "https://www.howrse.no": bonus = "57"; break;
                case "https://www.howrse.pl": bonus = "57"; break;
                case "https://www.lowadi.com": bonus = "135"; break;
                case "https://www.howrse.ro": bonus = "57"; break;
                case "https://us.howrse.com": bonus = "57"; break;
                case "https://ouranos.equideow.com": bonus = "57"; break;
                case "https://gaia.equideow.com": bonus = "57"; break;
                case "https://www.howrse.se": bonus = "57"; break;
                default: bonus = "135"; break;
            }
            await acc.Client.PostAsync("/elevage/chevaux/doDonnerBonus", $"id={Id}&bonus={bonus}").ConfigureAwait(false);
        }
        #endregion

        #region Male functions
        public async Task<int> HorsingMale(int horsingNum)
        {
            Status = Properties.Resources.HorseStatusMaleHorsing;
            string postData = $"id={Id}&action=save&type=equipe&price=0&owner=&nom=";
            while (Energy - 25 > 20)
            {
                string json = await acc.Client.PostAsync("/elevage/chevaux/reserverJument", postData).ConfigureAwait(false);
                ParseStats(json);
                horsingNum--;
            }
            return horsingNum;
        }

        public async Task HorsingMaleWithoutTeam(string femaleId)
        {
            Status = Properties.Resources.HorseStatusMaleHorsing;
            string postData = $"id={Id}&action=save&type=moi&price=0&owner=&nom=&mare={femaleId}";
            string json = await acc.Client.PostAsync("/elevage/chevaux/reserverJument", postData).ConfigureAwait(false);
            ParseStats(json);
        }

        public async Task<Horse> GetNextHorse(IHtmlDocument document)
        {
            Status = Properties.Resources.HorseStatusReturnToMom;
            string id = document.QuerySelector("#origins-body-content").QuerySelectorAll(".horsename")[1].GetAttribute("href").Split('=')[1];
            string name = document.QuerySelector("#origins-body-content").QuerySelectorAll(".horsename")[1].TextContent;
            var horse = new Horse(id, name, acc)
            {
                Scheme = new ObservableCollection<Training>()
            };
            foreach (var training in acc.Settings.Scheme)
            {
                var t = new Training(training.Name, training.Value);
                horse.Scheme.Add(t);
            }
            horse.LoadInfo(await horse.GetDoc().ConfigureAwait(false));
            return horse;
        }
        #endregion

        #region Female functions
        public async Task HorsingFemale()
        {
            string answer;
            do
            {
                Status = Properties.Resources.HorseStatusMaleHorsing;
                if (acc.Equ < 3000)
                {
                    if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > 3000)
                    {
                        await acc.Sell(acc.MainProductToSell, (3000 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                    }
                    else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > 3000)
                    {
                        await acc.Sell(acc.SubProductToSell, (3000 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, $"На аккаунте {acc.Login} закончилось экю и нет ресурсов для пополнения (или не выбраны для докупки)! Пополните запасы и нажмите ОК", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                }
                var doc = Parser.ParseDocument(await acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument={Id}").ConfigureAwait(false));
                var options = doc.GetElementById("race").GetElementsByTagName("option");
                int count = default, num = default;
                for (int i = 0; i < options.Length; i++)
                {
                    if (Breed == options[i].TextContent)
                    {
                        num = count;
                    }
                    count++;
                }
                string raceIndex = options[num].GetAttribute("value");
                string searchString = $"tri=cTotal&page=0&sens=DESC&rechercher=1&potentielTotal=0&prix=0&blup=-100&race={raceIndex}&purete=1&cE=0&cV=0&cD=0&cG=0&cT=0&cS=0&cheval=1&poney=1&pegase=1&=1&jument={Id}&type=equipe";
                do
                {
                    doc = Parser.ParseDocument(await acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?{searchString}").ConfigureAwait(false));
                    if (doc.QuerySelectorAll(".align-center.action a").Length > 0)
                    {
                        string action = doc.QuerySelectorAll(".align-center.action a").First().GetAttribute("href");
                        doc = Parser.ParseDocument(await acc.Client.GetAsync(action).ConfigureAwait(false));
                        string script = doc.QuerySelectorAll("#page-contents script").Last().InnerHtml;
                        string postData = Regex.Match(script, "params: \"(.*?)\"}").Groups[1].Value;
                        answer = await acc.Client.PostAsync("/elevage/chevaux/doReproduction", postData).ConfigureAwait(false);
                    }
                    else
                    {
                        answer = "1";
                        acc.Cts.Cancel();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, "Работа была остановлена! Не обнаружено случек в команде!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                } while (answer.Contains("\"errors\":[\"internal\"]"));
            } while (answer.Contains("saillieArgent") || !answer.Contains("?id=") && !(answer == "1"));
        }

        public async Task HorsingFemaleWithoutTeam(IHtmlDocument document)
        {
            if (document.QuerySelectorAll("#reproduction-bottom table").Length == 0)
            {
                if (acc.MaleHorse == null)
                {
                    var doc1 = Parser.ParseDocument(await acc.Client.GetAsync($"/elevage/fiche/arbre?id={Id}").ConfigureAwait(false));
                    var a = doc1.QuerySelectorAll(".width-100.spacer-large-top")[doc1.QuerySelectorAll(".width-100.spacer-large-top").Length - 2].QuerySelectorAll(".horsename")[0];
                    string id = a.GetAttribute("href").Split('=')[1];
                    string name = a.TextContent;
                    acc.MaleHorse = new Horse(id, name, acc);
                    acc.MaleHorse.LoadInfo(await acc.MaleHorse.GetDoc().ConfigureAwait(false));
                }
                await acc.RepeatHorsingMale(Id).ConfigureAwait(false);
                document = await GetDoc().ConfigureAwait(false);
            }
            Status = Properties.Resources.HorseStatusMaleHorsing;
            if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > 3000)
            {
                await acc.Sell(acc.MainProductToSell, (3000 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
            }
            else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > 3000)
            {
                await acc.Sell(acc.SubProductToSell, (3000 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
            }
            string href = document.QuerySelectorAll("#reproduction-bottom a")[2].GetAttribute("href");
            var doc = Parser.ParseDocument(await acc.Client.GetAsync(href).ConfigureAwait(false));
            string answer;
            do
            {
                string script = doc.QuerySelectorAll("#page-contents script").Last().InnerHtml;
                string postData = Regex.Match(script, "params: \"(.*?)\"}").Groups[1].Value;
                answer = await acc.Client.PostAsync("/elevage/chevaux/doReproduction", postData).ConfigureAwait(false);
            } while (answer.Contains("\"errors\":[\"internal\"]"));
        }

        public async Task<HorseSex> Echography()
        {
            Status = Properties.Resources.HorseStatsuEcho;
            if (acc.Equ < 50)
            {
                if (acc.Settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > 500)
                {
                    await acc.Sell(acc.MainProductToSell, (500 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                }
                else if (acc.Settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > 500)
                {
                    await acc.Sell(acc.SubProductToSell, (500 / acc.MainProductToSell.SellPrice).ToString()).ConfigureAwait(false);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, $"{Properties.Resources.AccountOutOfResourcesMessage1} {acc.Login} {Properties.Resources.AccountOutOfResourcesMessage2}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
            }
            string json = await acc.Client.PostAsync("/elevage/chevaux/doSonogram", $"id={Id}");
            var document = Parser.ParseDocument(json, "blocks", "reproduction-body-content");
            string text = document.QuerySelectorAll("#reproduction-tab-1 .col-1")[0].TextContent;
            string femaleName;
            switch (acc.Server)
            {
                case Server.Russia: femaleName = "кобыла"; break;
                case Server.International: femaleName = "filly"; break;
                case Server.Norway: femaleName = "hoppeføll"; break;
                case Server.Spain: femaleName = "potranca"; break;
                case Server.Australia: femaleName = "filly"; break;
                case Server.Canada: femaleName = "filly"; break;
                case Server.USA: femaleName = "filly"; break;
                case Server.England: femaleName = "filly"; break;
                case Server.Germany: femaleName = "Stutfohlen"; break;
                case Server.Bulgaria: femaleName = "кобилка"; break;
                case Server.Arabic: femaleName = "مهرة"; break;
                case Server.Romain: femaleName = "mânză"; break;
                case Server.Sweden: femaleName = "stoföl"; break;
                case Server.FranceGaia: femaleName = "pouliche"; break;
                case Server.FranceOuranos: femaleName = "pouliche"; break;
                case Server.Poland: femaleName = "klaczka"; break;
                default:
                    string strongName = document.QuerySelectorAll("#characteristics-body-content .dashed")[2].GetElementsByTagName("strong")[0].TextContent;
                    femaleName = document.QuerySelectorAll("#characteristics-body-content .dashed")[2].FirstChild.TextContent.Substring(strongName.Length + 1);
                    break;
            }
            return text.Contains(femaleName) ? HorseSex.Female : HorseSex.Male;
        }

        public HorseSex Echography(IHtmlDocument document)
        {
            string text = document.QuerySelectorAll("#reproduction-tab-1 .col-1")[0].TextContent;
            string femaleName;
            switch (acc.Server)
            {
                case Server.Russia: femaleName = "кобыла"; break;
                case Server.International: femaleName = "filly"; break;
                case Server.Norway: femaleName = "hoppeføll"; break;
                case Server.Spain: femaleName = "potranca"; break;
                case Server.Australia: femaleName = "filly"; break;
                case Server.Canada: femaleName = "filly"; break;
                case Server.USA: femaleName = "filly"; break;
                case Server.England: femaleName = "filly"; break;
                case Server.Germany: femaleName = "Stutfohlen"; break;
                case Server.Bulgaria: femaleName = "кобилка"; break;
                case Server.Arabic: femaleName = "مهرة"; break;
                case Server.Romain: femaleName = "mânză"; break;
                case Server.FranceGaia: femaleName = "pouliche"; break;
                case Server.FranceOuranos: femaleName = "pouliche"; break;
                case Server.Poland: femaleName = "klaczka"; break;
                default:
                    string strongName = document.QuerySelectorAll("#characteristics-body-content .dashed")[2].GetElementsByTagName("strong")[0].TextContent;
                    femaleName = document.QuerySelectorAll("#characteristics-body-content .dashed")[2].FirstChild.TextContent.Substring(strongName.Length + 1);
                    break;
            }
            return text.Contains(femaleName) ? HorseSex.Female : HorseSex.Male;
        }

        public async Task<Horse> Birth()
        {
            Status = Properties.Resources.HorseStatusBirth;
            var doc = Parser.ParseDocument(await acc.Client.GetAsync($"/elevage/chevaux/mettreBas?jument={Id}").ConfigureAwait(false));
            string sex = doc.QuerySelectorAll(".form-select-name img")[0].GetAttribute("alt");
            string affixValue = GetRenameValue("affixe-1", doc, acc.Settings.Affix);
            string farmValue = GetRenameValue("elevage-1", doc, acc.Settings.Farm);
            string horseName;
            if (acc.Settings.RandomNames)
            {
                horseName = (sex == "male") ? Properties.Resources.MaleNames.Split(',')[new Random().Next(0, Properties.Resources.MaleNames.Split(',').Length)] : Properties.Resources.FemaleNames.Split(',')[new Random().Next(0, Properties.Resources.FemaleNames.Split(',').Length)];
            }
            else
            {
                horseName = (sex == "male") ? acc.Settings.MaleName : acc.Settings.FemaleName;
            }
            string postData = $"valider=ok&poulain-1={horseName}&affixe-1={affixValue}&elevage-1={farmValue}";
            string answer;
            do
            {
                answer = await acc.Client.PostAsync($"/elevage/chevaux/choisirNoms?jument={Id}", postData).ConfigureAwait(false);
            } while (!answer.Contains("<!DOCTYPE html>"));
            doc = Parser.ParseDocument(answer);
            var horse = new Horse(doc.QuerySelectorAll(".horse-name a")[0].GetAttribute("href").Split('=')[1], horseName, acc)
            {
                Scheme = new ObservableCollection<Training>(),
                Sex = (sex == "male") ? HorseSex.Male : HorseSex.Female
            };
            foreach (var training in acc.Settings.Scheme)
            {
                var t = new Training(training.Name, training.Value);
                horse.Scheme.Add(t);
            }
            return horse;
        }
        #endregion
    }
}