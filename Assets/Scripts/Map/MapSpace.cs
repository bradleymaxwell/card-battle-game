using Units;

namespace Map
{
    public class MapSpace
    {
        public int Q { get; }
        public int R { get; }
        public IUnit Occupant { get; private set; }
        private readonly Logger _logger = new(nameof(MapSpace));
        
        public MapSpace(int q, int r)
        {
            Q = q;
            R = r;
        }

        public void SetOccupant(IUnit unit)
        {
            if (Occupant != null)
            {
                _logger.LogError($"Occupant is already set to {Occupant}");
                return;
            }
            
            Occupant = unit;
        }
    }
}