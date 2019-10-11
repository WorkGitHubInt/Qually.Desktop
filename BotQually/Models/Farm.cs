using AngleSharp.Dom.Html;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BotQually
{
    public class Farm : BaseModel
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string Name { get; set; }
        public string Count { get; set; }
        public string Id { get; private set; }
        public Account Acc { get; private set; }
        public Queue<Horse> Horses = new Queue<Horse>();
        public GlobalSettings Settings { get; set; }

        public Farm(string name, string id, Account acc)
        {
            Name = name;
            Id = id;
            Acc = acc;
            Task.Run(() => LoadCount());
        }

        public async Task LoadCount()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", $"go=1&id={Id}&chevalType=&race-ane=49&race-cheval=&race-poney=&race-cheval-trait=&race-all=&race-pegase=&race-licorne=&race-licorne-ailee=&race-cheval-trait-aile=&chevalTypeRace=&aneRaceId=49&ageComparaison=g&age=0&uniteAge=ans&chevalNom=&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r58=&r38=&r43=&r31=&r60=&r30=&r41=&r32=&r61=&r45=&r33=&r71=&r53=&r63=&r72=&r57=&r40=&r46=&r50=&r55=&r56=&r39=&r49=&r36=&r66=&r64=&r59=&r51=&r42=&r73=&r65=&r34=&r70=&r35=&r48=&r52=&r44=&r37=&r54=&r67=&r62=&gestation=2&nbSaillieComparaison=g&nbSaillie=0&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&bride=&selle=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=0&search=1&noFilter=1&advanced=1").ConfigureAwait(false));
            string count = System.Text.RegularExpressions.Regex.Match(document.QuerySelectorAll("script").Last().InnerHtml.Replace("\\", ""), "<span style=\"display:inline\" class=\"count\">(.*?)</span>").Groups[1].Value.Trim().Substring(1);
            Count = count.Substring(0, count.Length - 1);
        }

        public async Task LoadHorses(Settings settings)
        {
            Horses = new Queue<Horse>();
            string postData;
            if (settings.LoadSleep)
            {
                postData = $"go=1&id={Id}&filter=all&sort={Settings.Sort}";
            }
            else
            {
                postData = $"go=1&id={Id}&sort={Settings.Sort}&filter=all&chevalType=&race-ane=49&race-cheval=&race-poney=&race-cheval-trait=&race-all=&race-pegase=&race-licorne=&race-licorne-ailee=&race-cheval-trait-aile=&chevalTypeRace=&aneRaceId=49&ageComparaison=g&age=0&uniteAge=ans&chevalNom=&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r58=&r38=&r43=&r31=&r60=&r30=&r41=&r32=&r61=&r45=&r33=&r71=&r53=&r63=&r72=&r57=&r40=&r46=&r50=&r55=&r56=&r39=&r49=&r36=&r66=&r64=&r59=&r51=&r42=&r73=&r65=&r34=&r70=&r35=&r48=&r52=&r44=&r37=&r54=&r67=&r62=&gestation=2&nbSaillieComparaison=g&nbSaillie=0&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&bride=&selle=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=0&vente=2&search=1&noFilter=1&advanced=1";
            }
            var document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", postData).ConfigureAwait(false));
            var pages = document.QuerySelectorAll(".pageNumbering");
            if (pages.Length > 0)
            {
                var page = document.QuerySelectorAll(".pageNumbering li a")[document.QuerySelectorAll(".pageNumbering li a").Length - 1];
                if (page.HasAttribute("rel"))
                {
                    page = document.QuerySelectorAll(".pageNumbering li a")[document.QuerySelectorAll(".pageNumbering li a").Length - 2];
                }
                int last = Convert.ToInt32(page.GetAttribute("data-page")) + 1;
                for (int i = 0; i <= last; i++)
                {
                    if (settings.LoadSleep)
                    {
                        postData = $"go=1&id={Id}&startingPage={i.ToString()}&filter=all&sort={Settings.Sort}";
                    }
                    else
                    {
                        postData = $"go=1&id={Id}&startingPage={i.ToString()}&filter=all&sort={Settings.Sort}&chevalType=&race-ane=49&race-cheval=&race-poney=&race-cheval-trait=&race-all=&race-pegase=&race-licorne=&race-licorne-ailee=&race-cheval-trait-aile=&chevalTypeRace=&aneRaceId=49&ageComparaison=g&age=0&uniteAge=ans&chevalNom=&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r58=&r38=&r43=&r31=&r60=&r30=&r41=&r32=&r61=&r45=&r33=&r71=&r53=&r63=&r72=&r57=&r40=&r46=&r50=&r55=&r56=&r39=&r49=&r36=&r66=&r64=&r59=&r51=&r42=&r73=&r65=&r34=&r70=&r35=&r48=&r52=&r44=&r37=&r54=&r67=&r62=&gestation=2&nbSaillieComparaison=g&nbSaillie=0&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&bride=&selle=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=0&vente=2&search=1&noFilter=1&advanced=1";
                    }

                    document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", postData).ConfigureAwait(false));
                    ParsePage(document);
                }
            }
            else
            {
                ParsePage(document);
            }
            Count = Horses.Count.ToString();
        }

        private void ParsePage(IHtmlDocument document)
        {
            var horseName = document.QuerySelectorAll(".horsename");
            for (int i = 0; i < horseName.Length; i++)
            {
                string horseId = document.QuerySelectorAll(".horsename")[i].GetAttribute("href").Split('=')[1];
                string name = document.QuerySelectorAll(".horsename")[i].TextContent;
                Horses.Enqueue(new Horse(horseId, name, Acc));
            }
        }

        public async Task Run(Settings settings, GlobalSettings globalSettings, CancellationToken ct)
        {
            Settings = globalSettings;
            if (settings.BuyHay != "")
            {
                buyHayInt = Convert.ToInt32(settings.BuyHay) * 2 + 100;
            }
            if (settings.BuyOat != "")
            {
                buyOatInt = Convert.ToInt32(settings.BuyOat) * 2 + 100;
            }
            await LoadHorses(settings).ConfigureAwait(false);
            var tasks = new List<Task>();
            int count = Horses.Count;
            using (var semaphore = new SemaphoreSlim(5))
            {
                for (int i = 0 + settings.SkipIndex; i < count; i++)
                {
                    Acc.ProgressHorse = i.ToString();
                    Acc.Progress = Math.Round(Convert.ToDouble(i) * 100D / Convert.ToDouble(count), 2).ToString() + "%";
                    settings.SkipIndex = 0;
                    if (Settings.ParallelHorse)
                    {
                        await semaphore.WaitAsync(ct).ConfigureAwait(false);
                        tasks.Clear();
                        tasks.Add(HorseRun(Horses.Dequeue(), settings, semaphore));
                    }
                    else
                    {
                        await HorseRun(Horses.Dequeue(), settings, semaphore).ConfigureAwait(false);
                    }
                }
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            }
            Acc.Progress = "100%";
            if (lowHealthCount > 50)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    IoC.Kernel.Get<MainViewModel>().Notifications.Add(Acc.Login + "Обнаружено сниженое здоровье более чем на 50 лошадях!");
                });
            }
            tasks.Clear();
            if (settings.GoBabies)
            {
                using (var semaphore = new SemaphoreSlim(5))
                {
                    count = Babies.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Acc.Progress = "прогон жеребят " + i;
                        if (Settings.ParallelHorse)
                        {
                            await semaphore.WaitAsync(ct).ConfigureAwait(false);
                            tasks.Clear();
                            tasks.Add(HorseRun(Babies.Dequeue(), settings, semaphore));
                        }
                        else
                        {
                            await HorseRun(Babies.Dequeue(), settings, semaphore).ConfigureAwait(false);
                        }
                    }
                    await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
                }
            }
        }

        private int buyHayInt = 0;
        private int buyOatInt = 0;
        private Queue<Horse> Babies = new Queue<Horse>();
        private int lowHealthCount = 0;
        public async Task HorseRun(Horse horse, Settings settings, SemaphoreSlim semaphore)
        {
            IHtmlDocument document = await horse.GetDoc().ConfigureAwait(false);
            if (document.QuerySelector("#tab-gift-title") == null && document.GetElementsByClassName("h2").Length == 0)
            {
                horse.LoadInfo(document);
            }
            if (await horse.CheckHorse(document, settings).ConfigureAwait(false))
            {
                if (horse.Health > Convert.ToInt32(settings.HealthEdge))
                {
                    if (horse.Age >= 6 && document.QuerySelector("#cheval-inscription") != null)
                    {
                        if (settings.ReserveID != "" || settings.SelfReserve)
                        {
                            await horse.CentreReserve(settings).ConfigureAwait(false);
                        }
                        else
                        {
                            await horse.Centre("0", settings).ConfigureAwait(false);
                        }
                        document = await horse.GetDoc().ConfigureAwait(false);
                    }
                    await Pause().ConfigureAwait(false);
                    if (horse.Sex == HorseSex.Female && document.QuerySelector("#lienVeterinaire") != null)
                    {
                        string html = await horse.Birth(settings, Settings).ConfigureAwait(false);
                        if (settings.GoBabies)
                        {
                            string script = Parser.ParseDocument(html).QuerySelectorAll("#page-contents script")[1].InnerHtml;
                            string id = System.Text.RegularExpressions.Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
                            Babies.Enqueue(new Horse(id, "", Acc));
                        }
                        document = await horse.GetDoc().ConfigureAwait(false);
                        horse.LoadInfo(document);
                    }
                    await Pause().ConfigureAwait(false);
                    await horse.Action(document, "form-do-groom", "#boutonPanser", "doGroom").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    if (settings.Mission && horse.Age >= 24 && horse.Energy - 30 > 20 && document.QuerySelector("#mission-tab-0") != null)
                    {
                        if (!document.QuerySelectorAll("#mission-tab-0 a")[0].ClassName.Contains("action-disabled"))
                        {
                            await horse.Mission().ConfigureAwait(false);
                        }
                    }
                    await Pause().ConfigureAwait(false);
                    if (horse.Age >= 30 && horse.Energy - 25 > 20)
                    {
                        if (horse.Sex == HorseSex.Male && settings.HorsingMale && document.QuerySelector("#reproduction-tab-0") != null && !document.QuerySelectorAll("#reproduction-tab-0 a").Last().ClassName.Contains("action-disable"))
                        {
                            await horse.HorsingMale(document, settings).ConfigureAwait(false);
                        }
                        else if (horse.Sex == HorseSex.Female && settings.HorsingFemale)
                        {
                            await horse.HorsingFemale(document, settings).ConfigureAwait(false);
                        }
                    }
                    await Pause().ConfigureAwait(false);
                    if (buyHayInt > 0)
                    {
                        await CheckFood(Acc.Hay.Amount, settings.BuyHay, buyHayInt, Acc.Hay, settings).ConfigureAwait(false);
                    }
                    if (buyOatInt > 0)
                    {
                        await CheckFood(Acc.Oat.Amount, settings.BuyOat, buyOatInt, Acc.Oat, settings).ConfigureAwait(false);
                    }
                    await Pause().ConfigureAwait(false);
                    document = await horse.GetDoc().ConfigureAwait(false);
                    await horse.Feeding(document).ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    document = await horse.GetDoc().ConfigureAwait(false);
                    await horse.Action(document, "form-do-night", "#boutonCoucher", "doNight").ConfigureAwait(false);
                    Acc.LoadProducts().ConfigureAwait(false);
                }
                else
                {
                    lowHealthCount++;
                }
                if (semaphore != null)
                {
                    semaphore.Release();
                }
            }
        }

        private async Task CheckFood(int amount, string quantity, int buyInt, Product productToBuy, Settings settings)
        {
            if (quantity != "" && amount < 100)
            {
                if (Acc.Equ < buyInt)
                {
                    if (settings.MainProductToSell != ProductType.None && Acc.MainProductToSell.SellPrice * Acc.MainProductToSell.Amount > buyInt)
                    {
                        await Acc.Sell(Acc.MainProductToSell, quantity).ConfigureAwait(false);
                    }
                    else if (settings.SubProductToSell != ProductType.None && Acc.SubProductToSell.SellPrice * Acc.SubProductToSell.Amount > buyInt)
                    {
                        await Acc.Sell(Acc.SubProductToSell, quantity).ConfigureAwait(false);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, $"На аккаунте {Acc.Login} закончилось экю и нет ресурсов для пополнения! Пополните запасы и нажмите ОК", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                }
                await Acc.Buy(productToBuy, quantity).ConfigureAwait(false);
                Acc.LoadProducts().ConfigureAwait(false);
            }
        }

        private async Task Pause()
        {
            Random rnd = new Random();
            if (Settings.RandomPause)
            {
                int pause = rnd.Next(50, 100);
                await Task.Delay(pause).ConfigureAwait(false);
            }
        }
    }
}
