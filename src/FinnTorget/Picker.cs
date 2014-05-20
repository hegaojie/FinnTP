using System;
using System.Collections.Generic;
using System.Media;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using HtmlAgilityPack;

namespace FinnTorget
{
    public class Picker
    {
        private WebClient _wc;

        private DispatcherTimer _timer;
        
        private FinnConfig _config;

        private readonly IList<TorgetItem> _items;

        public Picker(WebClient wc, FinnConfig config)
        {
            _wc = wc;
            _config = config;

            _items = new List<TorgetItem>();

            _timer = new DispatcherTimer(DispatcherPriority.SystemIdle) { Interval = TimeSpan.FromSeconds(_config.Interval) };
            _timer.Tick += RunScan;
        }

        public void RunTimer()
        {
            _timer.Start();
        }

        public void UpdateConfig(FinnConfig config)
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
            _items.Clear();

            var startPage = 1;
            FetchResult result;
            DateTime? nextNotifyTimeStart = null;

            do
            {
                result = FetchTorgetNew(startPage++);

                if (result.NextNotifyTimeStart.HasValue)
                {
                    nextNotifyTimeStart = result.NextNotifyTimeStart;
                }

            } while (result.Continue);

            if (nextNotifyTimeStart.HasValue)
                _config.StartTime = nextNotifyTimeStart.Value;

            RaiseScanCompleted();
        }

        private FetchResult FetchTorgetNew(int pageNo)
        {
            var result = new FetchResult { Continue = true };

            try
            {
                var items = GetAllTorgetNodesOnPage(pageNo);

                if (items != null)
                {
                    var newNotifyStartInitialized = false;

                    foreach (var item in items)
                    {
                        var dt = GetPublishTime(item);
                        if (IsNew(dt))
                        {
                            if (pageNo == 1 && !newNotifyStartInitialized)
                            {
                                result.NextNotifyTimeStart = dt.AddSeconds(10);
                                newNotifyStartInitialized = true;
                            }

                            var imgNode = item.SelectSingleNode(".//img");

                            var hrefnode = item.SelectSingleNode(".//div[@class='photoframe']/a");

                            var torgetItem = new TorgetItem
                            {
                                ID = item.Attributes["id"].Value,
                                Text = imgNode.Attributes["alt"].Value,
                                ImageURL = imgNode.Attributes["src"].Value,
                                URL = "www.finn.no/finn/torget/" + hrefnode.Attributes["href"].Value,
                                PublishTime = dt
                            };

                            _items.Add(torgetItem);
                        }
                        else
                        {
                            result.Continue = false;
                            break;
                        }
                    }
                }
                else
                {
                    result.Continue = false;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            return result;
        }

        private DateTime GetPublishTime(HtmlNode item)
        {
            var timeNode = item.SelectSingleNode(".//dd[@data-automation-id='dateinfo']");

            var publishTime = timeNode.InnerText;

            var dt = _config.FromDateTimeString(publishTime);

            return dt;
        }

        private IEnumerable<HtmlNode> GetAllTorgetNodesOnPage(int pageNo)
        {
            var url = GetUrl(pageNo);

            var html = DownloadString(url);

            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true, OptionAutoCloseOnEnd = true };

            htmlDoc.LoadHtml(html);

            var items = htmlDoc.DocumentNode.SelectNodes("//div[@class='man phl pbm ptl gridview r-object media']");

            return items;
        }

        private string GetUrl(int pageNo)
        {
            return _config.Url.Replace("&page=", "&page=" + pageNo);
        }

        private bool IsNew(DateTime dateTime)
        {
            var ret = DateTime.Compare(_config.StartTime, dateTime) <= 0;
            return ret;
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