using System;
using System.Globalization;
using System.Windows;

namespace FinnTorget
{
    public class FinnConfigManager : IConfigManager
    {
        public const double DEFAULT_INTERVAL = 30.0;
        
        public const string DEFAULT_CONFIG_FILE = @"lastNotifyTime.txt";

        public const string DEFAULT_URL =
            "http://www.finn.no/finn/torget/resultat?location=0/20016&location=1/20016/20318&ITEM_CONDITION=0&SEARCHKEYNAV=SEARCH_ID_BAP_FREE&page=&sort=1&periode=";

        public const string LOAD_ERROR_MESSAGE = "Config file doesn't exsit, starting time is set to now!";

        public const string SAVE_ERROR_MESSAGE = "Some error happens when saving config file!";

        private readonly double _interval;

        private readonly string _file;

        private CultureInfo _cultrueInfo = new CultureInfo("nb-NO");

        public FinnConfigManager(string configFile, double defaultInterval)
        {
            _file = configFile;
            _interval = defaultInterval;
        }

        public FinnConfigManager() : this(DEFAULT_CONFIG_FILE, DEFAULT_INTERVAL)
        {
        }

        protected virtual IStreamWriter CreateWriter(string file)
        {
            return new MyStreamWriter(file);
        }

        protected virtual IStreamReader CreateReader(string file)
        {
            return new MyStreamReader(file);
        }

        public virtual void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        public FinnConfig LoadSettings()
        {
            var config = new FinnConfig();
            try
            {
                using (var reader = CreateReader(_file))
                {
                    var line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                        config._startTime = ParseDateTime(line);

                    line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                        config._interval = Convert.ToDouble(line);

                    line = reader.ReadLine();
                    config._url = !String.IsNullOrEmpty(line) ? line : DEFAULT_URL;
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox(LOAD_ERROR_MESSAGE);
                config = LoadDefaultSettings();
            }
            return config;
        }
        
        public void SaveSettings(FinnConfig config)
        {
            try
            {
                using (var writer = CreateWriter(_file))
                {
                    var str = config._startTime.ToString(_cultrueInfo);
                    writer.WriteLine(str);
                    writer.WriteLine(config._interval.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(config._url);
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox(SAVE_ERROR_MESSAGE);
            }
        }

        private FinnConfig LoadDefaultSettings()
        {
            var config = new FinnConfig
                {
                    _startTime = ParseDateTime(DateTime.Now.ToString(_cultrueInfo)),
                    _interval = _interval,
                    _url = DEFAULT_URL
                };
            return config;
        }

        private DateTime ParseDateTime(string dateTime)
        {
            try
            {
                DateTime dt;
                DateTime.TryParse(dateTime, _cultrueInfo, DateTimeStyles.None, out dt);
                return dt;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            return new DateTime();
        }
    }

    public interface IConfig
    {
        
    }
}