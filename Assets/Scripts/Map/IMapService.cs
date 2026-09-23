using System.Collections.Generic;
using Units;

namespace Map
{
    public interface IMapService
    {
        public IList<MapSpace> GetShortestPath(MapSpace userSpace, MapSpace targetSpace, int maxDistance, bool includeStartSpace = false);
        void Move(IUnit unit, int q, int r);
    }
}