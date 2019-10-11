using AngleSharp.Dom;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountHolder
{
    public class Guide
    {
        public Account Acc { get; set; }
        private string geyaId;
        private string horseId;

        public async Task Stage1()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 1: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/jeu/"));
            Acc.Progress = "Этап 1: шаг 2";
            await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId);
            Acc.Progress = "Этап 1: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage2()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 2: шаг 1";
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
            Acc.Progress = "Этап 2: шаг 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = "Этап 2: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = "Этап 2: шаг 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 2: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage3()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 3: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 3: шаг 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = "Этап 3: шаг 3";
            document = Parser.ParseDocument(await Acc.Client.GetAsync($"/marche/boutique"));
            Acc.Progress = "Этап 3: шаг 4";
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
            Acc.Progress = "Этап 3: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/marche/boutique"));
            Acc.Progress = "Этап 3: шаг 6";
            await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId);
            string Answer = "";
            Acc.Progress = "Этап 3: шаг 7";
            do
            {
                await Acc.Client.PostAsync($"/elevage/chevaux/doHarness", $"id={geyaId}&tapis=&tapisCourant=tapis-classique-1x&tapisCouleur=0&tapisCouleurCourant=0&selle=selle-classique-1x&selleCourant=&bride=&brideCourant=bride-classique-1x&bande=&bandeCourant=&bandeCouleur=0&bandeCouleurCourant=0&bonnet=&bonnetCourant=&bonnetCouleur=0&bonnetCouleurCourant=0");
            } while (Answer == "0");
            Acc.Progress = "Этап 3: шаг 8";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage4()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 4: шаг 1";
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
            Acc.Progress = "Этап 4: шаг 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 4: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage5()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 5: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = "Этап 5: шаг 2";
            await Acc.Client.GetAsync($"/marche/noir/");
            Acc.Progress = "Этап 5: шаг 3";
            await Acc.Client.GetAsync($"/marche/noir/object?qName=baton-fertilite");
            Acc.Progress = "Этап 5: шаг 4";
            string answer;
            do
            {
                answer = await Acc.Client.PostAsync($"/marche/noir/object?qName=baton-fertilite", $"cheval={geyaId}&process=1&mode=pass");
            } while (answer == "0");
            Acc.Progress = "Этап 5: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage6()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 6: шаг 1";
            await Acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument=" + geyaId);
            Acc.Progress = "Этап 6: шаг 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument=" + geyaId));
            Acc.Progress = "Этап 6: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/rechercherMale?jument=" + geyaId));
            Acc.Progress = "Этап 6: шаг 4";
            await Acc.Client.GetAsync($"/elevage/chevaux/saillie?offre=1&jument={geyaId}&search=jument%3D43376497");
            Acc.Progress = "Этап 6: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/saillie?offre=1&jument={geyaId}&search=jument%3D43376497"));
            Acc.Progress = "Этап 6: шаг 6";
            await Acc.Client.PostAsync($"/elevage/chevaux/doReproduction", $"id={geyaId}&offer=1&action=accept&search=jument={geyaId}#resultatsRecherche");
            Acc.Progress = "Этап 6: шаг 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage7()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 7: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 2";
            await Horse.Groom(geyaId, Acc.Client);
            Acc.Progress = "Этап 7: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 8";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 9";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 10";
            await Horse.Feed(geyaId, Acc.Client);
            Acc.Progress = "Этап 7: шаг 11";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 12";
            await Horse.Sleep(geyaId, Acc.Client);
            Acc.Progress = "Этап 7: шаг 13";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 7: шаг 14";
            await Horse.Age(geyaId, Acc.Client);
            Acc.Progress = "Этап 7: шаг 15";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage8()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 8: шаг 1";
            await Horse.Groom(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 2";
            await Horse.Drink(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 8: шаг 4";
            await Horse.Walk(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 8: шаг 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 8: шаг 7";
            await Horse.Stroke(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 8";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId), 27, 40);
            Acc.Progress = "Этап 8: шаг 9";
            await Horse.Carrot(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 10";
            await Horse.Feed(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 11";
            await Horse.Sleep(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 12";
            await Horse.Age(geyaId, Acc.Client);
            Acc.Progress = "Этап 8: шаг 13";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage9()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 9: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
            Acc.Progress = "Этап 9: шаг 2";
            await Horse.Groom(geyaId, Acc.Client);
            Acc.Progress = "Этап 9: шаг 3";
            await Horse.Feed(geyaId, Acc.Client);
            Acc.Progress = "Этап 9: шаг 3";
            await Horse.Sleep(geyaId, Acc.Client);
            Acc.Progress = "Этап 9: шаг 4";
            await Horse.Age(geyaId, Acc.Client);
            Acc.Progress = "Этап 9: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + geyaId));
        }

        public async Task Stage10()
        {
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/"));
            string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
            geyaId = Regex.Match(script, "var chevalId = (.*?);").Groups[1].Value;
            Acc.Progress = "Этап 10: шаг 1";
            await Acc.Client.GetAsync($"/elevage/chevaux/mettreBas?jument=" + geyaId);
            Acc.Progress = "Этап 10: шаг 2";
            await Acc.Client.PostAsync($"/elevage/chevaux/choisirNoms?jument=" + geyaId, "valider=ok&poulain-1=Horse2&poulain-2=Horse1");
            Acc.Progress = "Этап 10: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/?elevage=all-horses"));
        }

        public async Task Stage11()
        {
            Acc.Progress = "Этап 11: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/jeu/"));
            Acc.Progress = "Этап 11: шаг 2";
            Acc.Progress = "Этап 11: шаг 3";
            var document = Parser.ParseDocument(await Acc.Client.GetAsync($"/member/privateMessage/"));
            string script = document.GetElementsByClassName("message-link")[0].GetElementsByTagName("script")[0].InnerHtml;
            string post = Regex.Match(script, "{'params': '(.*?)'}").Groups[1].Value;
            await Acc.Client.PostAsync($"/member/privateMessage/view", post);
            Acc.Progress = "Этап 11: шаг 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/?elevage=all-horses"));
            Acc.Progress = "Этап 11: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/?elevage=all-horses"));
        }

        public async Task Stage12()
        {
            Acc.Progress = "Этап 12: ищу Horse1";
            string PostData = "go=1&id=all-horses&filter=all&sort=nom";
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", PostData));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            int age = 0;
            while (age < 6)
            {
                document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
                string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
                age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
                Acc.Progress = "Этап 12: возраст " + age;
                if (age < 6)
                {
                    await Horse.Suckle(horseId, Acc.Client);
                    await Horse.Groom(horseId, Acc.Client);
                    await Horse.Sleep(horseId, Acc.Client);
                    await Horse.Age(horseId, Acc.Client);
                }
            }
        }

        public async Task Stage13()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = "Этап 13: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 13: шаг 2";
            await Acc.Client.GetAsync($"/elevage/chevaux/centreInscription?id=" + horseId);
            Acc.Progress = "Этап 13: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/centreInscription?id=" + horseId));
            Acc.Progress = "Этап 13: шаг 4";
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
            Acc.Progress = "Этап 13: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        }

        public async Task Stage14()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = "Этап 14: корм";
            await Horse.FeedSmall(horseId, Acc.Client);
            Acc.Progress = "Этап 14: чистка";
            await Horse.Groom(horseId, Acc.Client);
            Acc.Progress = "Этап 14: сон";
            await Horse.Sleep(horseId, Acc.Client);
            Acc.Progress = "Этап 14: рост";
            await Horse.Age(horseId, Acc.Client);
        }

        public async Task Stage15()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = "Этап 15: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 15: шаг 2";
            await Horse.Play(horseId, Acc.Client);
            Acc.Progress = "Этап 15: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 15: шаг 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 15: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId), 27, 40);
        }

        public async Task Stage16()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            int age = 0;
            while (age < 18)
            {
                document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
                string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
                age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
                Acc.Progress = "Этап 16: возраст " + age;
                if (age < 18)
                {
                    await Horse.FeedSmall(horseId, Acc.Client);
                    await Horse.Groom(horseId, Acc.Client);
                    await Horse.Sleep(horseId, Acc.Client);
                    await Horse.Age(horseId, Acc.Client);
                }
            }
        }

        public async Task Stage17(string apple)
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = "Этап 17: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 17: шаг 2";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 17: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            string answer;
            do
            {
                answer = await Acc.Client.PostAsync($"/elevage/chevaux/bonus", $"id={horseId}&show=special");
            } while (answer == "0" || !answer.Contains("\"hasContent\":true"));
            Acc.Progress = "Этап 17: шаг 4";
            await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or&cheval=" + horseId);
            await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or-tutoriel");
            Acc.Progress = "Этап 17: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or-tutoriel"));
            Acc.Progress = "Этап 17: шаг 6";
            await Acc.Client.PostAsync($"/marche/noir/object?qName=pomme-or-tutoriel", $"cheval={horseId}&process=1&mode=inventaire&creation={apple}"); //Навешивание яблока
            Acc.Progress = "Этап 17: шаг 7";
            await Acc.Client.GetAsync($"/elevage/chevaux/animation?cheval=" + horseId);
            Acc.Progress = "Этап 17: шаг 8";
            await Acc.Client.PostAsync($"/elevage/chevaux/animation?cheval=" + horseId, "valider=1&animation=0&animationCouleur=");
            Acc.Progress = "Этап 17: шаг 9";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 17: шаг 10";
            await Acc.Client.GetAsync($"/member/pass/");
            Acc.Progress = "Этап 17: шаг 11";
            await Valider(await Acc.Client.GetAsync($"/member/pass/"));
        }

        public async Task Stage18()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            int age = 0;
            while (age < 36)
            {
                document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
                string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
                age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
                Acc.Progress = "Этап 18: возраст " + age;
                if (age < 36)
                {
                    await Horse.Feed(horseId, Acc.Client);
                    await Horse.Groom(horseId, Acc.Client);
                    await Horse.Sleep(horseId, Acc.Client);
                    await Horse.Age(horseId, Acc.Client);
                }
            }
        }

        public async Task Stage19()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            Acc.Progress = "Этап 19: шаг 1";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 19: шаг 2";
            await Acc.Client.PostAsync($"/elevage/chevaux/doSpecialise", $"id={horseId}&specialisation=western");
            Acc.Progress = "Этап 19: шаг 3";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 19: шаг 4";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 19: шаг 5";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 19: шаг 6";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            Acc.Progress = "Этап 19: шаг 7";
            await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
            await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId);
            Acc.Progress = "Этап 19 перелогин";
            await Acc.Client.PostAsync($"/site/doLogOut", $"sid={Acc.Client.SID}");
            await Acc.LogOut();
            await Acc.LogIn();
            await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId);
        }

        public async Task Stage20()
        {
            var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=age"));
            horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
            int age = 0;
            while (age < 18)
            {
                document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
                string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
                age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
                Acc.Progress = "Этап 20: возраст " + age;
                if (age < 18)
                {
                    if (age >= 6)
                    {
                        if (document.QuerySelector("#cheval-inscription") != null)
                        {
                            string postData = $"cheval={horseId}&elevage=&cheval={horseId}&competence=0&tri=tarif2&sens=DESC&tarif=&leconsPrix=&foret=2&montagne=2&plage=2&classique=2&western=2&fourrage=2&avoine=2&carotte=2&mash=2&hasSelles=2&hasBrides=2&hasTapis=2&hasBonnets=2&hasBandes=2&abreuvoir=2&douche=2&centre=&centreBox=0&directeur=&prestige=&bonus=&boxType=&boxLitiere=&prePrestige=&prodSelles=&prodBrides=&prodTapis=&prodBonnets=&prodBandes=";
                            string html = await Acc.Client.PostAsync($"/elevage/chevaux/centreSelection", postData);
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
                        }
                    }
                    if (age < 6)
                    {
                        await Horse.Suckle(horseId, Acc.Client);
                    }
                    else if (age < 18)
                    {
                        await Horse.FeedSmall(horseId, Acc.Client);
                    }
                    else
                    {
                        await Horse.Feed(horseId, Acc.Client);
                    }
                    await Horse.Groom(horseId, Acc.Client);
                    await Horse.Sleep(horseId, Acc.Client);
                    await Horse.Age(horseId, Acc.Client);
                }
            }
        }

        public async Task ValiderStart(string html)
        {
            var document = Parser.ParseDocument(html);
            string script = document.GetElementById("tutoriel-wrapper").LastElementChild.InnerHtml;
            string json = Regex.Match(script, "var callbackOptions = jQuery.parseJSON((.*?));").Groups[1].Value;
            string hash = Regex.Match(json, "{\"hash\":\"(.*?)\",\"isAjax\":false,\"isExternal\":false}").Groups[1].Value;
            string gid = Regex.Match(script, "var eJoueurTutoriel = jQuery.parseJSON((.*?));").Groups[1].Value;
            geyaId = Regex.Match(gid, "{\"etape\":\"introduction-presentation\",\"cheval\":{\"id\":(.*?)}}").Groups[1].Value;
            await Acc.Client.PostAsync($"/joueur/tutoriel/doValider", "id=null&h=" + hash);
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
    }
}
