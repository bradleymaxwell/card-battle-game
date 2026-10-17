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
            var q = other.Q - Q;
            var r = other.R - R;
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

        public bool IsOnSameDirectionalAxisAs(MapSpace other)
        {
            if (other == null)
            {
                return false;
            }
            
            return Q == other.Q || R == other.R || (Q + R == other.Q + other.R);
        }
        
        public (int q, int r) GetDirectionStep(MapSpace targetSpace)
        {
            var qDifference = targetSpace.Q - Q;
            var rDifference = targetSpace.R - R;
            if (rDifference == 0)
            {
                return qDifference > 0 ? (1, 0) : (-1, 0);
            }

            if (qDifference == 0)
            {
                return rDifference > 0 ? (0, 1) : (0, -1);
            }

            if (qDifference + rDifference == 0)
            {
                return qDifference > 0 ? (1, -1) : (-1, 1);
            }

            return (0, 0);
        }
    }
}