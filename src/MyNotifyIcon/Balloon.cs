using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MyNotifyIcon
{
    internal class Balloon : Popup
    {
        private readonly DispatcherTimer _timer;

        public string Id { get; private set; }

        public Balloon(int timeout, UIElement balloon)
        {
            Child = balloon;

            SubscribeCloseEvent();

            Id = Guid.NewGuid().ToString();

            _timer = new DispatcherTimer(DispatcherPriority.SystemIdle) { Interval = TimeSpan.FromSeconds(timeout) };
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        public void Close()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Action action = Close;
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                return;
            }
            lock (this)
            {
                IsOpen = false;
                StaysOpen = false;

                UnsubscribeCloseEvent();
            }

            RaiseIsClosedEvent();
        }

        public void Show(Position pos)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                var action = new Action<Position>(Show);
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                return;
            }

            lock (this)
            {
                AllowsTransparency = true;
                PopupAnimation = PopupAnimation.Slide;

                HorizontalOffset = pos.OffsetX;
                VerticalOffset = pos.OffsetY;

                IsOpen = true;
                StaysOpen = true;
            }
            
            RaiseIsActivatedEvent();
        }

        public event RoutedEventHandler IsActivated;

        public event RoutedEventHandler IsClosed;

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void SubscribeCloseEvent()
        {
            var fb = Child as IBalloon;
            if (fb != null)
                fb.Closing += Close;
        }

        private void UnsubscribeCloseEvent()
        {
            var fb = Child as IBalloon;
            if (fb != null)
                fb.Closing -= Close;
        }

        private void RaiseIsClosedEvent()
        {
            if (IsClosed != null)
                IsClosed(this, new RoutedEventArgs());
        }

        private void RaiseIsActivatedEvent()
        {
            if (IsActivated != null)
                IsActivated(this, new RoutedEventArgs());
        }
    }
}