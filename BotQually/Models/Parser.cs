using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;
using Newtonsoft.Json.Linq;

namespace BotQually
{
    public static class Parser
    {
        private static readonly HtmlParser parser = new HtmlParser();

        public static IHtmlDocument ParseDocument(string html)
        {
            IHtmlDocument document = parser.Parse(html);
            return document;
        }

        public static IHtmlDocument ParseDocument(string json, string selector)
        {
            JObject jobj = JObject.Parse(json);
            string html = (string)jobj[selector];
            IHtmlDocument document = parser.Parse(html);
            return document;
        }
    }
}
