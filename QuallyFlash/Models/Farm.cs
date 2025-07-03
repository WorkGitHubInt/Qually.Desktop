using AngleSharp.Html.Dom;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace QuallyFlash
{
    public class Farm : BaseModel
    {
        public string Name { get; set; }
        public string Count { get; set; }
        public string Id { get; private set; }
        public Account Acc { get; private set; }
        public ObservableCollection<Horse> Horses { get; set; } = new ObservableCollection<Horse>();
        public ObservableCollection<Horse> FilteredHorses { get; set; } = new ObservableCollection<Horse>();

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
            } else
            {
                postData = $"go=1&id={Id}&chevalType=&chevalEspece=any-all&unicorn=2&pegasus=2&race-cheval=&race-poney=&race-ane=&race-cheval-trait=&race-all=&race-cheval-pegase=&race-poney-pegase=&race-cheval-licorne=&race-poney-licorne=&race-cheval-licorne-ailee=&race-poney-licorne-ailee=&race-cheval-trait-pegase=&race-cheval-trait-licorne=&race-cheval-trait-licorne-ailee=&race-ane-pegase=&race-ane-licorne=&race-ane-licorne-ailee=&chevalTypeRace=&aneRaceId=51&ageComparaison=g&age=0&uniteAge=ans&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r6=&r13=&r1=&r28=&r47=&r43=&r45=&r42=&r35=&r10=&r26=&r39=&r44=&r11=&r7=&r2=&r49=&r5=&r32=&r15=&r31=&r30=&r29=&r40=&r25=&r16=&r46=&r17=&r50=&r22=&r52=&r38=&r24=&r3=&r33=&r51=&r48=&r8=&r14=&r41=&r23=&r9=&r34=&r19=&r27=&r4=&r21=&r12=&gestation=2&nbSaillie=2&hasCompanion=2&chevalNom=&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&pack-samurai-dragon=2&pack-knight=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&trail-riding-diary=2&haunted-trail-riding-diary=2&greek-trail-riding-diary=2&winter-trail-riding-diary=2&coats-bundle-witch=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&iris-coat=2&button-braided-mane=2&tail-braid-1=2&tail-braid-2=2&clipping=2&parade-apple=2&alexandre-dumas-inkwell=2&arthur-conan-doyle-inkwell=2&heracles-horseshoes=2&sisyphus-boulder=2&selle=&bride=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=2&search=1&noFilter=1&advanced=0";
            }
            var document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", postData).ConfigureAwait(false));
            string count = System.Text.RegularExpressions.Regex.Match(document.QuerySelectorAll("script").Last().InnerHtml.Replace("\\", ""), "<span style=\"display:inline\" class=\"count\">(.*?)</span>").Groups[1].Value.Trim().Substring(1);
            Count = count.Substring(0, count.Length - 1);
        }

        public async Task LoadHorses(string sort)
        {
            Horses.Clear();
            FilteredHorses.Clear();
            string postData;
            if (string.IsNullOrEmpty(Id))
            {
                postData = $"go=1&sort={sort}&filter=all";
            } else
            {
                postData = $"go=1&id={Id}&sort={sort}&filter=all";
            }
            var document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", postData));
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
                    if (string.IsNullOrEmpty(Id))
                    {
                        postData = $"go=1&startingPage={i}&sort={sort}&filter=all";
                    } else
                    {
                        postData = $"go=1&id={Id}&startingPage={i}&sort={sort}&filter=all";
                    }
                    document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", postData));
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
                Horses.Add(new Horse(horseId, name, Acc));
                FilteredHorses.Add(new Horse(horseId, name, Acc));
            }
        }
    }
}
