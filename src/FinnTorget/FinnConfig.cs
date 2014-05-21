using System;
using System.Globalization;

namespace FinnTorget
{
    public class FinnConfig : IConfig
    {
        private DateTime _startTime;

        public string StartTimeString { get; private set; }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                StartTimeString = _startTime.ToString(_cultureInfo);
            }
        }

        public string Url { get; set; }

        public double Interval { get; set; }

        public int BalloonTimeOut { get; set; }

        private readonly CultureInfo _cultureInfo = new CultureInfo("nb-NO");

        public DateTime FromDateTimeString(string dateTime)
        {
            DateTime dt;
            return DateTime.TryParse(dateTime, _cultureInfo, DateTimeStyles.None, out dt) ? dt : DateTime.Now;
        }
    }
}