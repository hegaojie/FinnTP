using System.Collections.Generic;
using HtmlAgilityPack;

namespace FinnTorget
{
    public class FinnHtmlParser
    {
        private readonly HtmlDocument _htmlDoc;

        public FinnHtmlParser()
        {
            _htmlDoc = new HtmlDocument { OptionFixNestedTags = true, OptionAutoCloseOnEnd = true };
        }

        public IEnumerable<HtmlNode> ParseTorgetNodes(string html)
        {
            _htmlDoc.LoadHtml(html);
            return _htmlDoc.DocumentNode.SelectNodes("//div[@class='man phl pbm ptl gridview r-object media']");
        }

        public string GetItemId(HtmlNode item)
        {
            return item.Attributes["id"].Value;
        }

        public string GetImageDescription(HtmlNode imageNode)
        {
            return imageNode.Attributes["alt"].Value;
        }

        public string GetImageUrl(HtmlNode imageNode)
        {
            return imageNode.Attributes["src"].Value;
        }

        public string GetDetailsUrl(HtmlNode item)
        {
            return "www.finn.no/finn/torget/" + item.SelectSingleNode(".//div[@class='photoframe']/a").Attributes["href"].Value;
        }

        public HtmlNode GetImageNode(HtmlNode item)
        {
            return item.SelectSingleNode(".//img");
        }

        public string GetPublishTimeString(HtmlNode item)
        {
            return item.SelectSingleNode(".//dd[@data-automation-id='dateinfo']").InnerText;
        }
    }
}