using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FinnTorget
{
    public class MyNotifyIcon : FrameworkElement
    {
        private const int TIMEOUT = 4000;
        public const int MAX_POPUPS = 6;

        private NotifyIconData _iconData;

        private readonly MessageSink _msgSink;

        private readonly int _timeOut;

        private readonly int _offsetY = 120;

        private readonly IList<MyPopup> _popups = new List<MyPopup>();

        private readonly PositionQueue _queue;

        private readonly Icon _startIcon;

        private readonly Icon _notifyIcon;

        public int PopupCount { get { return _popups.Count; } }

        public MyNotifyIcon(Action<MouseEvent> mouseEventHandler)
        {
            _msgSink = new MessageSink();
            _msgSink.MouseEventReceived += mouseEventHandler;

            var x = SystemParameters.PrimaryScreenWidth - 240;
            var y = SystemParameters.PrimaryScreenHeight - 120;
            _queue = new PositionQueue(x, y, _offsetY);

            _timeOut = TIMEOUT;

            _startIcon = new Icon(@"Icons\emotion-7.ico");
            _notifyIcon = new Icon(@"Icons\emotion-14.ico");

            
            if (ContextMenu == null)
                ContextMenu = new ContextMenu();

            var mi = new MenuItem { Header = "Exit" };
            mi.Click += MiOnClick;

            ContextMenu.Items.Add(mi);
        }

        private void MiOnClick(object sender, RoutedEventArgs e)
        {
            DeleteIcon();

            if (IconRemoved != null)
                IconRemoved(this);
        }

        public void ShowBalloon(FancyBalloon balloonContent)
        {
            var popup = new MyPopup(_timeOut, balloonContent);
            popup.IsActivated += PopupOnIsActivated;
            popup.IsClosed += PopupOnIsClosed;

            Position pos;
            lock (this)
            {
                pos = _queue.ObtainPosition(popup.Id);
                _popups.Add(popup);
            }

            popup.Show(pos);
        }

        public void ShowContextMenu(Point position)
        {
            ContextMenu.IsOpen = true;
            ContextMenu.Placement = PlacementMode.AbsolutePoint;

            ContextMenu.HorizontalOffset = position.X;
            ContextMenu.VerticalOffset = position.Y;

            Win32Api.SetForegroundWindow(_msgSink.MsgWinHandle);
        }

        private void PopupOnIsClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var popup = sender as MyPopup;
            lock (this)
            {
                _queue.ReleasePosition(popup.Id);
                _popups.Remove(popup);
            }
        }

        private void PopupOnIsActivated(object sender, RoutedEventArgs routedEventArgs)
        {
        }

        public event Action<object> IconRemoved;

        public void AddIcon()
        {
            _iconData = new NotifyIconData();
            _iconData.cbSize = (uint)Marshal.SizeOf(_iconData);
            _iconData.uFlags = NotifyIconDisplayMode.NIF_ICON | NotifyIconDisplayMode.NIF_MESSAGE | NotifyIconDisplayMode.NIF_TIP;

            _iconData.hIcon = _startIcon.Handle;

            _iconData.uTimeoutOruVersion = 10;
            _iconData.uCallbackMessage = 0x400;

            _iconData.hWnd = _msgSink.MsgWinHandle;

            _iconData.dwState = StateMode.NIS_HIDDEN;
            _iconData.dwStateMask = StateMode.NIS_HIDDEN;
            _iconData.szInfoTitle = "";
            _iconData.szTip = "";
            _iconData.szInfo = "";

            Win32Api.Shell_NotifyIcon(IconMessageType.NIM_ADD, ref _iconData);
        }

        public void DeleteIcon()
        {
            Win32Api.Shell_NotifyIcon(IconMessageType.NIM_DELETE, ref _iconData);
        }

        public void ModifyIcon()
        {
            if (_iconData.hIcon == _notifyIcon.Handle)
                return;

            _iconData.hIcon = _notifyIcon.Handle;
            Win32Api.Shell_NotifyIcon(IconMessageType.NIM_MODIFY, ref _iconData);
        }

        public void RestoreIcon()
        {
            _iconData.hIcon = _startIcon.Handle;
            Win32Api.Shell_NotifyIcon(IconMessageType.NIM_MODIFY, ref _iconData);
        }

        #region ParentTaskbarIcon

        /// <summary>
        /// An attached property that is assigned to 
        /// </summary>  
        public static readonly DependencyProperty ParentTaskbarIconProperty =
            DependencyProperty.RegisterAttached("ParentTaskbarIcon", typeof(MyNotifyIcon), typeof(MyNotifyIcon));

        /// <summary>
        /// Gets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static MyNotifyIcon GetParentTaskbarIcon(DependencyObject d)
        {
            return (MyNotifyIcon)d.GetValue(ParentTaskbarIconProperty);
        }

        /// <summary>
        /// Sets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetParentTaskbarIcon(DependencyObject d, MyNotifyIcon value)
        {
            d.SetValue(ParentTaskbarIconProperty, value);
        }

        #endregion

        #region CustomBalloon

        /// <summary>
        /// CustomBalloon Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey CustomBalloonPropertyKey
            = DependencyProperty.RegisterReadOnly("CustomBalloon", typeof(Popup), typeof(MyNotifyIcon),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty CustomBalloonProperty
            = CustomBalloonPropertyKey.DependencyProperty;

        /// <summary>
        /// A custom popup that is being displayed in the tray area in order
        /// to display messages to the user.
        /// </summary>
        public Popup CustomBalloon
        {
            get { return (Popup)GetValue(CustomBalloonProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the <see cref="CustomBalloon"/> property.  
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetCustomBalloon(Popup value)
        {
            SetValue(CustomBalloonPropertyKey, value);
        }

        #endregion
    }
}