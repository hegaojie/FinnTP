using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using HtmlAgilityPack;
using System.Linq;

namespace FinnTorget
{
    public class Picker
    {
        private WebClient _wc;
        private DispatcherTimer _timer;
        private FinnConfig _config;
        private readonly IList<TorgetItem> _items;
        private HtmlDocument _htmlDoc;

        public Picker(WebClient wc, FinnConfig config)
        {
            _wc = wc;
            _config = config;
            _htmlDoc = new HtmlDocument { OptionFixNestedTags = true, OptionAutoCloseOnEnd = true };

            _items = new List<TorgetItem>();

            _timer = new DispatcherTimer(DispatcherPriority.SystemIdle) { Interval = TimeSpan.FromSeconds(_config.Interval) };
            _timer.Tick += RunScan;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Restart(FinnConfig config)
        {
            _timer.Stop();
            _timer.Interval = TimeSpan.FromSeconds(config.Interval);
            _timer.Start();
        }

        protected virtual string DownloadString(string url)
        {
            return _wc.DownloadString(url);
        }

        private void RunScan(object sender, EventArgs eventArgs)
        {
            RemoveAllTorgetItems();
            ScanFinnTorgetPages();
            UpdateStartTimeForConfig();
            RaiseScanCompleted();
        }

        private void RemoveAllTorgetItems()
        {
            _items.Clear();
        }

        private void UpdateStartTimeForConfig()
        {
            var latestStartTime = GetLatestDateTime();
            if (IsLaterThanConfigStartTime(latestStartTime))
                _config.StartTime = latestStartTime;
        }

        private DateTime GetLatestDateTime()
        {
            var dateTime = (from torgetItem in _items select torgetItem.PublishTime).Max();
            return dateTime;
        }

        private void ScanFinnTorgetPages()
        {
            try
            {
                FetchResult result;
                var startPage = 1;
                do
                {
                    result = FetchNewTorgetItemFromPage(startPage++);
                } while (result.Continue);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private FetchResult FetchNewTorgetItemFromPage(int pageNo)
        {
            var items = GetAllTorgetNodesOnPage(pageNo);
            return items != null ? AddNewTorgetItems(items) : new FetchResult { Continue = false };
        }

        private FetchResult AddNewTorgetItems(IEnumerable<HtmlNode> items)
        {
            foreach (var torgetItem in items.Where(IsNewTorgetItem).Select(CreateTorgetItem))
                _items.Add(torgetItem);

            return new FetchResult{Continue = items.All(IsNewTorgetItem)};
        }

        private bool IsNewTorgetItem(HtmlNode item)
        {
            return IsLaterThanConfigStartTime(GetPublishTime(item));
        }

        private TorgetItem CreateTorgetItem(HtmlNode item)
        {
            var imageNode = GetImageNode(item);

            var torgetItem = new TorgetItem
                {
                    ID = GetItemId(item),
                    Text = GetImageDescription(imageNode),
                    ImageURL = GetImageUrl(imageNode),
                    URL = GetDetailsUrl(item),
                    PublishTime = GetPublishTime(item)
                };

            return torgetItem;
        }

        private string GetItemId(HtmlNode item)
        {
            return item.Attributes["id"].Value;
        }

        private string GetImageDescription(HtmlNode imageNode)
        {
            return imageNode.Attributes["alt"].Value;
        }

        private string GetImageUrl(HtmlNode imageNode)
        {
            return imageNode.Attributes["src"].Value;
        }

        private string GetDetailsUrl(HtmlNode item)
        {
            return "www.finn.no/finn/torget/" + item.SelectSingleNode(".//div[@class='photoframe']/a").Attributes["href"].Value;
        }

        private HtmlNode GetImageNode(HtmlNode item)
        {
            return item.SelectSingleNode(".//img");
        }

        private DateTime GetPublishTime(HtmlNode item)
        {
            var publishTime = item.SelectSingleNode(".//dd[@data-automation-id='dateinfo']").InnerText;
            return _config.FromDateTimeString(publishTime);
        }

        private IEnumerable<HtmlNode> GetAllTorgetNodesOnPage(int pageNo)
        {
            var url = GetUrl(pageNo);
            var html = DownloadString(url);
            _htmlDoc.LoadHtml(html);
            return _htmlDoc.DocumentNode.SelectNodes("//div[@class='man phl pbm ptl gridview r-object media']");
        }

        private string GetUrl(int pageNo)
        {
            return _config.Url.Replace("&page=", "&page=" + pageNo);
        }

        private bool IsLaterThanConfigStartTime(DateTime dateTime)
        {
            return DateTime.Compare(_config.StartTime, dateTime) <= 0;
        }

        #region Events

        public event ScanCompletedHandler ScanCompeleted;

        private void RaiseScanCompleted()
        {
            if (ScanCompeleted != null)
                ScanCompeleted(_items);
        }

        #endregion
    }

    public delegate void ScanCompletedHandler(IEnumerable<TorgetItem> torgets);

}