using System;
using Targeting;
using Units;
using Action = System.Action;

namespace Map
{
    public class MapSpace : ISelectable
    {
        public int Q { get; }
        public int R { get; }
        public IUnit Occupant { get; set; }
        
        public MapSpace(int q, int r)
        {
            Q = q;
            R = r;
        }

        public Action OnSelect { get; set; }
        public Action OnDeselect { get; set; }

        public int GetDistanceTo(MapSpace other)
        {
            var qDistance = Math.Abs(Q - other.Q);
            var rDistance = Math.Abs(R - other.R);
            var sDistance = Math.Abs((-Q - R) - (-other.Q - other.R));

            return (qDistance + rDistance + sDistance) / 2;
        }
        
        public bool IsNeighbourOf(MapSpace other)
        {
            return GetDistanceTo(other) == 1;
        }

        public MapDirection GetDirectionTo(MapSpace other)
        {
            var q = Q - other.Q;
            var r = R - other.R;
            return (q, r) switch
            {
                (-1, 0) => MapDirection.Left,
                (1, 0) => MapDirection.Right,
                (-1, 1) => MapDirection.LeftUp,
                (0, 1) => MapDirection.RightUp,
                (0, -1) => MapDirection.LeftDown,
                (1, -1) => MapDirection.RightDown,
                _ => MapDirection.None
            };
        }
    }
}