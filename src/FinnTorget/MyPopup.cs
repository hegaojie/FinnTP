using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace FinnTorget
{
    public delegate void CloseButtonEventHandler();

    public delegate void ClickTextEventHandler();

    public class MyPopup : Popup
    {
        private readonly DispatcherTimer _timer;

        public string Id { get; private set; }

        public MyPopup(int timeout, FancyBalloon balloon)
        {
            Child = balloon;

            SubscribeCloseEvent();

            Id = Guid.NewGuid().ToString();

            _timer = new DispatcherTimer(DispatcherPriority.SystemIdle) { Interval = TimeSpan.FromDays(1) };
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void SubscribeCloseEvent()
        {
            var fb = Child as FancyBalloon;
            if (fb != null)
                fb.CloseButtonPressed += Close;
        }

        private void UnsubscribeCloseEvent()
        {
            var fb = Child as FancyBalloon;
            if (fb != null)
                fb.CloseButtonPressed -= Close;
        }

        private void Close()
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
                //Placement = PlacementMode.AbsolutePoint;
                


                HorizontalOffset = pos.OffsetX;
                VerticalOffset = pos.OffsetY;

                //assign this instance as an attached property
                //TODO:SetParentTaskbarIcon(balloon, this);
                //fire attached event
                //TODO:RaiseBalloonShowingEvent(balloon, this);

                IsOpen = true;
                StaysOpen = true;
            }

            RaiseIsActivatedEvent();
        }

        public event RoutedEventHandler IsActivated;

        public event RoutedEventHandler IsClosed;

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