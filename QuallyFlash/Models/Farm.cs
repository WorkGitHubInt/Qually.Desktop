using AngleSharp.Dom.Html;
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
            var document = Parser.ParseDocument(await Acc.Client.PostAsync("/elevage/chevaux/searchHorse", $"go=1&id={Id}&chevalType=&race-ane=49&race-cheval=&race-poney=&race-cheval-trait=&race-all=&race-pegase=&race-licorne=&race-licorne-ailee=&race-cheval-trait-aile=&chevalTypeRace=&aneRaceId=49&ageComparaison=g&age=0&uniteAge=ans&chevalNom=&pierre-philosophale=2&sablier-chronos=2&bras-morphee=2&pommeOr=2&pommeOrDisparue=2&rayonHelios=2&lyre-apollon=2&5th-element=2&fragment=2&jouvence=2&pack-poseidon=2&genetiqueComparaison=g&genetique=0&excellenceComparaison=g&excellence=0&blupComparaison=g&blup=-100&purete=2&sexe=&rall=&r58=&r38=&r43=&r31=&r60=&r30=&r41=&r32=&r61=&r45=&r33=&r71=&r53=&r63=&r72=&r57=&r40=&r46=&r50=&r55=&r56=&r39=&r49=&r36=&r66=&r64=&r59=&r51=&r42=&r73=&r65=&r34=&r70=&r35=&r48=&r52=&r44=&r37=&r54=&r67=&r62=&gestation=2&nbSaillieComparaison=g&nbSaillie=0&classique=2&western=2&competencesComparaison=g&competences=0&enduranceComparaison=g&endurance=0&vitesseComparaison=g&vitesse=0&dressageComparaison=g&dressage=0&galopComparaison=g&galop=0&trotComparaison=g&trot=0&sautComparaison=g&saut=0&pack-nyx=2&caresse-philotes=2&don-hestia=2&citrouille-ensorcelee=2&sceau-apocalypse=2&chapeau-magique=2&double-face=2&livre-monstres=2&catrina-brooch=2&esprit-nomade=2&diamond-apple=2&pomme-vintage=2&bride=&selle=&tapis=&bonnet=&bande=&centreEquestre=2&travaille=2&couche=2&vente=0&search=1&noFilter=1&advanced=1").ConfigureAwait(false));
            string count = System.Text.RegularExpressions.Regex.Match(document.QuerySelectorAll("script").Last().InnerHtml.Replace("\\", ""), "<span style=\"display:inline\" class=\"count\">(.*?)</span>").Groups[1].Value.Trim().Substring(1);
            Count = count.Substring(0, count.Length - 1);
        }

        public async Task LoadHorses(string sort)
        {
            Horses.Clear();
            FilteredHorses.Clear();
            string postData = $"go=1&id={Id}&filter=all&sort={sort}";
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
                    postData = $"go=1&id={Id}&startingPage={i.ToString()}&filter=all&sort={sort}";
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
