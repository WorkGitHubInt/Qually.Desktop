using AngleSharp.Html.Dom;
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
            string postData;
            if (string.IsNullOrEmpty(Id))
            {
                postData = $"go=1&chevalType=&chevalEspece=any-all&unicorn=2&pegasus=2&race-cheval=&race-poney=&race-ane=&race-cheval-trait=&race-all=&race-cheval-pegase=&race-poney-pegase=&race-cheval-licorne=&race-poney-licorne=&race-cheval-licorne-ailee=&race-poney-licorne-ailee=&race-cheval-trait-pegase=&race-cheval-trait-licorne=&race-cheval-trait-licorne-ailee=&race-ane-pegase=&race-ane-licorne=&race-ane-licorne-ailee=&chevalTypeRace=&aneRaceId=51&ageComparaison=g&age=0&uniteAge=ans&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r6=&r13=&r1=&r28=&r47=&r43=&r45=&r42=&r35=&r10=&r26=&r39=&r44=&r11=&r7=&r2=&r49=&r5=&r32=&r15=&r31=&r30=&r29=&r40=&r25=&r16=&r46=&r17=&r50=&r22=&r52=&r38=&r24=&r3=&r33=&r51=&r48=&r8=&r14=&r41=&r23=&r9=&r34=&r19=&r27=&r4=&r21=&r12=&gestation=2&nbSaillie=2&hasCompanion=2&chevalNom=&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&pack-samurai-dragon=2&pack-knight=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&trail-riding-diary=2&haunted-trail-riding-diary=2&greek-trail-riding-diary=2&winter-trail-riding-diary=2&coats-bundle-witch=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&iris-coat=2&button-braided-mane=2&tail-braid-1=2&tail-braid-2=2&clipping=2&parade-apple=2&alexandre-dumas-inkwell=2&arthur-conan-doyle-inkwell=2&heracles-horseshoes=2&sisyphus-boulder=2&selle=&bride=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=2&search=1&noFilter=1&advanced=0";
            }
            else
            {
                postData = $"go=1&id={Id}&chevalType=&chevalEspece=any-all&unicorn=2&pegasus=2&race-cheval=&race-poney=&race-ane=&race-cheval-trait=&race-all=&race-cheval-pegase=&race-poney-pegase=&race-cheval-licorne=&race-poney-licorne=&race-cheval-licorne-ailee=&race-poney-licorne-ailee=&race-cheval-trait-pegase=&race-cheval-trait-licorne=&race-cheval-trait-licorne-ailee=&race-ane-pegase=&race-ane-licorne=&race-ane-licorne-ailee=&chevalTypeRace=&aneRaceId=51&ageComparaison=g&age=0&uniteAge=ans&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r6=&r13=&r1=&r28=&r47=&r43=&r45=&r42=&r35=&r10=&r26=&r39=&r44=&r11=&r7=&r2=&r49=&r5=&r32=&r15=&r31=&r30=&r29=&r40=&r25=&r16=&r46=&r17=&r50=&r22=&r52=&r38=&r24=&r3=&r33=&r51=&r48=&r8=&r14=&r41=&r23=&r9=&r34=&r19=&r27=&r4=&r21=&r12=&gestation=2&nbSaillie=2&hasCompanion=2&chevalNom=&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&pack-samurai-dragon=2&pack-knight=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&trail-riding-diary=2&haunted-trail-riding-diary=2&greek-trail-riding-diary=2&winter-trail-riding-diary=2&coats-bundle-witch=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&iris-coat=2&button-braided-mane=2&tail-braid-1=2&tail-braid-2=2&clipping=2&parade-apple=2&alexandre-dumas-inkwell=2&arthur-conan-doyle-inkwell=2&heracles-horseshoes=2&sisyphus-boulder=2&selle=&bride=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=2&search=1&noFilter=1&advanced=0";
            }
            var document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", postData).ConfigureAwait(false));
            string count = System.Text.RegularExpressions.Regex.Match(document.QuerySelectorAll("script").Last().InnerHtml.Replace("\\", ""), "<span style=\"display:inline\" class=\"count\">(.*?)</span>").Groups[1].Value.Trim().Substring(1);
            Count = count.Substring(0, count.Length - 1);
        }

        public async Task LoadHorses(Settings settings)
        {
            Horses = new Queue<Horse>();
            string postData;
            if (settings.LoadSleep)
            {
                if (string.IsNullOrEmpty(Id))
                {
                    postData = $"go=1&sort={Settings.Sort}&filter=all";
                } else
                {
                    postData = $"go=1&id={Id}&sort={Settings.Sort}&filter=all";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Id))
                {
                    postData = $"go=1&sort={Settings.Sort}&filter=all&chevalType=&chevalEspece=any-all&unicorn=2&pegasus=2&race-cheval=&race-poney=&race-ane=&race-cheval-trait=&race-all=&race-cheval-pegase=&race-poney-pegase=&race-cheval-licorne=&race-poney-licorne=&race-cheval-licorne-ailee=&race-poney-licorne-ailee=&race-cheval-trait-pegase=&race-cheval-trait-licorne=&race-cheval-trait-licorne-ailee=&race-ane-pegase=&race-ane-licorne=&race-ane-licorne-ailee=&chevalTypeRace=&aneRaceId=51&ageComparaison=g&age=0&uniteAge=ans&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r6=&r13=&r1=&r28=&r47=&r43=&r45=&r42=&r35=&r10=&r26=&r39=&r44=&r11=&r7=&r2=&r49=&r5=&r32=&r15=&r31=&r30=&r29=&r40=&r25=&r16=&r46=&r17=&r50=&r22=&r52=&r38=&r24=&r3=&r33=&r51=&r48=&r8=&r14=&r41=&r23=&r9=&r34=&r19=&r27=&r4=&r21=&r12=&gestation=2&nbSaillie=2&hasCompanion=2&chevalNom=&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&pack-samurai-dragon=2&pack-knight=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&trail-riding-diary=2&haunted-trail-riding-diary=2&greek-trail-riding-diary=2&winter-trail-riding-diary=2&coats-bundle-witch=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&iris-coat=2&button-braided-mane=2&tail-braid-1=2&tail-braid-2=2&clipping=2&parade-apple=2&alexandre-dumas-inkwell=2&arthur-conan-doyle-inkwell=2&heracles-horseshoes=2&sisyphus-boulder=2&selle=&bride=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=2&search=1&noFilter=1&advanced=1";
                } else
                {
                    postData = $"go=1&id={Id}&sort={Settings.Sort}&filter=all&chevalType=&chevalEspece=any-all&unicorn=2&pegasus=2&race-cheval=&race-poney=&race-ane=&race-cheval-trait=&race-all=&race-cheval-pegase=&race-poney-pegase=&race-cheval-licorne=&race-poney-licorne=&race-cheval-licorne-ailee=&race-poney-licorne-ailee=&race-cheval-trait-pegase=&race-cheval-trait-licorne=&race-cheval-trait-licorne-ailee=&race-ane-pegase=&race-ane-licorne=&race-ane-licorne-ailee=&chevalTypeRace=&aneRaceId=51&ageComparaison=g&age=0&uniteAge=ans&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r6=&r13=&r1=&r28=&r47=&r43=&r45=&r42=&r35=&r10=&r26=&r39=&r44=&r11=&r7=&r2=&r49=&r5=&r32=&r15=&r31=&r30=&r29=&r40=&r25=&r16=&r46=&r17=&r50=&r22=&r52=&r38=&r24=&r3=&r33=&r51=&r48=&r8=&r14=&r41=&r23=&r9=&r34=&r19=&r27=&r4=&r21=&r12=&gestation=2&nbSaillie=2&hasCompanion=2&chevalNom=&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&pack-samurai-dragon=2&pack-knight=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&trail-riding-diary=2&haunted-trail-riding-diary=2&greek-trail-riding-diary=2&winter-trail-riding-diary=2&coats-bundle-witch=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&iris-coat=2&button-braided-mane=2&tail-braid-1=2&tail-braid-2=2&clipping=2&parade-apple=2&alexandre-dumas-inkwell=2&arthur-conan-doyle-inkwell=2&heracles-horseshoes=2&sisyphus-boulder=2&selle=&bride=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=2&search=1&noFilter=1&advanced=1";
                }
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
                        if (string.IsNullOrEmpty(Id))
                        {
                            postData = $"go=1&startingPage={i}&filter=all&sort={Settings.Sort}";
                        } else
                        {
                            postData = $"go=1&id={Id}&startingPage={i}&filter=all&sort={Settings.Sort}";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Id))
                        {
                            postData = $"go=1&startingPage={i}&filter=all&sort={Settings.Sort}&chevalType=&chevalEspece=any-all&unicorn=2&pegasus=2&race-cheval=&race-poney=&race-ane=&race-cheval-trait=&race-all=&race-cheval-pegase=&race-poney-pegase=&race-cheval-licorne=&race-poney-licorne=&race-cheval-licorne-ailee=&race-poney-licorne-ailee=&race-cheval-trait-pegase=&race-cheval-trait-licorne=&race-cheval-trait-licorne-ailee=&race-ane-pegase=&race-ane-licorne=&race-ane-licorne-ailee=&chevalTypeRace=&aneRaceId=51&ageComparaison=g&age=0&uniteAge=ans&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r6=&r13=&r1=&r28=&r47=&r43=&r45=&r42=&r35=&r10=&r26=&r39=&r44=&r11=&r7=&r2=&r49=&r5=&r32=&r15=&r31=&r30=&r29=&r40=&r25=&r16=&r46=&r17=&r50=&r22=&r52=&r38=&r24=&r3=&r33=&r51=&r48=&r8=&r14=&r41=&r23=&r9=&r34=&r19=&r27=&r4=&r21=&r12=&gestation=2&nbSaillie=2&hasCompanion=2&chevalNom=&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&pack-samurai-dragon=2&pack-knight=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&trail-riding-diary=2&haunted-trail-riding-diary=2&greek-trail-riding-diary=2&winter-trail-riding-diary=2&coats-bundle-witch=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&iris-coat=2&button-braided-mane=2&tail-braid-1=2&tail-braid-2=2&clipping=2&parade-apple=2&alexandre-dumas-inkwell=2&arthur-conan-doyle-inkwell=2&heracles-horseshoes=2&sisyphus-boulder=2&selle=&bride=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=2&search=1&noFilter=1&advanced=1";
                        } else
                        {
                            postData = $"go=1&id={Id}&startingPage={i}&filter=all&sort={Settings.Sort}&chevalType=&chevalEspece=any-all&unicorn=2&pegasus=2&race-cheval=&race-poney=&race-ane=&race-cheval-trait=&race-all=&race-cheval-pegase=&race-poney-pegase=&race-cheval-licorne=&race-poney-licorne=&race-cheval-licorne-ailee=&race-poney-licorne-ailee=&race-cheval-trait-pegase=&race-cheval-trait-licorne=&race-cheval-trait-licorne-ailee=&race-ane-pegase=&race-ane-licorne=&race-ane-licorne-ailee=&chevalTypeRace=&aneRaceId=51&ageComparaison=g&age=0&uniteAge=ans&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r6=&r13=&r1=&r28=&r47=&r43=&r45=&r42=&r35=&r10=&r26=&r39=&r44=&r11=&r7=&r2=&r49=&r5=&r32=&r15=&r31=&r30=&r29=&r40=&r25=&r16=&r46=&r17=&r50=&r22=&r52=&r38=&r24=&r3=&r33=&r51=&r48=&r8=&r14=&r41=&r23=&r9=&r34=&r19=&r27=&r4=&r21=&r12=&gestation=2&nbSaillie=2&hasCompanion=2&chevalNom=&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&pack-samurai-dragon=2&pack-knight=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&trail-riding-diary=2&haunted-trail-riding-diary=2&greek-trail-riding-diary=2&winter-trail-riding-diary=2&coats-bundle-witch=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&iris-coat=2&button-braided-mane=2&tail-braid-1=2&tail-braid-2=2&clipping=2&parade-apple=2&alexandre-dumas-inkwell=2&arthur-conan-doyle-inkwell=2&heracles-horseshoes=2&sisyphus-boulder=2&selle=&bride=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=2&search=1&noFilter=1&advanced=1";
                        }
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

        public async Task Run(GlobalSettings globalSettings, CancellationToken ct)
        {
            Settings = globalSettings;
            if (Acc.Settings.BuyHay != "")
            {
                buyHayInt = Convert.ToInt32(Acc.Settings.BuyHay) * 2 + 100;
            }
            if (Acc.Settings.BuyOat != "")
            {
                buyOatInt = Convert.ToInt32(Acc.Settings.BuyOat) * 2 + 100;
            }
            await LoadHorses(Acc.Settings).ConfigureAwait(false);
            var tasks = new List<Task>();
            int count = Horses.Count;
            for (int i = 0; i < Acc.Settings.SkipIndex - 1; i++)
            {
                Horses.Dequeue();
            }
            using (var semaphore = new SemaphoreSlim(5))
            {
                for (int i = 0; i < count; i++)
                {
                    Acc.ProgressHorse = (i + Acc.Settings.SkipIndex).ToString();
                    Acc.Progress = Math.Round(Convert.ToDouble(i + Acc.Settings.SkipIndex) * 100D / Convert.ToDouble(count), 2).ToString() + "%";
                    Acc.Settings.SkipIndex = 0;
                    if (Settings.ParallelHorse)
                    {
                        await semaphore.WaitAsync(ct).ConfigureAwait(false);
                        tasks.Clear();
                        tasks.Add(HorseRun(Horses.Dequeue(), semaphore));
                    }
                    else
                    {
                        await HorseRun(Horses.Dequeue(), semaphore).ConfigureAwait(false);
                    }
                }
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            }
            Acc.Progress = "100%";
            if (lowHealthCount > 50)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MainViewModel.GetInstance().Notifications.Add(Acc.Login + Properties.Resources.FarmNotificationWarnining);
                });
            }
            tasks.Clear();
            if (Acc.Settings.GoBabies)
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
                            tasks.Add(HorseRun(Babies.Dequeue(), semaphore));
                        }
                        else
                        {
                            await HorseRun(Babies.Dequeue(), semaphore).ConfigureAwait(false);
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
        public async Task HorseRun(Horse horse, SemaphoreSlim semaphore)
        {
            IHtmlDocument document = await horse.GetDoc().ConfigureAwait(false);
            if (document.QuerySelector("#tab-gift-title") == null && document.GetElementsByClassName("h2").Length == 0 && document.QuerySelector("#poulain-1") == null)
            {
                horse.LoadInfo(document);
            }
            if (await horse.CheckHorse(document, Acc.Settings).ConfigureAwait(false))
            {
                if (horse.Health > Convert.ToInt32(Acc.Settings.HealthEdge))
                {
                    if (horse.Health <= 30)
                    {
                        lowHealthCount++;
                    }
                    if (horse.Age >= 6 && document.QuerySelector("#cheval-inscription") != null)
                    {
                        if (Acc.Settings.ReserveID != "" || Acc.Settings.SelfReserve)
                        {
                            await horse.CentreReserve(Acc.Settings).ConfigureAwait(false);
                        }
                        else
                        {
                            await horse.Centre("0", Acc.Settings).ConfigureAwait(false);
                        }
                        document = await horse.GetDoc().ConfigureAwait(false);
                    }
                    await Pause().ConfigureAwait(false);
                    if (horse.Sex == HorseSex.Female && document.QuerySelector("#lienVeterinaire") != null)
                    {
                        string html = await horse.Birth(Acc.Settings).ConfigureAwait(false);
                        if (Acc.Settings.GoBabies)
                        {
                            var scripts = Parser.ParseDocument(html).QuerySelectorAll(".content__middle script");
                            string script = "";
                            foreach (var item in scripts)
                            {
                                if (item.InnerHtml.Contains("var chevalId"))
                                {
                                    script = item.InnerHtml;
                                    break;
                                }
                            }
                            string id = System.Text.RegularExpressions.Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
                            Babies.Enqueue(new Horse(id, "", Acc));
                        }
                        document = await horse.GetDoc().ConfigureAwait(false);
                        horse.LoadInfo(document);
                    }
                    await Pause().ConfigureAwait(false);
                    await horse.Action(document, "form-do-groom", "#boutonPanser", "doGroom").ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    if (Acc.Settings.Mission && horse.Age >= 24 && horse.Energy - 30 > 20 && document.QuerySelector("#mission-tab-0") != null)
                    {
                        if ((Acc.Settings.MissionOld && horse.Age >= 336) || horse.Age < 336)
                        {
                            if (!document.QuerySelectorAll("#mission-tab-0 a")[0].ClassName.Contains("action-disabled"))
                            {
                                await horse.Mission().ConfigureAwait(false);
                            }
                        }
                    }
                    await Pause().ConfigureAwait(false);
                    if (horse.Age >= 30 && horse.Energy - 25 > 20)
                    {
                        if (horse.Sex == HorseSex.Male && Acc.Settings.HorsingMale && document.QuerySelector("#reproduction-tab-0") != null && !document.QuerySelectorAll("#reproduction-tab-0 a").Last().ClassName.Contains("action-disable"))
                        {
                            await horse.HorsingMale(document, Acc.Settings).ConfigureAwait(false);
                        }
                        else if (horse.Sex == HorseSex.Female && Acc.Settings.HorsingFemale)
                        {
                            await horse.HorsingFemale(document, Acc.Settings).ConfigureAwait(false);
                        }
                    }
                    await Pause().ConfigureAwait(false);
                    if (buyHayInt > 0)
                    {
                        await CheckFood(Acc.Hay.Amount, Acc.Settings.BuyHay, buyHayInt, Acc.Hay).ConfigureAwait(false);
                    }
                    if (buyOatInt > 0)
                    {
                        await CheckFood(Acc.Oat.Amount, Acc.Settings.BuyOat, buyOatInt, Acc.Oat).ConfigureAwait(false);
                    }
                    await Pause().ConfigureAwait(false);
                    document = await horse.GetDoc().ConfigureAwait(false);
                    await horse.Feeding(document).ConfigureAwait(false);
                    await Pause().ConfigureAwait(false);
                    document = await horse.GetDoc().ConfigureAwait(false);
                    if (Acc.Settings.Stroke)
                    {
                        await horse.Action(document, "form-do-stroke", "#boutonCaresser", "doStroke").ConfigureAwait(false);
                    }
                    await horse.Action(document, "form-do-night", "#boutonCoucher", "doNight").ConfigureAwait(false);
                    #pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
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

        private async Task CheckFood(int amount, string quantity, int buyInt, Product productToBuy)
        {
            if (quantity != "" && amount < 100)
            {
                if (Acc.Equ < buyInt)
                {
                    if (Acc.Settings.MainProductToSell != ProductType.None && Acc.MainProductToSell.SellPrice * Acc.MainProductToSell.Amount > buyInt)
                    {
                        await Acc.Sell(Acc.MainProductToSell, quantity).ConfigureAwait(false);
                    }
                    else if (Acc.Settings.SubProductToSell != ProductType.None && Acc.SubProductToSell.SellPrice * Acc.SubProductToSell.Amount > buyInt)
                    {
                        await Acc.Sell(Acc.SubProductToSell, quantity).ConfigureAwait(false);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            if (Settings.MoneyNotification)
                            {
                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    MainViewModel.GetInstance().Notifications.Add($"Аккаунт: {Acc.Login} закончились деньги и/или ресурсы");
                                });
                            } else
                            {
                                if (!Acc.IsEquMessageShown)
                                {
                                    Acc.IsEquMessageShown = true;
                                    MessageBox.Show(Application.Current.MainWindow, $"На аккаунте {Acc.Login} закончилось экю и нет ресурсов для пополнения! Пополните запасы и нажмите ОК", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                        }));
                        while (Acc.Equ < buyInt)
                        {
                            Acc.Progress = "ожидание ресурсов";
                            if (Acc.Settings.MainProductToSell != ProductType.None && Acc.MainProductToSell.SellPrice * Acc.MainProductToSell.Amount > buyInt)
                            {
                                await Acc.Sell(Acc.MainProductToSell, quantity).ConfigureAwait(false);
                            }
                            else if (Acc.Settings.SubProductToSell != ProductType.None && Acc.SubProductToSell.SellPrice * Acc.SubProductToSell.Amount > buyInt)
                            {
                                await Acc.Sell(Acc.SubProductToSell, quantity).ConfigureAwait(false);
                            }
                            await Acc.LoadProducts().ConfigureAwait(false);
                        }
                        Acc.IsEquMessageShown = false;
                    }
                }
                await Acc.Buy(productToBuy, quantity).ConfigureAwait(false);
                #pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
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
