﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using FinnTorget.Annotations;

namespace FinnTorget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MyNotifyIcon _notifyIcon;

        private readonly FinnConfig _config;

        private readonly IConfigManager _cfgManager;

        private readonly Picker _picker;

        private bool _isActivated;

        #region Properties back fields

        private ObservableCollection<TorgetItem> _items;
        private double _intervalSeconds;
        private string _startTime;
        private string _url;

        #endregion

        #region Properties Getter and Setter

        public IOrderedEnumerable<TorgetItem> Items
        {
            get { return _items.OrderByDescending(i => i.PublishTime); }
        }

        public double Interval
        {
            get { return _intervalSeconds; }
            set
            {
                if (_intervalSeconds != value)
                {
                    _intervalSeconds = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StartTime
        {
            get { return _startTime; }
            set
            {
                if (!String.Equals(_startTime, value))
                {
                    _startTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                if (!String.Equals(_url, value))
                {
                    _url = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            _items = new ObservableCollection<TorgetItem>();

            _cfgManager = new FinnConfigManager();

            _config = _cfgManager.LoadConfiguration();

            _picker = new Picker(new WebClient { Encoding = Encoding.UTF8 }, _config);
            _picker.ScanCompeleted += PickerOnScanCompeleted;
            _picker.RunTimer();

            InitSettings(_config);

            DataContext = this;
        }

        public void ShowAndActivate()
        {
            Show();
            Activate();
            _isActivated = true;
        }

        public void HideAndDeactivate()
        {
            Hide();
            _isActivated = false;
        }

        private void InitSettings(FinnConfig config)
        {
            Url = config.Url;
            Interval = config.Interval;
            StartTime = config.StartTimeString;
        }

        private void PlayNotifySound()
        {
            using (var player = new SoundPlayer(@"Sounds\Shells_falls-Marcel-829263474.wav"))
            {
                player.Play();
            }
        }

        #region CallBack

        private void NotifyIconOnIconRemoved(object obj)
        {
            _cfgManager.SaveConfiguration(_config);
            Application.Current.Shutdown();
        }

        private void PickerOnScanCompeleted(IEnumerable<TorgetItem> torgets)
        {
            if (torgets == null) return;

            bool newTorgetAdded = false;
            foreach (var torgetItem in torgets)
            {
                if (!_items.Contains(torgetItem))
                {
                    _items.Add(torgetItem);
                    newTorgetAdded = true;

                    if (_notifyIcon.PopupCount < MyNotifyIcon.MAX_POPUPS)
                    {
                        var fb = new FancyBalloon { BalloonText = torgetItem.Text };
                        _notifyIcon.ShowBalloon(fb);
                        PlayNotifySound();
                    }
                }
            }

            if (newTorgetAdded)
            {
                OnPropertyChanged("Items");
                _notifyIcon.ModifyIcon();
            }
        }

        private void MsgSinkOnMouseEventReceived(MouseEvent mouseEvent)
        {
            switch (mouseEvent)
            {
                case MouseEvent.RightMouseDown:
                    var position = new Point();
                    Win32Api.GetCursorPos(ref position);

                    _notifyIcon.ShowContextMenu(position);

                    break;

                case MouseEvent.LeftMouseDoubleClick:
                    if (_isActivated)
                        HideAndDeactivate();
                    else
                        ShowAndActivate();

                    break;

                case MouseEvent.LeftMouseDown:
                    break;

                case MouseEvent.LeftMouseUp:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Events Handler

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnRemoveAll_OnClick(object sender, RoutedEventArgs e)
        {
            _items.Clear();
            OnPropertyChanged("Items");
            _notifyIcon.RestoreIcon();
            _notifyIcon.ClearAllPopups();
        }

        private void BtnApply_OnClick(object sender, RoutedEventArgs e)
        {
            _config.Url = Url;
            _config.StartTime = _config.FromDateTimeString(StartTime);
            _config.Interval = Convert.ToDouble(Interval);

            _picker.UpdateConfig(_config);

            MessageBox.Show(this, "New settings are applied!");
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()));
            e.Handled = true;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            HideAndDeactivate();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _notifyIcon = new MyNotifyIcon(MsgSinkOnMouseEventReceived);
            _notifyIcon.IconRemoved += NotifyIconOnIconRemoved;
            _notifyIcon.AddIcon();

            HideAndDeactivate();
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    public class MyStreamReader : StreamReader, IStreamReader
    {
        public MyStreamReader(string file)
            : base(file)
        { }
    }

    public class MyStreamWriter : StreamWriter, IStreamWriter
    {
        public MyStreamWriter(string file)
            : base(file)
        {
        }
    }

    public interface IStreamReader : IDisposable
    {
        string ReadLine();
    }

    public interface IStreamWriter : IDisposable
    {
        void WriteLine(string line);
    }
}
