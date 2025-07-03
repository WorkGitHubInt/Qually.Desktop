using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountHolder
{
    public class Guide
    {
        public Account Acc { get; set; }
        private string geyaId;
        private string horseId;
        private string imgUrl;

        public async Task Stage1()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 1: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/jeu/"));
            Acc.Progress = $"{Properties.Resources.StageText} 1: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/jeu/"));
            Acc.Progress = $"{Properties.Resources.StageText} 1: {Properties.Resources.StepText} 3";
            await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId);
            Acc.Progress = $"{Properties.Resources.StageText} 1: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage2()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 2: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 2: {Properties.Resources.StepText} 2";
            document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            var form = document.GetElementById("training-tab-main").GetElementsByTagName("form")[0];
            string name = form.Children[1].GetAttribute("name").ToLower();
            string value = form.Children[1].GetAttribute("value").ToLower();
            string id = form.Children[2].GetAttribute("value");
            string competence = form.Children[3].GetAttribute("value");
            string answer;
            do
            {
                answer = await Acc.Client.PostAsync($"/elevage/chevaux/doTraining", $"{name}={value}&id={id}&competence={competence}");
            } while (answer == "0" || !answer.Contains("\"errors\":[]"));
            Acc.Progress = $"{Properties.Resources.StageText} 2: {Properties.Resources.StepText} 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = $"{Properties.Resources.StageText} 2: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = $"{Properties.Resources.StageText} 2: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 2: {Properties.Resources.StepText} 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 2: {Properties.Resources.StepText} 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
        }

        public async Task Stage3()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 3: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 3: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = $"{Properties.Resources.StageText} 3: {Properties.Resources.StepText} 3";
            document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            imgUrl = document.QuerySelector("#competition-body-content button:not(.disabled)").ParentElement.ParentElement.ParentElement.QuerySelector("img").GetAttribute("src");
            string postData = imgUrl.Contains("western") ? $"id={geyaId}&specialisation=western" : $"id={geyaId}&specialisation=classique";
            await Acc.Client.PostAsync($"/elevage/chevaux/doSpecialise", postData);
            Acc.Progress = $"{Properties.Resources.StageText} 3: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 3: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 3: {Properties.Resources.StepText} 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 3: {Properties.Resources.StepText} 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage4()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 3";
            document = Parser.ParseDocument(await Acc.Client.GetAsync($"/marche/boutique"));
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 4";
            string json = await Acc.Client.PostAsync($"/marche/produits", "id=equipement&mode=eleveur&visibilite=marche-eleveur");
            json = json.Substring(72);
            string html = json.Substring(0, json.Length - 20).Replace("\\", "");
            document = Parser.ParseDocument(html);
            string id = document.GetElementsByTagName("select")[0].GetAttribute("id").Substring(8);
            string answer;
            do
            {
                answer = await Acc.Client.PostAsync($"/marche/achat", "id=" + id + "&mode=eleveur&nombre=1&typeRedirection=&idElement=");
            } while (answer == "0" || !answer.Contains("\"errorsText\":\"\""));
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync($"/marche/boutique"));
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 6";
            await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId);
            string Answer = "";
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 7";
            string postData = imgUrl.Contains("western") ? $"id={geyaId}&tapis=&tapisCourant=tapis-western-1x&tapisCouleur=0&tapisCouleurCourant=0&selle=selle-western-1x&selleCourant=&bride=&brideCourant=bride-western-1x&bande=&bandeCourant=bande-1x&bandeCouleur=0&bandeCouleurCourant=0&bonnet=&bonnetCourant=bonnet-1x&bonnetCouleur=0&bonnetCouleurCourant=0" : $"id={geyaId}&tapis=&tapisCourant=tapis-classique-1x&tapisCouleur=0&tapisCouleurCourant=0&selle=selle-classique-1x&selleCourant=&bride=&brideCourant=bride-classique-1x&bande=&bandeCourant=bande-1x&bandeCouleur=0&bandeCouleurCourant=0&bonnet=&bonnetCourant=bonnet-1x&bonnetCouleur=0&bonnetCouleurCourant=0";
            do
            {
                await Acc.Client.PostAsync($"/elevage/chevaux/doHarness", postData);
            } while (Answer == "0");
            Acc.Progress = $"{Properties.Resources.StageText} 4: {Properties.Resources.StepText} 8";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
        }

        public async Task Stage5()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 5: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 5: {Properties.Resources.StepText} 2";
            document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            var a = document.GetElementById("competition-body-content").GetElementsByTagName("a");
            int i = 0;
            int num = 0;
            foreach (IElement el in a)
            {
                if (!el.ClassName.Contains("action-disabled"))
                {
                    num = i;
                }
                i++;
            }
            string href = a[num].ClassName;
            string competition = href.Substring(34);
            await Acc.Client.GetAsync(a[num].GetAttribute("url"));
            string html;
            if (Acc.Client.BaseAddress.ToString().Contains("lowadi.com"))
            {
                html = await Acc.Client.PostAsync($"/elevage/competition/liste", $"type={competition}&id={geyaId}&longueur=5");
            }
            else
            {
                html = await Acc.Client.PostAsync($"/elevage/competition/liste", $"type={competition}&id={geyaId}&course={competition}");
            }
            string key;
            if (Acc.Client.BaseAddress.ToString().Contains("lowadi.com"))
            {
                key = Regex.Match(html, $"type={competition}&id={geyaId}&competition={geyaId}&key=(.*?)&bouton=0").Groups[1].Value;
            }
            else
            {
                key = Regex.Match(html, $"type=course&id={geyaId}&competition={geyaId}&key=(.*?)&bouton=0").Groups[1].Value;
            }
            if (Acc.Client.BaseAddress.ToString().Contains("lowadi.com"))
            {
                await Acc.Client.GetAsync($"/elevage/chevaux/doCompetition?type={competition}&id={geyaId}&competition={geyaId}&key={key}&bouton=0");
            }
            else
            {
                await Acc.Client.GetAsync($"/elevage/chevaux/doCompetition?type=course&id={geyaId}&competition={geyaId}&key={key}&bouton=0");
            }
            Acc.Progress = $"{Properties.Resources.StageText} 5: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 5: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage6()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 6: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = $"{Properties.Resources.StageText} 6: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 6: {Properties.Resources.StepText} 3";
            await Acc.Client.GetAsync($"/marche/noir/");
            Acc.Progress = $"{Properties.Resources.StageText} 6: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/marche/noir/")); 
            Acc.Progress = $"{Properties.Resources.StageText} 6: {Properties.Resources.StepText} 5";
            await Acc.Client.GetAsync($"/marche/noir/object?qName=baton-fertilite");
            Acc.Progress = $"{Properties.Resources.StageText} 6: {Properties.Resources.StepText} 6";
            string answer;
            do
            {
                answer = await Acc.Client.PostAsync($"/marche/noir/object?qName=baton-fertilite", $"cheval={geyaId}&process=1&mode=pass");
            } while (answer == "0");
            Acc.Progress = $"{Properties.Resources.StageText} 6: {Properties.Resources.StepText} 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
        }

        public async Task Stage7()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 7: {Properties.Resources.StepText} 1";
            await Acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument=" + geyaId);
            Acc.Progress = $"{Properties.Resources.StageText} 7: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 7: {Properties.Resources.StepText} 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 7: {Properties.Resources.StepText} 4";
            await Acc.Client.GetAsync($"/elevage/chevaux/saillie?offre=1&jument={geyaId}&search=jument%3D43376497");
            Acc.Progress = $"{Properties.Resources.StageText} 7: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/saillie?offre=1&jument={geyaId}&search=jument%3D43376497"));
            Acc.Progress = $"{Properties.Resources.StageText} 7: {Properties.Resources.StepText} 6";
            await Acc.Client.PostAsync($"/elevage/chevaux/doReproduction", $"id={geyaId}&offer=1&action=accept&search=jument={geyaId}#resultatsRecherche");
            Acc.Progress = $"{Properties.Resources.StageText} 7: {Properties.Resources.StepText} 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
        }

        public async Task Stage8()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 3";
            await Horse.Groom(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 8";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 9";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 10";
            await Horse.Feed(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 11";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 12";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 13";
            await Horse.Sleep(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 14";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 15";
            await Horse.Age(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 8: {Properties.Resources.StepText} 16";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage9()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 1";
            await Horse.Groom(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 3";
            await Horse.Drink(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 7";
            await Horse.Walk(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 8";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 9";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 10";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 11";
            await Horse.Stroke(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 12";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 13";
            await Horse.Carrot(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 14";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 15";
            await Horse.Feed(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 16";
            await Horse.Sleep(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 17";
            await Horse.Age(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 9: {Properties.Resources.StepText} 18";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage10()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 10: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 10: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = $"{Properties.Resources.StageText} 10: {Properties.Resources.StepText} 3";
            await Horse.Groom(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 10: {Properties.Resources.StepText} 4";
            await Horse.Feed(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 10: {Properties.Resources.StepText} 5";
            await Horse.Sleep(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 10: {Properties.Resources.StepText} 6";
            await Horse.Age(geyaId, Acc.Client);
            Acc.Progress = $"{Properties.Resources.StageText} 10: {Properties.Resources.StepText} 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage11()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = getScript(document);
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = $"{Properties.Resources.StageText} 11: {Properties.Resources.StepText} 1";
            await Acc.Client.GetAsync($"/elevage/chevaux/mettreBas?jument=" + geyaId);
            Acc.Progress = $"{Properties.Resources.StageText} 11: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/choisirNoms?jument={geyaId}"));
            Acc.Progress = $"{Properties.Resources.StageText} 11: {Properties.Resources.StepText} 3";
            await Acc.Client.PostAsync($"/elevage/chevaux/choisirNoms?jument=" + geyaId, "valider=ok&poulain-1=Horse2&poulain-2=Horse1");
            Acc.Progress = $"{Properties.Resources.StageText} 11: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/?elevage=all-horses"));
        }

        public async Task Stage12()
        {
            Acc.Progress = $"{Properties.Resources.StageText} 12: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/?elevage=all-horses"));
            await Acc.Client.GetAsync($"/elevage/chevaux/?elevage=all-horses");
            Acc.Progress = $"{Properties.Resources.StageText} 12: {Properties.Resources.StepText} 2";
            await Acc.Client.GetAsync($"/marche/vente/?type=enchere&typeSave=1");
            Acc.Progress = $"{Properties.Resources.StageText} 12: {Properties.Resources.StepText} 3";
            await Valider(await Acc.Client.GetAsync($"/marche/vente/?type=enchere&typeSave=1"));
            Acc.Progress = $"{Properties.Resources.StageText} 12: {Properties.Resources.StepText} 4";
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/marche/vente/?type=enchere&typeSave=1"));
            string href = document.GetElementById("table-0").GetElementsByClassName("odd highlight")[0].GetElementsByTagName("a").Last().GetAttribute("href");
            document = Parser.ParseDocument(await Acc.Client.GetAsync(href));
            var form = document.GetElementById("venteAjouterEnchere");
            string name1 = form.Children[1].GetAttribute("name").ToLower();
            string value = form.Children[1].GetAttribute("value").ToLower();
            string session = form.Children[4].GetAttribute("value").ToLower();
            string postData = $"{name1}={value}&vente=1&session={session}&tutoriel=auction-1";
            await Acc.Client.PostAsync("/marche/vente/enchere/doEnchere", postData);
            Acc.Progress = $"{Properties.Resources.StageText} 12: {Properties.Resources.StepText} 5";
            await Valider(await Acc.Client.GetAsync("/marche/vente/tutoriel/voir?type=enchere&tutorial=auction-1"));
            Acc.Progress = $"{Properties.Resources.StageText} 12: {Properties.Resources.StepText} 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/?elevage=all-horses"));
        }

        public async Task Stage13()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = $"{Properties.Resources.StageText} 13: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 13: {Properties.Resources.StepText} 2";
            await Acc.Client.GetAsync($"/elevage/chevaux/centreInscription?id=" + horseId);
            Acc.Progress = $"{Properties.Resources.StageText} 13: {Properties.Resources.StepText} 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/centreInscription?id=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 13: {Properties.Resources.StepText} 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/centreInscription?id=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 13: {Properties.Resources.StepText} 5";
            string postData = $"cheval={horseId}&elevage=&cheval={horseId}&competence=0&tri=tarif2&sens=DESC&tarif=&leconsPrix=&foret=2&montagne=2&plage=2&classique=2&western=2&fourrage=2&avoine=2&carotte=2&mash=2&hasSelles=2&hasBrides=2&hasTapis=2&hasBonnets=2&hasBandes=2&abreuvoir=2&douche=2&centre=&centreBox=0&directeur=&prestige=&bonus=&boxType=&boxLitiere=&prePrestige=&prodSelles=&prodBrides=&prodTapis=&prodBonnets=&prodBandes=";
            string html;
            do
            {
                html = await Acc.Client.PostAsync($"/elevage/chevaux/centreSelection", postData);
            } while (html == "0" || !html.Contains("\"hasContent\":true"));
            string Response;
            do
            {
                string str = Regex.Match(html, "{'params': 'id=" + horseId + "&centre=(.*?)&duree=3&elevage=&hash=(.*?)'}").Groups[0].Value;
                string centreId = Regex.Match(str, "&centre=(.*?)&duree=3").Groups[0].Value;
                string[] centreId1 = centreId.Substring(7).Split('&');
                string id1 = centreId1[0].Substring(1);
                string hash = Regex.Match(str, "&duree=3&elevage=&hash=(.*?)'}").Groups[1].Value;
                postData = "id=" + horseId + "&centre=" + id1 + "&duree=3&elevage=&hash=" + hash;
                Response = await Acc.Client.PostAsync($"/elevage/chevaux/doCentreInscription", postData);
            } while (Response == "0" || Response.Contains("centreMaximum") || Response.Contains("�"));
            Acc.Progress = $"{Properties.Resources.StageText} 13: {Properties.Resources.StepText} 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        }

        public async Task Stage14(string apple)
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            string answer;
            do
            {
                answer = await Acc.Client.PostAsync($"/elevage/chevaux/bonus", $"id={horseId}&show=special");
            } while (answer == "0" || !answer.Contains("\"hasContent\":true"));
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 4";
            await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or&cheval=" + horseId);
            await Valider(await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or-tutoriel"));
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 5";
            await Acc.Client.PostAsync($"/marche/noir/object?qName=pomme-or-tutoriel", $"cheval={horseId}&process=1&mode=inventaire&creation={apple}"); //Навешивание яблока
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 7";
            await Acc.Client.GetAsync($"/elevage/chevaux/animation?cheval=" + horseId);
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 8";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/animation?cheval=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 9";
            await Acc.Client.PostAsync($"/elevage/chevaux/animation?cheval=" + horseId, "valider=1&animation=0&animationCouleur=");
            Acc.Progress = $"{Properties.Resources.StageText} 14: {Properties.Resources.StepText} 10";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        }

        public async Task Stage15()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = $"{Properties.Resources.StageText} 15: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = $"{Properties.Resources.StageText} 15: {Properties.Resources.StepText} 2";
            await Acc.Client.GetAsync($"/member/pass/");
            Acc.Progress = $"{Properties.Resources.StageText} 15: {Properties.Resources.StepText} 3";
            await Valider(await Acc.Client.GetAsync($"/member/pass/"));
        }

        public async Task Stage16()
        {
            Acc.Progress = $"{Properties.Resources.StageText} 16: {Properties.Resources.StepText} 1";
            await Valider(await Acc.Client.GetAsync($"/member/pass/"));
            await Acc.LogOut();
            await Acc.LogIn();
            Acc.Progress = $"{Properties.Resources.StageText} 16: {Properties.Resources.StepText} 2";
            await Acc.Client.GetAsync($"/beginner/#header-anchor");
            Acc.Progress = $"{Properties.Resources.StageText} 16: {Properties.Resources.StepText} 3";
            await Valider(await Acc.Client.GetAsync($"/beginner/#header-anchor"));
        }

        public async Task Valider(string html)
        {
            var document = Parser.ParseDocument(html);
            string script = document.GetElementById("tutoriel-wrapper").LastElementChild.InnerHtml;
            string json = Regex.Match(script, "var callbackOptions = jQuery.parseJSON((.*?));").Groups[1].Value;
            string hash = Regex.Match(json, "{\"hash\":\"(.*?)\",\"isAjax\":false,\"isExternal\":false}").Groups[1].Value;
            await Acc.Client.PostAsync($"/joueur/tutoriel/doValider", "id=null&h=" + hash);
        }

        public async Task Valider(string html, int startIndex, int length)
        {
            var document = Parser.ParseDocument(html);
            string script = document.GetElementById("tutoriel-wrapper").LastElementChild.InnerHtml;
            string json = Regex.Match(script, "var callbackOptions = (.*?);").Groups[1].Value;
            string hash = json.Substring(startIndex, length);
            await Acc.Client.PostAsync($"/joueur/tutoriel/doValider", "id=null&h=" + hash);
        }

        private string getScript(IHtmlDocument document)
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
            return script;
        }
    }
}
