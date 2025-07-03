using System;
using System.Threading.Tasks;

namespace AccountHolder
{
    public static class Horse
    {
        public static readonly Random rnd = new Random();

        public static async Task Feed(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("feeding").Children[1].GetAttribute("name").ToLower();
            string name2 = document.GetElementById("feeding").Children[2].GetAttribute("name").Substring(7).ToLower();
            string name3 = document.GetElementById("feeding").Children[3].GetAttribute("name").Substring(7).ToLower();
            string name4 = document.GetElementById("feeding").Children[4].GetAttribute("name").Substring(7).ToLower();
            string value = document.GetElementById("feeding").Children[1].GetAttribute("value").ToLower();
            string oat = document.GetElementById("oatsSlider-sliderHidden").GetAttribute("name").Substring(7).ToLower();
            string hay = document.GetElementById("haySlider-sliderHidden").GetAttribute("name").Substring(7).ToLower();
            string haytogive = document.GetElementsByClassName("section-fourrage section-fourrage-target")[0].TextContent.Trim().Replace(" ", string.Empty);
            string oatstogive = document.GetElementsByClassName("section-avoine section-avoine-target")[0].TextContent.Trim().Replace(" ", string.Empty);
            string postData = $"{name1}={value}&{name2}={id}&{name3}={rnd.Next(20, 68)}&{name4}={rnd.Next(15, 55)}&{hay}={haytogive}&{oat}={oatstogive}";
            await client.PostAsync($"/elevage/chevaux/doEat", postData);
        }

        public static async Task FeedSmall(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("feeding").Children[1].GetAttribute("name").ToLower();
            string name2 = document.GetElementById("feeding").Children[2].GetAttribute("name").Substring(7).ToLower();
            string name3 = document.GetElementById("feeding").Children[3].GetAttribute("name").Substring(7).ToLower();
            string name4 = document.GetElementById("feeding").Children[4].GetAttribute("name").Substring(7).ToLower();
            string value = document.GetElementById("feeding").Children[1].GetAttribute("value").ToLower();
            string hay = document.GetElementById("haySlider-sliderHidden").GetAttribute("name").Substring(7).ToLower();
            string haytogive = document.GetElementsByClassName("section-fourrage section-fourrage-target")[0].TextContent.Trim().Replace(" ", string.Empty);
            string postData = $"{name1}={value}&{name2}={id}&{name3}={rnd.Next(20, 68)}&{name4}={rnd.Next(15, 55)}&{hay}={haytogive}";
            await client.PostAsync($"/elevage/chevaux/doEat", postData);
        }

        public static async Task Groom(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("form-do-groom").Children[1].GetAttribute("name").ToLower();
            string name2 = document.GetElementById("form-do-groom").Children[2].GetAttribute("name").Substring(13).ToLower();
            string value = document.GetElementById("form-do-groom").Children[1].GetAttribute("value").ToLower();
            string name3 = document.GetElementById("form-do-groom").Children[3].GetAttribute("name").Substring(13).ToLower();
            string name4 = document.GetElementById("form-do-groom").Children[4].GetAttribute("name").Substring(13).ToLower();
            string postData = $"{name1}={value}&{name2}={id}&{name3}={rnd.Next(27, 60)}&{name4}={rnd.Next(21, 49)}";
            await client.PostAsync($"/elevage/chevaux/doGroom", postData);
        }

        public static async Task Sleep(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("form-do-night").Children[1].GetAttribute("name").ToLower();
            string value = document.GetElementById("form-do-night").Children[1].GetAttribute("value").ToLower();
            string name2 = document.GetElementById("form-do-night").Children[2].GetAttribute("name").Substring(13).ToLower();
            string name3 = document.GetElementById("form-do-night").Children[3].GetAttribute("name").Substring(13).ToLower();
            string name4 = document.GetElementById("form-do-night").Children[4].GetAttribute("name").Substring(13).ToLower();
            string postData = $"{name1}={value}&{name2}={id}&{name3}={rnd.Next(25, 60)}&{name4}={rnd.Next(20, 55)}";
            await client.PostAsync($"/elevage/chevaux/doNight", postData);
        }

        public static async Task Age(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("age").Children[1].GetAttribute("name").ToLower();
            string value = document.GetElementById("age").Children[1].GetAttribute("value").ToLower();
            string name2 = document.GetElementById("age").Children[2].GetAttribute("name").Substring(3).ToLower();
            string name3 = document.GetElementById("age").Children[3].GetAttribute("name").Substring(3).ToLower();
            string age = document.GetElementById("age").Children[3].GetAttribute("value");
            string name4 = document.GetElementById("age").Children[4].GetAttribute("name").Substring(3).ToLower();
            string name5 = document.GetElementById("age").Children[5].GetAttribute("name").Substring(3).ToLower();
            string postData = $"{name1}={value}&{name2}={id}&{name3}={age}&{name4}=0&{name5}=0";
            await client.PostAsync($"/elevage/chevaux/doAge", postData);
        }

