using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuallyFlash
{
    public class RequestClient : IClient
    {
        public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public CancellationToken Ct { get; set; }
        public string SID { get; set; }
        public WebProxy Proxy { get; set; }
        public string BaseAddress { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public Account Account { get; set; }

        public async Task<string> GetAsync(string url)
        {
            string answer = string.Empty;
            Result result;
            do
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseAddress + url);
                request.Method = "GET";
                request = SetOptions(request);
                try
                {
                    Ct.ThrowIfCancellationRequested();
                    using (var response = await request.GetResponseAsync(Ct).ConfigureAwait(false))
                    using (Stream stream = response.GetResponseStream())
                    {
                        var sr = new StreamReader(stream);
                        answer = await sr.ReadToEndAsync().ConfigureAwait(false);
                        //if (response.ResponseUri.AbsoluteUri.Contains("/site/logIn?redirection="))
                        //{
                        //    await Relog();
                        //    answer = await GetAsync(url);
                        //}
                        result = Result.Success;
                    }
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
                    result = Result.Error;
                }
            } while (result == Result.Error || result == Result.Empty);
            return answer;
        }

        public async Task<string> PostAsync(string url, string postData)
        {
            string answer = string.Empty;
            Result result;
            do
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseAddress + url);
                request.Method = "POST";
                request = SetOptions(request);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                try
                {
                    Ct.ThrowIfCancellationRequested();
                    Stream stream = request.GetRequestStream();
                    stream.Write(byteArray, 0, byteArray.Length);
                    stream.Close();
                    var response = await request.GetResponseAsync(Ct).ConfigureAwait(false);
                    using (stream = response.GetResponseStream())
                    {
                        var sr = new StreamReader(stream);
                        answer = await sr.ReadToEndAsync().ConfigureAwait(false);
                        //if (answer.Contains("\\/site\\/logIn"))
                        //{
                        //    await Relog();
                        //    answer = await PostAsync(url, postData);
                        //}
                        result = Result.Success;
                    }
                    response.Close();
                }
                catch (Exception e)
                {
                    if (e is OperationCanceledException)
                    {
                        throw new OperationCanceledException(e.Message, e, Ct);
                    } else
                    {
                        logger.Error("Ошибка интернет: " + e.Message);
                    }
                    result = Result.Error;
                }
            } while (result == Result.Error || result == Result.Empty);
            return answer;
        }

        public void SetSID()
        {
            foreach (Cookie cookie in CookieContainer.GetCookies(new Uri(BaseAddress)))
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
                CookieContainer = await Account.ReLogIn();
                await Account.LogInCo(loginCo, false);
                Account.Login = loginCo;
                Account.LoginCo = login;
            }
            else
            {
                CookieContainer = await Account.ReLogIn();
            }
        }

        private HttpWebRequest SetOptions(HttpWebRequest request)
        {
            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.Expect100Continue = false;
            request.Host = BaseAddress.Substring(8);
            request.KeepAlive = true;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.CookieContainer = CookieContainer;
            return request;
        }
    }
}
