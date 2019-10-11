using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountHolder
{
    public class NetClient
    {
        public HttpClient HttpClient { get; private set; }
        public HttpClientHandler Handler { get; private set; }
        public CancellationToken Ct { get; set; } = new CancellationToken();
        public string SID { get; set; }
        public string BaseAddress { get; set; }

        public NetClient(string uri)
        {
            var cookies = new CookieContainer();
            Handler = new HttpClientHandler()
            {
                CookieContainer = cookies,
            };
            HttpClient = new HttpClient(Handler)
            {
                BaseAddress = new Uri(uri)
            };
            BaseAddress = uri;
        }

        public async Task<string> PostAsync(string url, string postData)
        {
            string answer = "";
            Result requestResult;
            do
            {
                try
                {
                    var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = await HttpClient.PostAsync(url, content, Ct);
                    result.EnsureSuccessStatusCode();
                    answer = await result.Content.ReadAsStringAsync();
                    requestResult = Result.Success;
                }
                catch (ArgumentNullException)
                {
                    requestResult = Result.Error;
                }
                catch (HttpRequestException)
                {
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
                    var result = await HttpClient.GetAsync(url, Ct);
                    result.EnsureSuccessStatusCode();
                    answer = await result.Content.ReadAsStringAsync();
                    requestResult = Result.Success;
                }
                catch (ArgumentNullException)
                {
                    requestResult = Result.Error;
                }
                catch (HttpRequestException)
                {
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
    }
}
