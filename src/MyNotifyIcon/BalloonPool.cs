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
        private int _timeOut;

        public BalloonPool(int timeOut)
        {
            _balloons = new List<Balloon>();
            UpdateTimeOut(timeOut);
        }

        public int Count { get { return _balloons.Count; } }

        public void UpdateTimeOut(int timeOut)
        {
            _timeOut = timeOut;
        }

        public void ShowSingle(UserControl customBalloon)
        {
            var balloon = new Balloon(_timeOut, customBalloon);
            balloon.IsActivated += BalloonOnIsActivated;
            balloon.IsClosed += BalloonOnIsClosed;

            Position pos;
            lock (this)
            {
                InitializePositionQueue(customBalloon.Width, customBalloon.Height);
                pos = _positionQueue.OccupyPosition(balloon.Id);
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

            var origin = new Point()
                {
                    X = (int)(SystemParameters.PrimaryScreenWidth - offsetX),
                    Y = (int)(SystemParameters.PrimaryScreenHeight - offsetY)
                };

            _positionQueue = new PositionQueue(origin, offsetY);
        }

        private void BalloonOnIsClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var balloon = sender as Balloon;
            lock (this)
            {
                _positionQueue.ReleasePositionAndClearPositionsIfAllAreFree(balloon.Id);
                _balloons.Remove(balloon);
            }
        }

        private void BalloonOnIsActivated(object sender, RoutedEventArgs routedEventArgs)
        {//TODO:
        }
    }
}