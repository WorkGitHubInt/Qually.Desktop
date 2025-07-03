using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotQually
{
    public class Client : IClient
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public HttpClient HttpClient { get; private set; }
        public HttpClientHandler Handler { get; private set; }
        public CancellationToken Ct { get; set; } = new CancellationToken();
        public string SID { get; set; }
        public string BaseAddress { get; set; }

        public Client(string uri, IWebProxy proxy)
        {
            var cookies = new CookieContainer();
            Handler = new HttpClientHandler()
            {
                CookieContainer = cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            if (proxy != null)
            {
                Handler.Proxy = proxy;
            }
            HttpClient = new HttpClient(Handler)
            {
                BaseAddress = new Uri(uri)
            };
            BaseAddress = uri;
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36 OPR/95.0.0.0");
            HttpClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
            HttpClient.DefaultRequestHeaders.Host = HttpClient.BaseAddress.Host;
            HttpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br, zstd");
            HttpClient.DefaultRequestHeaders.Add("Origin", HttpClient.BaseAddress.ToString());
            HttpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            HttpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }

        public Client(string uri, IWebProxy proxy, CookieContainer cookieContainer, string sid)
        {
            Handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            if (proxy != null)
            {
                Handler.Proxy = proxy;
            }
            HttpClient = new HttpClient(Handler)
            {
                BaseAddress = new Uri(uri)
            };
            BaseAddress = uri;
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 OPR/86.0.4363.59");
            HttpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US;q=0.8,en;q=0.7");
            HttpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br, zstd");
            HttpClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
            HttpClient.DefaultRequestHeaders.Host = HttpClient.BaseAddress.Host;
            HttpClient.DefaultRequestHeaders.Add("Origin", HttpClient.BaseAddress.ToString());
            HttpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Opera\";v=\"86\"");
            HttpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            HttpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            HttpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            HttpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
        }

        public async Task<string> PostAsync(string url, string postData)
        {
            string answer = "";
            Result requestResult;
            do
            {
                Ct.ThrowIfCancellationRequested();
                try
                {
                    var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpResponseMessage response = await HttpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    answer = await response.Content.ReadAsStringAsync();
                    response.Dispose();
                    requestResult = Result.Success;
                }
                catch (Exception e)
                {
                    requestResult = Result.Error;
                    logger.Error("Ошибка интернет: " + e.Message + " URL: " + url + "\nPostData: " + postData);
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
                Ct.ThrowIfCancellationRequested();
                try
                {
                    HttpResponseMessage response = await HttpClient.GetAsync(url, Ct);
                    response.EnsureSuccessStatusCode();
                    answer = await response.Content.ReadAsStringAsync();
                    response.Dispose();
                    requestResult = Result.Success;
                }
                catch (Exception e)
                {
                    requestResult = Result.Error;
                    logger.Error("Ошибка интернет: " + e.Message + " URL: " + url);
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
    }
}