using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json.Linq;

namespace AccountHolder
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
            string html = jobj[selector][0].Values()[0].ToString();
            IHtmlDocument document = parser.ParseDocument(html);
            return document;
        }
    }
}
