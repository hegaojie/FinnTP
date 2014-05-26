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
        private readonly WebClient _wc;
        private readonly FinnHtmlParser _finnHtmlParser;
        private DispatcherTimer _timer;

        private FinnConfig _config;
        private readonly IList<TorgetItem> _items;

        public Picker(WebClient wc, FinnHtmlParser htmlParser)
        {
            _wc = wc;
            _finnHtmlParser = htmlParser;
            _items = new List<TorgetItem>();
        }

        public void Run(FinnConfig config)
        {
            _config = config;
            Restart();
        }

        private DispatcherTimer Timer
        {
            get
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer(DispatcherPriority.SystemIdle);
                    _timer.Tick += RunScan;
                }
                return _timer;
            }
        }

        private void RunScan(object sender, EventArgs eventArgs)
        {
            RemoveAllTorgetItems();
            ScanFinnTorgetPages();
            RaiseScanCompleted();
        }

        private void Restart()
        {
            StopTimer();
            StartTimer();
        }

        private void StartTimer()
        {
            Timer.Interval = TimeSpan.FromSeconds(_config.Interval);
            Timer.Start();
        }

        private void StopTimer()
        {
            if (Timer.IsEnabled)
                Timer.Stop();
        }

        private void RemoveAllTorgetItems()
        {
            _items.Clear();
        }

        private void ScanFinnTorgetPages()
        {
            try
            {
                bool hasMoreToFetch;
                var startPage = 1;
                do
                {
                    hasMoreToFetch = FetchNewTorgetItemFromPage(startPage++);
                } while (hasMoreToFetch);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private string DownloadString(string url)
        {
            return _wc.DownloadString(url);
        }

        private DateTime GetLatestDateTime()
        {
            return _items.Count > 0 ? (from torgetItem in _items select torgetItem.PublishTime).Max() : new DateTime();
        }

        private bool FetchNewTorgetItemFromPage(int pageNo)
        {
            var items = GetAllTorgetNodesOnPage(pageNo);
            return items != null && AddNewTorgetItems(items);
        }

        private bool AddNewTorgetItems(IEnumerable<HtmlNode> items)
        {
            foreach (var torgetItem in items.Where(IsNewTorgetItem).Select(CreateTorgetItem))
                _items.Add(torgetItem);

            return items.All(IsNewTorgetItem);
        }

        private bool IsNewTorgetItem(HtmlNode item)
        {
            return _config.IsStartTimeEarlierThan(GetPublishTime(item));
        }

        private TorgetItem CreateTorgetItem(HtmlNode item)
        {
            var imageNode = _finnHtmlParser.GetImageNode(item);

            var torgetItem = new TorgetItem
                {
                    ID = _finnHtmlParser.GetItemId(item),
                    Text = _finnHtmlParser.GetImageDescription(imageNode),
                    ImageURL = _finnHtmlParser.GetImageUrl(imageNode),
                    URL = _finnHtmlParser.GetDetailsUrl(item),
                    PublishTime = GetPublishTime(item)
                };

            return torgetItem;
        }

        private DateTime GetPublishTime(HtmlNode item)
        {
            var publishTime = _finnHtmlParser.GetPublishTimeString(item);
            return _config.FromDateTimeString(publishTime);
        }

        private IEnumerable<HtmlNode> GetAllTorgetNodesOnPage(int pageNo)
        {
            var url = GetUrl(pageNo);
            var html = DownloadString(url);
            return _finnHtmlParser.ParseTorgetNodes(html);
        }

        private string GetUrl(int pageNo)
        {
            return _config.Url.Replace("&page=", "&page=" + pageNo);
        }

        #region Events

        public event ScanCompletedHandler ScanCompeleted;

        private void RaiseScanCompleted()
        {
            if (ScanCompeleted != null)
                ScanCompeleted(_items, GetLatestDateTime());
        }

        #endregion
    }

    public delegate void ScanCompletedHandler(IEnumerable<TorgetItem> torgets, DateTime newStartTime);

}