using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountHolder
{
    class Trash
    {
        //public async Task Stage12()
        //{
        //    Acc.Progress = "Этап 12: ищу Horse1";
        //    string PostData = "go=1&id=all-horses&filter=all&sort=nom";
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", PostData));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    int age = 0;
        //    while (age < 6)
        //    {
        //        document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //        string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
        //        age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
        //        Acc.Progress = "Этап 12: возраст " + age;
        //        if (age < 6)
        //        {
        //            await Horse.Suckle(horseId, Acc.Client);
        //            await Horse.Groom(horseId, Acc.Client);
        //            await Horse.Sleep(horseId, Acc.Client);
        //            await Horse.Age(horseId, Acc.Client);
        //        }
        //    }
        //}

        //public async Task Stage14()
        //{
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    Acc.Progress = "Этап 14: корм";
        //    await Horse.FeedSmall(horseId, Acc.Client);
        //    Acc.Progress = "Этап 14: чистка";
        //    await Horse.Groom(horseId, Acc.Client);
        //    Acc.Progress = "Этап 14: сон";
        //    await Horse.Sleep(horseId, Acc.Client);
        //    Acc.Progress = "Этап 14: рост";
        //    await Horse.Age(horseId, Acc.Client);
        //}

        //public async Task Stage15()
        //{
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    Acc.Progress = "Этап 15: шаг 1";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 15: шаг 2";
        //    await Horse.Play(horseId, Acc.Client);
        //    Acc.Progress = "Этап 15: шаг 3";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 15: шаг 4";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 15: шаг 5";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId), 27, 40);
        //}

        //public async Task Stage16()
        //{
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    int age = 0;
        //    while (age < 18)
        //    {
        //        document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //        string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
        //        age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
        //        Acc.Progress = "Этап 16: возраст " + age;
        //        if (age < 18)
        //        {
        //            await Horse.FeedSmall(horseId, Acc.Client);
        //            await Horse.Groom(horseId, Acc.Client);
        //            await Horse.Sleep(horseId, Acc.Client);
        //            await Horse.Age(horseId, Acc.Client);
        //        }
        //    }
        //}

        //public async Task Stage17(string apple)
        //{
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    Acc.Progress = "Этап 17: шаг 1";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 17: шаг 2";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 17: шаг 3";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    string answer;
        //    do
        //    {
        //        answer = await Acc.Client.PostAsync($"/elevage/chevaux/bonus", $"id={horseId}&show=special");
        //    } while (answer == "0" || !answer.Contains("\"hasContent\":true"));
        //    Acc.Progress = "Этап 17: шаг 4";
        //    await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or&cheval=" + horseId);
        //    await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or-tutoriel");
        //    Acc.Progress = "Этап 17: шаг 5";
        //    await Valider(await Acc.Client.GetAsync($"/marche/noir/object?qName=pomme-or-tutoriel"));
        //    Acc.Progress = "Этап 17: шаг 6";
        //    await Acc.Client.PostAsync($"/marche/noir/object?qName=pomme-or-tutoriel", $"cheval={horseId}&process=1&mode=inventaire&creation={apple}"); //Навешивание яблока
        //    Acc.Progress = "Этап 17: шаг 7";
        //    await Acc.Client.GetAsync($"/elevage/chevaux/animation?cheval=" + horseId);
        //    Acc.Progress = "Этап 17: шаг 8";
        //    await Acc.Client.PostAsync($"/elevage/chevaux/animation?cheval=" + horseId, "valider=1&animation=0&animationCouleur=");
        //    Acc.Progress = "Этап 17: шаг 9";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 17: шаг 10";
        //    await Acc.Client.GetAsync($"/member/pass/");
        //    Acc.Progress = "Этап 17: шаг 11";
        //    await Valider(await Acc.Client.GetAsync($"/member/pass/"));
        //}

        //public async Task Stage18()
        //{
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    int age = 0;
        //    while (age < 36)
        //    {
        //        document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //        string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
        //        age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
        //        Acc.Progress = "Этап 18: возраст " + age;
        //        if (age < 36)
        //        {
        //            await Horse.Feed(horseId, Acc.Client);
        //            await Horse.Groom(horseId, Acc.Client);
        //            await Horse.Sleep(horseId, Acc.Client);
        //            await Horse.Age(horseId, Acc.Client);
        //        }
        //    }
        //}

        //public async Task Stage19()
        //{
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=nom"));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    Acc.Progress = "Этап 19: шаг 1";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 19: шаг 2";
        //    await Acc.Client.PostAsync($"/elevage/chevaux/doSpecialise", $"id={horseId}&specialisation=western");
        //    Acc.Progress = "Этап 19: шаг 3";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 19: шаг 4";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 19: шаг 5";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 19: шаг 6";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    Acc.Progress = "Этап 19: шаг 7";
        //    await Valider(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //    await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId);
        //    Acc.Progress = "Этап 19 перелогин";
        //    await Acc.Client.PostAsync($"/site/doLogOut", $"sid={Acc.Client.SID}");
        //    await Acc.LogOut();
        //    await Acc.LogIn();
        //    await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId);
        //}

        //public async Task Stage20()
        //{
        //    var document = Parser.ParseDocument(await Acc.Client.PostAsync($"/elevage/chevaux/searchHorse", "go=1&id=all-horses&filter=all&sort=age"));
        //    horseId = document.GetElementsByClassName("horsename")[0].GetAttribute("href").Split('=')[1];
        //    int age = 0;
        //    while (age < 18)
        //    {
        //        document = Parser.ParseDocument(await Acc.Client.GetAsync($"/elevage/chevaux/cheval?id=" + horseId));
        //        string script = document.GetElementById("page-contents").GetElementsByTagName("script")[0].InnerHtml;
        //        age = Convert.ToInt32(Regex.Match(script, "var chevalAge = (.*?);").Groups[1].Value);
        //        Acc.Progress = "Этап 20: возраст " + age;
        //        if (age < 18)
        //        {
        //            if (age >= 6)
        //            {
        //                if (document.QuerySelector("#cheval-inscription") != null)
        //                {
        //                    string postData = $"cheval={horseId}&elevage=&cheval={horseId}&competence=0&tri=tarif2&sens=DESC&tarif=&leconsPrix=&foret=2&montagne=2&plage=2&classique=2&western=2&fourrage=2&avoine=2&carotte=2&mash=2&hasSelles=2&hasBrides=2&hasTapis=2&hasBonnets=2&hasBandes=2&abreuvoir=2&douche=2&centre=&centreBox=0&directeur=&prestige=&bonus=&boxType=&boxLitiere=&prePrestige=&prodSelles=&prodBrides=&prodTapis=&prodBonnets=&prodBandes=";
        //                    string html = await Acc.Client.PostAsync($"/elevage/chevaux/centreSelection", postData);
        //                    string Response;
        //                    do
        //                    {
        //                        string str = Regex.Match(html, "{'params': 'id=" + horseId + "&centre=(.*?)&duree=3&elevage=&hash=(.*?)'}").Groups[0].Value;
        //                        string centreId = Regex.Match(str, "&centre=(.*?)&duree=3").Groups[0].Value;
        //                        string[] centreId1 = centreId.Substring(7).Split('&');
        //                        string id1 = centreId1[0].Substring(1);
        //                        string hash = Regex.Match(str, "&duree=3&elevage=&hash=(.*?)'}").Groups[1].Value;
        //                        postData = "id=" + horseId + "&centre=" + id1 + "&duree=3&elevage=&hash=" + hash;
        //                        Response = await Acc.Client.PostAsync($"/elevage/chevaux/doCentreInscription", postData);
        //                    } while (Response == "0" || Response.Contains("centreMaximum") || Response.Contains("�"));
        //                }
        //            }
        //            if (age < 6)
        //            {
        //                await Horse.Suckle(horseId, Acc.Client);
        //            }
        //            else if (age < 18)
        //            {
        //                await Horse.FeedSmall(horseId, Acc.Client);
        //            }
        //            else
        //            {
        //                await Horse.Feed(horseId, Acc.Client);
        //            }
        //            await Horse.Groom(horseId, Acc.Client);
        //            await Horse.Sleep(horseId, Acc.Client);
        //            await Horse.Age(horseId, Acc.Client);
        //        }
        //    }
        //}
    }
}
