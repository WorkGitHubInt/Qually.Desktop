using AngleSharp.Html.Dom;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BotQually
{
    public class Horse
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string Id { get; private set; }
        public string Name { get; private set; }
        private readonly Account acc;
        private byte health;
        private byte energy;
        private int age;
        public byte Health
        {
            get => health;
            private set
            {
                if (value >= 0 && value <= 100)
                {
                    health = value;
                }
                else
                {
                    throw new Exception("Ошибка в здоровье" + value);
                }
            }
        }
        public byte Energy
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
            private set
            {
                if (value >= 0)
                {
                    age = value;
                }
                else
                {
                    throw new Exception("Ошибка в возрасте" + value);
                }
            }
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

        #region Simple actions
        public void LoadInfo(IHtmlDocument document)
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
        }

        private string ParseForm(IHtmlDocument document, string formId)
        {
            var form = document.GetElementById(formId);
            string value = form.Children[1].GetAttribute("value").ToLower();
            string name1 = form.Children[1].GetAttribute("name").ToLower();
            string postData;
            if (formId == "form-do-eat-treat-carotte")
            {
                postData = $"{name1}={value}&id={Id}&friandise=carrote";
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
                string name3 = form.Children[3].GetAttribute("name").Substring(formId.Length).ToLower();
                string name4 = form.Children[4].GetAttribute("name").Substring(formId.Length).ToLower();
                postData += $"&{name3}={new Random().Next(20, 60)}&{name4}={new Random().Next(20, 60)}";
            }
            return postData;
        }

        public async Task<string> Action(IHtmlDocument document, string formId, string selector, string action)
        {
            string answer = "";
            if (!document.QuerySelector(selector).ClassName.Contains("action-disabled"))
            {
                string postData = ParseForm(document, formId);
                answer = await acc.Client.PostAsync($"/elevage/chevaux/{action}", postData).ConfigureAwait(false);
            }
            return answer;
        }

        public async Task Mission()
        {
            await acc.Client.PostAsync("/elevage/chevaux/doCentreMission", $"id={Id}").ConfigureAwait(false);
            Energy -= 30;
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

        public async Task Feeding(IHtmlDocument document)
        {
            string postData;
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
                    await acc.Client.PostAsync("/elevage/chevaux/doEat", postData).ConfigureAwait(false);
                }
            }
            else
            {
                ParseFood(document, "hay", out string hayName, out int hay);
                ParseFood(document, "oats", out string oatName, out int oat);
                postData = ParseForm(document, "feeding") + $"&{hayName}={hay.ToString()}&{oatName}={oat.ToString()}";
                if (hay != 0 || oat != 0)
                {
                    await acc.Client.PostAsync("/elevage/chevaux/doEat", postData).ConfigureAwait(false);
                }
            }
        }
        #endregion

        #region Centre
        public async Task Centre(string competence, Settings settings)
        {
            string answer;
            do
            {
                await MoneyCheck(settings).ConfigureAwait(false);
                string fourage = settings.CentreHay ? "1" : "2";
                string avoine = settings.CentreOat ? "1" : "2";
                int selectTarif = 2;
                switch (settings.CentreDuration)
                {
                    case "1": selectTarif = 1; break;
                    case "3": selectTarif = 2; break;
                    case "10": selectTarif = 3; break;
                    case "30": selectTarif = 4; break;
                    case "60": selectTarif = 5; break;
                }
                string postData = $"cheval={Id}&elevage=&cheval={Id}&competence={competence}&tri=tarif{selectTarif}&sens=ASC&tarif=&leconsPrix=&foret=2&montagne=2&plage=2&classique=2&western=2&fourrage={fourage}&avoine={avoine}&carotte=2&mash=2&hasSelles=2&hasBrides=2&hasTapis=2&hasBonnets=2&hasBandes=2&abreuvoir=2&douche=2&centre=&centreBox=0&directeur=&prestige=&bonus=&boxType=&boxLitiere=&prePrestige=&prodSelles=&prodBrides=&prodTapis=&prodBonnets=&prodBandes=";
                string json;
                do
                {
                    json = await acc.Client.PostAsync("/elevage/chevaux/centreSelection", postData).ConfigureAwait(false);
                } while (!json.Contains("{\"content\":"));
                var doc = Parser.ParseDocument(json, "content");
                var scripts = doc.QuerySelectorAll("#table-0 tbody tr")[0].QuerySelectorAll("script");
                for (int i = 0; i < scripts.Length; i++)
                {
                    if (scripts[i].InnerHtml.Contains("duree=" + settings.CentreDuration + "&elevage="))
                    {
                        postData = Regex.Match(scripts[i].InnerHtml, "{'params': '(.*?)'}").Groups[1].Value;
                    }
                }
                answer = await acc.Client.PostAsync("/elevage/chevaux/doCentreInscription", postData).ConfigureAwait(false);
            } while (!answer.Contains("message=centreOk"));
        }

        public async Task CentreReserve(Settings settings)
        {
            string answer;
            do
            {
                await MoneyCheck(settings).ConfigureAwait(false);
                string id = settings.ReserveID;
                if (settings.SelfReserve)
                {
                    var document = Parser.ParseDocument(await acc.Client.GetAsync("/centre/bureau/").ConfigureAwait(false));
                    id = document.QuerySelectorAll("#page-contents .action.action-style-2")[0].GetAttribute("href").Split('&')[0].Split('=')[1];
                }
                int selectTarif = 2;
                switch (settings.ReserveDuration)
                {
                    case "1": selectTarif = 1; break;
                    case "3": selectTarif = 2; break;
                    case "10": selectTarif = 3; break;
                    case "30": selectTarif = 4; break;
                    case "60": selectTarif = 5; break;
                }
                string postData = await acc.Client.PostAsync($"/elevage/chevaux/boxReserveSelection", $"cheval={Id}&tri=tarif{selectTarif}&sens=ASC&cheval={Id}").ConfigureAwait(false);
                JObject jobj = JObject.Parse(postData);
                if ((bool)jobj["hasBox"])
                {
                    var doc = Parser.ParseDocument(postData, "content");
                    if (doc.QuerySelectorAll(".pageNumbering").Length > 0)
                    {
                        int last = Convert.ToInt32(doc.QuerySelectorAll(".pageNumbering a").Last().GetAttribute("data-page")) + 1;
                        for (int i = 0; i < last; i++)
                        {
                            doc = Parser.ParseDocument(await acc.Client.PostAsync($"/elevage/chevaux/boxReserveSelection", $"cheval={Id}&tri=tarif{selectTarif}&sens=ASC&cheval={Id}&page={i}").ConfigureAwait(false), "content");
                            postData = ScriptSearch(doc, id, settings);
                            if (postData != default)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        postData = ScriptSearch(doc, id, settings);
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
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MainViewModel.GetInstance().Notifications.Add($"Аккаунт: {acc.Login}; Нет резервных стойл.");
                    });
                    answer = "0";
                }
            } while (!answer.Contains("message=centreOk") && answer != "0");
            if (answer.Contains("message=centreOk"))
            {
                if (settings.Continue)
                {
                    await CentreExtend(settings).ConfigureAwait(false);
                }
            }
            if (answer == "0" && settings.WriteToAll)
            {
                await Centre("0", settings).ConfigureAwait(false);
            }
        }

        public async Task CentreExtend(Settings settings)
        {
            await acc.Client.PostAsync("/elevage/chevaux/doCenterExtends", $"id={Id}&duration={settings.ContinueDuration}").ConfigureAwait(false);
        }

        private string ScriptSearch(IHtmlDocument document, string id, Settings settings)
        {
            string postData = "";
            var scripts = document.QuerySelectorAll("#table-0 tbody")[0].QuerySelectorAll("script");
            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i].InnerHtml.Contains($"id={Id}&centre={id}") && scripts[i].InnerHtml.Contains($"duree={settings.ReserveDuration}"))
                {
                    postData = Regex.Match(scripts[i].InnerHtml, "{'params': '(.*?)'}").Groups[1].Value;
                    break;
                }
            }
            return postData;
        }

        private async Task MoneyCheck(Settings settings)
        {
            int min = 0;
            switch (settings.CentreDuration)
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
                if (settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > min + 100)
                {
                    await acc.Sell(acc.MainProductToSell, min.ToString()).ConfigureAwait(false);
                }
                else if (settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > min + 100)
                {
                    await acc.Sell(acc.SubProductToSell, min.ToString()).ConfigureAwait(false);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        GlobalSettings globalSettings = GlobalSettings.GetInstance();
                        if (globalSettings.MoneyNotification)
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                MainViewModel.GetInstance().Notifications.Add($"Аккаунт: {acc.Login} закончились деньги и/или ресурсы");
                            });
                        }
                        else
                        {
                            if (!acc.IsEquMessageShown)
                            {
                                acc.IsEquMessageShown = true;
                                MessageBox.Show(Application.Current.MainWindow, $"На аккаунте {acc.Login} закончилось экю и нет ресурсов для пополнения! Пополните запасы и нажмите ОК", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }));
                }
                #pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                acc.LoadProducts().ConfigureAwait(false);
            }
        }
        #endregion

        #region Horsing
        public async Task<string> Birth(Settings settings)
        {
            string[] maleNames = Properties.Resources.MaleNames.Split(',');
            string[] femaleNames = Properties.Resources.MaleNames.Split(',');
            string maleName = settings.RandomNames ?  maleNames[new Random().Next(0, maleNames.Length - 4)] : settings.MaleName;
            string femaleName = settings.RandomNames ? femaleNames[new Random().Next(0, femaleNames.Length - 4)] : settings.FemaleName;
            await acc.Client.GetAsync($"/elevage/chevaux/mettreBas?jument={Id}").ConfigureAwait(false);
            string html = await acc.Client.GetAsync($"/elevage/chevaux/choisirNoms?jument={Id}").ConfigureAwait(false);
            var doc = Parser.ParseDocument(html);
            string affixValue = "";
            foreach (var option in doc.QuerySelectorAll("#affixe-1 option"))
            {
                if (option.TextContent.Trim() == settings.Affix)
                {
                    affixValue = option.GetAttribute("value");
                }
            }
            string farmValue = "";
            foreach (var option in doc.QuerySelectorAll("#elevage-1 option"))
            {
                if (!option.HasAttribute("disabled"))
                {
                    if (option.TextContent.Trim() == settings.Farm)
                    {
                        farmValue = option.GetAttribute("value");
                    }
                }
            }
            string sex = doc.QuerySelectorAll(".form-select-name img")[0].GetAttribute("alt");
            string name = sex == "male" ? maleName : femaleName;
            string postData = $"valider=ok&poulain-1={name}&affixe-1={affixValue}&elevage-1={farmValue}";
            string answer;
            do
            {
                answer = await acc.Client.PostAsync($"/elevage/chevaux/choisirNoms?jument={Id}", postData).ConfigureAwait(false);
            } while (!answer.Contains("<!DOCTYPE html>"));
            return answer;
        }

        public async Task HorsingMale(IHtmlDocument document, Settings settings)
        {
            Energy = Convert.ToByte(document.QuerySelector("#energie").TextContent);
            string postData;
            if (settings.HorsingMaleCommand)
            {
                postData = $"id={Id}&action=save&type=equipe&price=0&owner=&nom=";
            } else
            {
                postData = $"id={Id}&action=save&type=public&price={settings.HorsingMalePrice}&owner=&nom=";
            }
            await HorsingCycle(postData).ConfigureAwait(false);
            await Action(document, "form-do-stroke", "#boutonCaresser", "doStroke").ConfigureAwait(false);
            if (settings.Carrot)
            {
                await Action(document, "form-do-eat-treat-carotte", "#boutonCarotte", "doEatTreat").ConfigureAwait(false);
            }
            await Action(document, "form-do-drink", "#boutonBoire", "doDrink").ConfigureAwait(false);
            document = await GetDoc().ConfigureAwait(false);
            Energy = Convert.ToByte(document.QuerySelector("#energie").TextContent);
            await HorsingCycle(postData).ConfigureAwait(false);
        }

        private async Task HorsingCycle(string postData)
        {
            while (Energy - 25 > 20)
            {
                await acc.Client.PostAsync("/elevage/chevaux/reserverJument", postData).ConfigureAwait(false);
                Energy -= 25;
            }
        }

        private async Task<string> HorsingFemaleSettings(Settings settings, IHtmlDocument document)
        {
            string raceIndex = "";
            string gp = settings.HorsingFemaleCommand ? settings.GPEdge : "0";
            string type = settings.HorsingFemaleCommand ? "equipe" : "public";
            bool blood = settings.ClearBlood;
            string price = settings.HorsingFemaleCommand ? "0" : settings.HorsingFemalePrice;
            string breeder = settings.Breeder;
            if (settings.SelfMale)
            {
                breeder = acc.Login;
            }
            string race = document.QuerySelector("#characteristics-body-content a:first-child").TextContent;
            string purete = "2";
            if (blood)
            {
                purete = "1";
                var doc = Parser.ParseDocument(await acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument={Id}").ConfigureAwait(false));
                var options = doc.QuerySelectorAll("#race option");
                int count = default, num = default;
                for (int i = 0; i < options.Length; i++)
                {
                    if (race == options[i].TextContent)
                    {
                        num = count;
                    }
                    count++;
                }
                raceIndex = options[num].GetAttribute("value");
            }
            if (settings.HorsingFemaleCommand)
            {
                return $"tri=cTotal&page=0&sens=DESC&rechercher=1&breeder={breeder}&potentielTotal={gp}&prix=0&blup=-100&race={raceIndex}&purete={purete}&cE=0&cV=0&cD=0&cG=0&cT=0&cS=0&cheval=1&poney=1&pegase=1&=1&potentielTotalC=l&jument={Id}&type=equipe";
            } else
            {
                return $"tri=cTotal&page=0&sens=DESC&rechercher=1&breeder={breeder}&potentielTotal={gp}&prix={price}&blup=-100&race={raceIndex}&purete={purete}&cE=0&cV=0&cD=0&cG=0&cT=0&cS=0&cheval=1&poney=1&pegase=1&=1&prixC=l&jument={Id}&type=public";
            }
        }

        public async Task HorsingFemale(IHtmlDocument document, Settings settings)
        {
            try
            {
                var reproduction1 = document.QuerySelectorAll("#reproduction-tab-0 a");
                if (reproduction1.Length > 0)
                {
                    var reproduction = reproduction1.Last();
                    if (reproduction.Id != "boutonEchographie" && !reproduction.ClassName.Contains("disabled"))
                    {
                        if (document.QuerySelector("#tab-description") == null && document.QuerySelectorAll("#reproduction > .message").Length == 0)
                        {
                            string answer = default;
                            do
                            {
                                int horsingFemalePrice = Convert.ToInt32(settings.HorsingFemalePrice) + 100;
                                if (acc.Equ < horsingFemalePrice)
                                {
                                    if (settings.MainProductToSell != ProductType.None && acc.MainProductToSell.Amount * acc.MainProductToSell.SellPrice > horsingFemalePrice)
                                    {
                                        await acc.Sell(acc.MainProductToSell, settings.HorsingFemalePrice).ConfigureAwait(false);
                                    }
                                    else if (settings.SubProductToSell != ProductType.None && acc.SubProductToSell.Amount * acc.SubProductToSell.SellPrice > horsingFemalePrice)
                                    {
                                        await acc.Sell(acc.SubProductToSell, settings.HorsingFemalePrice).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                        {
                                            GlobalSettings globalSettings = GlobalSettings.GetInstance();
                                            if (globalSettings.MoneyNotification)
                                            {
                                                Application.Current.Dispatcher.Invoke(delegate
                                                {
                                                    MainViewModel.GetInstance().Notifications.Add($"Аккаунт: {acc.Login} закончились деньги и/или ресурсы");
                                                });
                                            }
                                            else
                                            {
                                                if (!acc.IsEquMessageShown)
                                                {
                                                    acc.IsEquMessageShown = true;
                                                    MessageBox.Show(Application.Current.MainWindow, $"На аккаунте {acc.Login} закончилось экю и нет ресурсов для пополнения! Пополните запасы и нажмите ОК", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                }
                                            }
                                        }));
                                    }
                                }
                                string searchString = await HorsingFemaleSettings(settings, document).ConfigureAwait(false);
                                do
                                {
                                    var doc = Parser.ParseDocument(await acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?{searchString}").ConfigureAwait(false));
                                    if (doc.QuerySelectorAll(".align-center.action a").Length > 0)
                                    {
                                        var refs = doc.QuerySelectorAll(".align-center.action a");
                                        string action = refs[new Random().Next(0, refs.Length - 1)].GetAttribute("href");
                                        doc = Parser.ParseDocument(await acc.Client.GetAsync(action).ConfigureAwait(false));
                                        string script = doc.QuerySelectorAll("#page-contents script").Last().InnerHtml;
                                        if (!script.Contains("return window.location"))
                                        {
                                            string postData = script.Substring(script.IndexOf("params") + 9);
                                            postData = postData.Substring(0, postData.Length - 16);
                                            answer = await acc.Client.PostAsync("/elevage/chevaux/doReproduction", postData).ConfigureAwait(false);
                                        } else
                                        {
                                            answer = "1";
                                        }
                                    }
                                    else
                                    {
                                        answer = "1";
                                    }
                                } while (answer.Contains("\"errors\":[\"internal\"]"));
                            } while (answer.Contains("saillieArgent") || !answer.Contains("?id=") && !(answer == "1"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MainViewModel.GetInstance().Notifications.Add($"Ошибка ж. случки Аккаунт: {acc.Login}; Лошадь: {Name}");
                });
                logger.Error($"Аккаунт {acc.Login} ошибка при ж случках: {e.Message}\n{e.StackTrace}");
            }
        }
        #endregion

        public async Task<bool> CheckHorse(IHtmlDocument document, Settings settings)
        {
            var history = document.GetElementsByClassName("spacer-small-bottom button button-style-0");
            if (document.GetElementsByClassName("h2").Length != 0)
            {
                await acc.Client.GetAsync($"/elevage/chevaux/paradis?id={Id}").ConfigureAwait(false);
                return false;
            }
            else if (document.QuerySelector("#tab-gift-title") != null)
            {
                return false;
            }
            else if (document.QuerySelector("#poulain-1") != null)
            {
                string mom = document.Forms[0].GetAttribute("action").Substring(35);
                string sex = document.QuerySelectorAll(".form-select-name img")[0].GetAttribute("alt");
                string littleHorse;
                if (sex == "male")
                {
                    littleHorse = settings.MaleName;
                }
                else
                {
                    littleHorse = settings.FemaleName;
                }
                string postData = "valider=ok&poulain-1=" + littleHorse + "&affixe-1=&elevage-1=";
                await acc.Client.PostAsync("/elevage/chevaux/choisirNoms?jument=" + mom, postData).ConfigureAwait(false);
                return false;
            }
            else if (document.QuerySelector("#vieillissement-head-title") != null || !settings.OldHorses && Age >= 360 || document.QuerySelector("#annulation") != null || history.Length > 0)
            {
                return false;
            }
            else if (Age >= 360 && !settings.OldHorses)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}