using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using MyNotifyIcon;
using Point = MyNotifyIcon.Point;

namespace MyNotifyIcon
{
    public delegate void ClosingBalloonEventHandler();

    public class WPFNotifyIcon : FrameworkElement
    {
        private const int BLINK_INVERVAL_MILLISECONDS = 500;

        private readonly MessageSink _msgSink;
        private readonly Icon _startIcon;
        private readonly Icon _notifyIcon;
        private DispatcherTimer _timer;
        private NotifyIconData _iconData;

        public WPFNotifyIcon(Icon startIcon, Icon notifyIcon, Action<MouseEvent> mouseEventHandler)
        {
            _msgSink = new MessageSink();
            _msgSink.MouseEventReceived += mouseEventHandler;
            
            _startIcon = startIcon;
            _notifyIcon = notifyIcon;
        }

        public void ShowContextMenu(Point position)
        {
            ContextMenu.IsOpen = true;
            ContextMenu.Placement = PlacementMode.AbsolutePoint;

            ContextMenu.HorizontalOffset = position.X;
            ContextMenu.VerticalOffset = position.Y;

            Win32Api.SetForegroundWindow(_msgSink.MsgWinHandle);
        }

        public void ShowContextMenu(int x, int y)
        {
            ShowContextMenu(new Point { X = x, Y = y });
        }

        public WPFNotifyIcon AddContextMenuItem(MenuItem menuItem)
        {
            if (ContextMenu == null)
                ContextMenu = new ContextMenu();

            if (ContextMenu.Items.Contains(menuItem))
                return this;

            ContextMenu.Items.Add(menuItem);

            return this;
        }

        public event Action<object> IconRemoved;

        public event Action<object> IconAdded;

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

            if (IconAdded != null)
                IconAdded(this);
        }

        public void DeleteIcon()
        {
            Win32Api.Shell_NotifyIcon(IconMessageType.NIM_DELETE, ref _iconData);

            if (IconRemoved != null)
                IconRemoved(this);
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
            _timer.Stop();
        }

        public void BlinkIcon()
        {
            InitializeTimer();
            _timer.Start();
        }

        private void InitializeTimer()
        {
            if (_timer != null)
                return;

            _timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(BLINK_INVERVAL_MILLISECONDS) };
            _timer.Tick += TimerOnTick;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (_iconData.hIcon == _startIcon.Handle)
            {
                _iconData.hIcon = _notifyIcon.Handle;
            }
            else if (_iconData.hIcon == _notifyIcon.Handle)
            {
                _iconData.hIcon = _startIcon.Handle;
            }

            Win32Api.Shell_NotifyIcon(IconMessageType.NIM_MODIFY, ref _iconData);
        }

        #region ParentTaskbarIcon

        /// <summary>
        /// An attached property that is assigned to 
        /// </summary>  
        public static readonly DependencyProperty ParentTaskbarIconProperty =
            DependencyProperty.RegisterAttached("ParentTaskbarIcon", typeof(WPFNotifyIcon), typeof(WPFNotifyIcon));

        /// <summary>
        /// Gets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static WPFNotifyIcon GetParentTaskbarIcon(DependencyObject d)
        {
            return (WPFNotifyIcon)d.GetValue(ParentTaskbarIconProperty);
        }

        /// <summary>
        /// Sets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetParentTaskbarIcon(DependencyObject d, WPFNotifyIcon value)
        {
            d.SetValue(ParentTaskbarIconProperty, value);
        }

        #endregion

        #region CustomBalloon

        /// <summary>
        /// CustomBalloon Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey CustomBalloonPropertyKey
            = DependencyProperty.RegisterReadOnly("CustomBalloon", typeof(Popup), typeof(WPFNotifyIcon),
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
