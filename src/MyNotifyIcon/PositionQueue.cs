using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNotifyIcon
{
    internal class PositionQueue
    {
        private readonly double _origX;
        private readonly double _origY;
        private readonly double _offsetY;

        public PositionQueue(Point origin, double offsetY)
        {
            _origX = origin.X;
            _origY = origin.Y;
            _offsetY = offsetY;
            Positions = new List<Position>();
        }

        public IList<Position> Positions { get; private set; }

        public int Count
        {
            get { return Positions.Count; }
        }

        public void ReleasePositionAndClearPositionsIfAllAreFree(string id)
        {
            ReleasePosition(id);
            ClearPositions();
        }

        public Position OccupyPosition(string id)
        {
            var position = ObtainAndAppendPosition();
            OccupyPosition(position, id);
            return position;
        }

        private bool ExistPosition(string id)
        {
            return Positions.Any(pos => String.Equals(pos.Id, id) && pos.IsOccupied);
        }

        private bool AllPositionsAreFree()
        {
            return Positions.All(pos => !pos.IsOccupied);
        }

        private void ClearPositions()
        {
            if (AllPositionsAreFree())
                Positions.Clear();
        }

        private void ReleasePosition(string id)
        {
            if (ExistPosition(id))
                ReleasePosition(Positions.FirstOrDefault(p => String.Equals(p.Id, id)));
        }

        private void ReleasePosition(Position position)
        {
            position.IsOccupied = false;
            position.Id = String.Empty;
        }

        private Position ObtainAndAppendPosition()
        {
            var position = new Position();

            if (IsEmptyPositionQueue())
            {
                position = ObtainFirstNewPosition();
                Positions.Add(position);
            }
            else if (AllPositionsAreOccupied())
            {
                position = ObtainLastNewPosition();
                Positions.Add(position);
            }
            else if (ExistFreePosition())
                position = ObtainFirstFreePosition();

            return position;
        }

        private Position ObtainFirstFreePosition()
        {
            return Positions.FirstOrDefault(p => !p.IsOccupied);
        }

        private Position ObtainFirstNewPosition()
        {
            return new Position { OffsetX = _origX, OffsetY = _origY };
        }

        private Position ObtainLastNewPosition()
        {
            return new Position { OffsetX = _origX, OffsetY = _origY - _offsetY * Positions.Count };
        }

        private void OccupyPosition(Position position, string id)
        {
            position.Id = id;
            position.IsOccupied = true;
        }

        private bool IsEmptyPositionQueue()
        {
            return Positions.Count == 0;
        }

        private bool AllPositionsAreOccupied()
        {
            return Positions.All(p => p.IsOccupied);
        }

        private bool ExistFreePosition()
        {
            return Positions.Any(pos => !pos.IsOccupied);
        }
    }
}