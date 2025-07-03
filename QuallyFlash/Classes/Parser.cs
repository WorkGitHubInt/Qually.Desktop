using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using Newtonsoft.Json.Linq;

namespace QuallyFlash
{
    public static class Parser
    {
        private static readonly HtmlParser parser = new HtmlParser();

        public static IHtmlDocument ParseDocument(string html)
        {
            IHtmlDocument document = parser.ParseDocument(html);
            return document;
        }

        public static IHtmlDocument ParseDocument(string json, string selector)
        {
            JObject jobj = JObject.Parse(json);
            string html = (string)jobj[selector];
            IHtmlDocument document = parser.ParseDocument(html);
            return document;
        }

        public static IHtmlDocument ParseDocument(string json, string selector1, string selector2)
        {
            JObject jobj = JObject.Parse(json);
            string html = (string)jobj[selector1][selector2];
            IHtmlDocument document = parser.ParseDocument(html);
            return document;
        }

        public static string ParseAnswer(string json, string selector)
        {
            JObject jobj = JObject.Parse(json);
            string answer = (string)jobj[selector];
            return answer;
        }
    }
}
