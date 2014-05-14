using System;

namespace FinnTorget
{
    public class FetchResult
    {
        public bool Continue { get; set; }
        public DateTime? NextNotifyTimeStart { get; set; }
    }
}