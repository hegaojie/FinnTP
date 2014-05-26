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
        private CultureInfo _cultrueInfo;

        public FinnConfigManager(string configFile, double defaultInterval)
        {
            _file = configFile;
            _interval = defaultInterval;
            _cultrueInfo = new CultureInfo("nb-NO");
        }

        public FinnConfigManager()
            : this(DEFAULT_CONFIG_FILE, DEFAULT_INTERVAL)
        {
        }

        public FinnConfig LoadConfiguration()
        {
            try
            {
                return ReadConfigurationFromFile();
            }
            catch (Exception exc)
            {
                ShowMessageBox(LOAD_ERROR_MESSAGE);
                return LoadDefaultConfiguration();
            }
        }

        public void SaveConfiguration(FinnConfig config)
        {
            try
            {
                WriteConfigurationToFile(config);
            }
            catch (Exception exc)
            {
                ShowMessageBox(SAVE_ERROR_MESSAGE);
            }
        }

        public virtual void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        private FinnConfig ReadConfigurationFromFile()
        {
            using (var reader = CreateReader(_file))
            {
                var config = new FinnConfig();
                
                var line = reader.ReadLine();
                config.StartTime = ParseDateTime(!String.IsNullOrEmpty(line) ? line : DateTime.Now.ToString(_cultrueInfo));

                line = reader.ReadLine();
                config.Interval = !String.IsNullOrEmpty(line) ? Convert.ToDouble(line) : DEFAULT_INTERVAL;

                line = reader.ReadLine();
                config.Url = !String.IsNullOrEmpty(line) ? line : DEFAULT_URL;

                line = reader.ReadLine();
                config.BalloonTimeOut = !String.IsNullOrEmpty(line) ? Convert.ToInt32(line) : DEFAULT_TIMEOUT_SECONDS;

                return config;
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

        private void WriteConfigurationToFile(FinnConfig config)
        {
            using (var writer = CreateWriter(_file))
            {
                writer.WriteLine(config.StartTime.ToString(_cultrueInfo));
                writer.WriteLine(config.Interval.ToString(_cultrueInfo));
                writer.WriteLine(config.Url);
                writer.WriteLine(config.BalloonTimeOut.ToString(_cultrueInfo));
            }
        }

        private DateTime ParseDateTime(string dateTime)
        {
            DateTime dt;
            DateTime.TryParse(dateTime, _cultrueInfo, DateTimeStyles.None, out dt);
            return dt;
        }

        protected virtual IStreamWriter CreateWriter(string file)
        {
            return new MyStreamWriter(file);
        }

        protected virtual IStreamReader CreateReader(string file)
        {
            return new MyStreamReader(file);
        }
    }

    public interface IConfig
    {

    }
}