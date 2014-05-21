using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MyNotifyIcon
{
    public class BalloonPool
    {
        public const int MAXIMUM_COUNT = 6;

        private PositionQueue _positionQueue;
        private readonly IList<Balloon> _balloons;
        private readonly int _timeOut;

        public BalloonPool(int timeOut)
        {
            _balloons = new List<Balloon>();
            _timeOut = timeOut;
        }

        public int Count { get { return _balloons.Count; } }

        public void ShowSingle(UserControl customBalloon)
        {
            var balloon = new Balloon(_timeOut, customBalloon);
            balloon.IsActivated += BalloonOnIsActivated;
            balloon.IsClosed += BalloonOnIsClosed;

            Position pos;
            lock (this)
            {
                InitializePositionQueue(customBalloon.Width, customBalloon.Height);
                pos = _positionQueue.ObtainPosition(balloon.Id);
                _balloons.Add(balloon);
            }

            balloon.Show(pos);
        }

        public void Clear()
        {
            while (_balloons.Count > 0)
            {
                _balloons[0].Close();
            }
        }

        private void InitializePositionQueue(double offsetX, double offsetY)
        {
            if (_positionQueue != null)
                return;

            _positionQueue = new PositionQueue(SystemParameters.PrimaryScreenWidth - offsetX, SystemParameters.PrimaryScreenHeight - offsetY, offsetY);
        }

        private void BalloonOnIsClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var balloon = sender as Balloon;
            lock (this)
            {
                _positionQueue.ReleasePosition(balloon.Id);
                _balloons.Remove(balloon);
            }
        }

        private void BalloonOnIsActivated(object sender, RoutedEventArgs routedEventArgs)
        {//TODO:
        }
    }
}