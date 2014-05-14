using System.Collections.Generic;
using System.Linq;

namespace FinnTorget
{
    public class PositionQueue
    {
        private readonly double _origX;
        private readonly double _origY;
        private readonly double _offsetY;

        public PositionQueue(double origX, double origY, double offsetY)
        {
            _origX = origX;
            _origY = origY;
            _offsetY = offsetY;
            Positions = new List<Position>();
        }

        public IList<Position> Positions { get; private set; } 

        public int Count
        {
            get { return Positions.Count; }
        }

        public void ReleasePosition(string id)
        {
            var pos = Positions.FirstOrDefault(p => string.Equals(p.Id, id));
            if (pos != null)
            {
                pos.IsOccupied = false;
                pos.Id = string.Empty;
            }

            if (Positions.All(p => !p.IsOccupied))
                Positions.Clear();
        }

        public Position ObtainPosition(string id)
        {
            if (Positions.Count == 0)
            {
                var newPos = new Position
                {
                    Id = id,
                    OffsetX = _origX,
                    OffsetY = _origY,
                    IsOccupied = true
                };
                Positions.Add(newPos);
                return newPos;
            }

            if (Positions.All(p => p.IsOccupied))
            {
                var newPos = new Position
                {
                    Id = id,
                    OffsetX = _origX,
                    OffsetY = _origY - _offsetY * Positions.Count,
                    IsOccupied = true
                };
                Positions.Add(newPos);
                return newPos;
            }

            if (Positions.Any(p => !p.IsOccupied))
            {
                var pos = Positions.FirstOrDefault(p => !p.IsOccupied);
                pos.Id = id;
                pos.IsOccupied = true;
                return pos;
            }

            return new Position();
        }
    }
}