using System;
using System.Globalization;
using System.Windows;

namespace FinnTorget
{
    public class FinnConfigManager : IConfigManager
    {
        public const int DEFAULT_TIMEOUT_SECONDS = 5;

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

        public FinnConfig LoadConfiguration()
        {
            var config = new FinnConfig();
            try
            {
                using (var reader = CreateReader(_file))
                {
                    var line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                        config.StartTime = ParseDateTime(line);

                    line = reader.ReadLine();
                    config.Interval = !String.IsNullOrEmpty(line) ? Convert.ToDouble(line) : DEFAULT_INTERVAL;

                    line = reader.ReadLine();
                    config.Url = !String.IsNullOrEmpty(line) ? line : DEFAULT_URL;

                    line = reader.ReadLine();
                    config.BalloonTimeOut = !String.IsNullOrEmpty(line) ? Convert.ToInt32(line) : DEFAULT_TIMEOUT_SECONDS;
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox(LOAD_ERROR_MESSAGE);
                config = LoadDefaultConfiguration();
            }
            return config;
        }
        
        public void SaveConfiguration(FinnConfig config)
        {
            try
            {
                using (var writer = CreateWriter(_file))
                {
                    var str = config.StartTime.ToString(_cultrueInfo);
                    writer.WriteLine(str);
                    writer.WriteLine(config.Interval.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(config.Url);
                    writer.WriteLine(config.BalloonTimeOut.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox(SAVE_ERROR_MESSAGE);
            }
        }

        private FinnConfig LoadDefaultConfiguration()
        {
            var config = new FinnConfig
                {
                    StartTime = ParseDateTime(DateTime.Now.ToString(_cultrueInfo)),
                    Interval = _interval,
                    Url = DEFAULT_URL,
                    BalloonTimeOut = DEFAULT_TIMEOUT_SECONDS
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