        public static async Task Drink(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("form-do-drink").Children[1].GetAttribute("name").ToLower();
            string name2 = document.GetElementById("form-do-drink").Children[2].GetAttribute("name").Substring(13).ToLower();
            string value = document.GetElementById("form-do-drink").Children[1].GetAttribute("value").ToLower();
            string name3 = document.GetElementById("form-do-drink").Children[3].GetAttribute("name").Substring(13).ToLower();
            string name4 = document.GetElementById("form-do-drink").Children[4].GetAttribute("name").Substring(13).ToLower();
            string postData = $"{name1}={value}&{name2}={id}&{name3}={rnd.Next(27, 60)}&{name4}={rnd.Next(21, 49)}";
            await client.PostAsync($"/elevage/chevaux/doDrink", postData);
        }

        public static async Task Stroke(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("form-do-stroke").Children[1].GetAttribute("name").ToLower();
            string name2 = document.GetElementById("form-do-stroke").Children[2].GetAttribute("name").Substring(14).ToLower();
            string value = document.GetElementById("form-do-stroke").Children[1].GetAttribute("value").ToLower();
            string name3 = document.GetElementById("form-do-stroke").Children[3].GetAttribute("name").Substring(14).ToLower();
            string name4 = document.GetElementById("form-do-stroke").Children[4].GetAttribute("name").Substring(14).ToLower();
            string postData = $"{name1}={value}&{name2}={id}&{name3}={rnd.Next(27, 60)}&{name4}={rnd.Next(21, 49)}";
            await client.PostAsync($"/elevage/chevaux/doStroke", postData);
        }

        public static async Task Carrot(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name = document.GetElementById("form-do-eat-treat-carotte").Children[1].GetAttribute("name").ToLower();
            string value = document.GetElementById("form-do-eat-treat-carotte").Children[1].GetAttribute("value").ToLower();
            string postData = $"{name}={value}&id={id}&friandise=carotte";
            await client.PostAsync($"/elevage/chevaux/doEatTreat", postData);
        }

        public static async Task Walk(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name = document.GetElementById("formbaladeForet").Children[1].GetAttribute("name").ToLower();
            string value = document.GetElementById("formbaladeForet").Children[1].GetAttribute("value").ToLower();
            string name2 = document.GetElementById("formbaladeForet").Children[2].GetAttribute("name").Substring(15).ToLower();
            string name3 = document.GetElementById("formbaladeForet").Children[3].GetAttribute("name").Substring(15).ToLower();
            string name4 = document.GetElementById("walkforetSlider-sliderHidden").GetAttribute("name").Substring(15).ToLower();
            string postData = $"{name}={value}&{name2}={id}&{name3}=foret&{name4}=10";
            await client.PostAsync($"/elevage/chevaux/doWalk", postData);
        }

        public static async Task Suckle(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("form-do-suckle").Children[1].GetAttribute("name").ToLower();
            string name2 = document.GetElementById("form-do-suckle").Children[2].GetAttribute("name").Substring(14).ToLower();
            string value = document.GetElementById("form-do-suckle").Children[1].GetAttribute("value").ToLower();
            string postData = $"{name1}={value}&{name2}={id}";
            await client.PostAsync($"/elevage/chevaux/doSuckle", postData);
        }

        public static async Task Play(string id, NetClient client)
        {
            var document = Parser.ParseDocument(await client.GetAsync($"/elevage/chevaux/cheval?id=" + id));
            string name1 = document.GetElementById("formCenterPlay").Children[1].GetAttribute("name").ToLower();
            string value = document.GetElementById("formCenterPlay").Children[1].GetAttribute("value").ToLower();
            string name2 = document.GetElementById("formCenterPlay").Children[2].GetAttribute("name").Substring(14).ToLower();
            string name3 = document.GetElementById("centerPlaySlider-sliderHidden").GetAttribute("name").Substring(14).ToLower();
            string postData = $"{name1}={value}&{name2}={id}&playEndurance=0&playVitesse=0&playDressage=0&playGalop=0&playTrot=0&playSaut=0&{name3}=10&playAge=8&playForm=formCenterPlay&playDoucheGain=10&playMaxCompetence=10&playCompetenceGain=0.2";
            await client.PostAsync($"/elevage/chevaux/doPlay", postData);
        }
    }
}
