using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Xml.Serialization;

namespace AccountHolder
{
    public class Account : BaseViewModel
    {
        [XmlIgnore]
        public Dictionary<Server, string> Servers = new Dictionary<Server, string>
        {
            { Server.Australia, "https://au.howrse.com"},
            { Server.England, "https://www.howrse.co.uk"},
            { Server.Arabic, "https://ar.howrse.com" },
            { Server.Bulgaria, "https://www.howrse.bg" },
            { Server.International, "https://www.howrse.com" },
            { Server.Spain, "https://www.caballow.com" },
            { Server.Canada,"https://ca.howrse.com" },
            { Server.Germany, "https://www.howrse.de" },
            { Server.Norway, "https://www.howrse.no" },
            { Server.Poland, "https://www.howrse.pl" },
            { Server.Russia, "https://www.lowadi.com" },
            { Server.Romain, "https://www.howrse.ro" },
            { Server.USA, "https://us.howrse.com" },
            { Server.FranceOuranos, "https://ouranos.equideow.com" },
            { Server.FranceGaia, "https://gaia.equideow.com" },
            { Server.Sweden, "https://www.howrse.se" }
        };
        [XmlIgnore]
        public Server Server { get; set; } = Server.Russia;
        [XmlIgnore]
        public NetClient Client { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Days { get; set; }
        public string Tutorial { get; set; }
        [XmlIgnore]
        public string Progress { get; set; }

        public async Task<bool> LogIn()
        {
            string answer;
            string baseAddress = Servers[Server];
            if (Server == Server.FranceGaia || Server == Server.FranceOuranos)
            {
                Client = new NetClient("https://www.equideow.com");
                answer = await Client.GetAsync("/");
                var document = Parser.ParseDocument(answer);
                string name = document.GetElementById("authentification").Children[1].GetAttribute("name").ToLower();
                string value = document.GetElementById("authentification").Children[1].GetAttribute("value").ToLower();
                string server;
                if (Server == Server.FranceOuranos)
                {
                    server = "ouranos";
                }
                else
                {
                    server = "gaia";
                }
                string postData = $"{name}={value}&to={server}&login={Login}&password={Password}&redirection=&isBoxStyle=";
                answer = await Client.PostAsync("/site/doLogIn", postData);
                if (answer.Contains("?identification=1") || answer.Contains("\"errors\":[]"))
                {
                    string url = Regex.Match(answer, "\"redirection\":\"(.*?)\"}").Groups[1].Value.Replace("\\", "");
                    string query = url.Substring(baseAddress.Length);
                    Client = new NetClient(baseAddress);
                    await Client.GetAsync(query);
                    Client.SetSID();
                    return true;
                }
                return false;
            }
            else
            {
                Client = new NetClient(baseAddress);
                var document = Parser.ParseDocument(await Client.GetAsync("/"));
                string name = document.GetElementById("authentification").Children[1].GetAttribute("name").ToLower();
                string value = document.GetElementById("authentification").Children[1].GetAttribute("value").ToLower();
                string postData = $"{name}={value}&login={Login}&password={HttpUtility.UrlEncode(Password)}&redirection=&isBoxStyle=";
                answer = await Client.PostAsync("/site/doLogIn", postData);
                if (answer.Contains("?identification=1") || answer.Contains("\"errors\":[]"))
                {
                    Client.SetSID();
                    return true;
                }
                return false;
            }
        }

        public async void GoAccount()
        {
            var document = Parser.ParseDocument(await Client.GetAsync("/"));
            string name = document.GetElementById("authentification").Children[1].GetAttribute("name").ToLower();
            string value = document.GetElementById("authentification").Children[1].GetAttribute("value").ToLower();
            string postData = $"{name}={value}&login={Login}&password={Password}&redirection=&isBoxStyle=";
            await Client.PostAsync("/site/doLogIn", postData);
            document = Parser.ParseDocument(await Client.GetAsync($"/jeu/"));
            string id = document.GetElementById("header-hud").GetElementsByClassName("level-2 level-extended")[0].GetAttribute("href").Split('=')[1];
            document = Parser.ParseDocument(await Client.GetAsync($"/joueur/fiche/?id={id}"));
            Days = document.GetElementsByClassName("color-brown")[0].GetElementsByTagName("strong")[0].TextContent;
        }

        public async Task GetDays()
        {
            if (await LogIn())
            {
                try
                {
                    var document = Parser.ParseDocument(await Client.GetAsync($"/jeu/"));
                    string id = document.GetElementsByClassName("level-2 level-extended")[0].GetAttribute("href").Split('=')[1];
                    document = Parser.ParseDocument(await Client.GetAsync($"/joueur/fiche/?id=" + id));
                    string day = document.GetElementsByClassName("color-brown")[0].GetElementsByTagName("strong")[0].TextContent;
                    Days = day;
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.AccountErrorMessage1, Properties.Resources.AccountErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else
            {
                MessageBox.Show(Properties.Resources.AccountErrorMessage2, Properties.Resources.AccountErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task LogOut()
        {
            await Client.PostAsync($"/site/doLogOut", $"sid={Client.SID}");
        }

        public async Task DoTraining(string apple)
        {
            if (await LogIn())
            {
                Guide guide = new Guide()
                {
                    Acc = this
                };
                Progress = $"{Properties.Resources.StageText} 1";
                await guide.Stage1();
                Progress = $"{Properties.Resources.StageText} 2";
                await guide.Stage2();
                Progress = $"{Properties.Resources.StageText} 3";
                await guide.Stage3();
                Progress = $"{Properties.Resources.StageText} 4";
                await guide.Stage4();
                Progress = $"{Properties.Resources.StageText} 5";
                await guide.Stage5();
                Progress = $"{Properties.Resources.StageText} 6";
                await guide.Stage6();
                Progress = $"{Properties.Resources.StageText} 7";
                await guide.Stage7();
                Progress = $"{Properties.Resources.StageText} 8";
                await guide.Stage8();
                Progress = $"{Properties.Resources.StageText} 9";
                await guide.Stage9();
                Progress = $"{Properties.Resources.StageText} 10";
                await guide.Stage10();
                Progress = $"{Properties.Resources.StageText} 11";
                await guide.Stage11();
                Progress = $"{Properties.Resources.StageText} 12";
                await guide.Stage12();
                Progress = $"{Properties.Resources.StageText} 13";
                await guide.Stage13();
                Progress = $"{Properties.Resources.StageText} 14";
                await guide.Stage14(apple);
                Progress = $"{Properties.Resources.StageText} 15";
                await guide.Stage15();
                Progress = $"{Properties.Resources.StageText} 16";
                await guide.Stage16();
                Tutorial = Properties.Resources.TutorialYesText;
            }
            else
            {
                MessageBox.Show(Properties.Resources.AccountErrorMessage2, Properties.Resources.AccountErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
