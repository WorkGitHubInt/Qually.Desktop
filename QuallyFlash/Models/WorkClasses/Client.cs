using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuallyFlash
{
    public class Client : IClient
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public HttpClient HttpClient { get; private set; }
        public HttpClientHandler Handler { get; private set; }
        public CancellationToken Ct { get; set; }
        public string SID { get; set; }
        public Account Account { get; set; }

        public Client(string uri, IWebProxy proxy)
        {
            var cookies = new CookieContainer();
            Handler = new HttpClientHandler()
            {
                CookieContainer = cookies,
            };
            if (proxy != null)
            {
                Handler.Proxy = proxy;
            }
            HttpClient = new HttpClient(Handler)
            {
                BaseAddress = new Uri(uri)
            };
        }

        public Client(string uri, IWebProxy proxy, CookieContainer cookieContainer, string sid)
        {
            Handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
            };
            if (proxy != null)
            {
                Handler.Proxy = proxy;
            }
            HttpClient = new HttpClient(Handler)
            {
                BaseAddress = new Uri(uri)
            };
        }

        public async Task<string> PostAsync(string url, string postData)
        {
            string answer = "";
            Result requestResult;
            do
            {
                try
                {
                    Ct.ThrowIfCancellationRequested();
                    var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = await HttpClient.PostAsync(url, content, Ct);
                    result.EnsureSuccessStatusCode();
                    answer = await result.Content.ReadAsStringAsync();
                    //if (result.RequestMessage.RequestUri.PathAndQuery.Contains("/site/logIn?redirection="))
                    //{
                    //    await Relog();
                    //    answer = await GetAsync(url);
                    //}
                    requestResult = Result.Success;
                }
                catch (Exception e)
                {
                    if (e is OperationCanceledException)
                    {
                        throw new OperationCanceledException(e.Message, e, Ct);
                    }
                    else
                    {
                        logger.Error("Ошибка интернет: " + e.Message);
                    }
                    requestResult = Result.Error;
                }
            } while (requestResult == Result.Error || requestResult == Result.Empty);
            return answer;
        }

        public async Task<string> GetAsync(string url)
        {
            string answer = "";
            Result requestResult;
            do
            {
                try
                {
                    Ct.ThrowIfCancellationRequested();
                    var result = await HttpClient.GetAsync(url, Ct);
                    result.EnsureSuccessStatusCode();
                    answer = await result.Content.ReadAsStringAsync();
                    //if (result.RequestMessage.RequestUri.PathAndQuery.Contains("/site/logIn?redirection="))
                    //{
                    //    await Relog();
                    //    answer = await GetAsync(url);
                    //}
                    requestResult = Result.Success;
                }
                catch (Exception e)
                {
                    if (e is OperationCanceledException)
                    {
                        throw new OperationCanceledException(e.Message, e, Ct);
                    }
                    else
                    {
                        logger.Error("Ошибка интернет: " + e.Message);
                    }
                    requestResult = Result.Error;
                }
            } while (requestResult == Result.Error || requestResult == Result.Empty);
            return answer;
        }

        public void SetSID()
        {
            foreach (Cookie cookie in Handler.CookieContainer.GetCookies(HttpClient.BaseAddress))
            {
                if (cookie.Name == "sessionprod")
                {
                    SID = cookie.Value;
                }
            }
        }

        public async Task Relog()
        {
            if (Account.Type == AccountType.Co)
            {
                string login = Account.LoginCo;
                string loginCo = Account.Login;
                Account.Login = login;
                await Account.LogIn();
                await Account.LogInCo(loginCo, false);
                Account.Login = loginCo;
                Account.LoginCo = login;
            }
            else
            {
                await Account.LogIn();
            }
        }
    }
}