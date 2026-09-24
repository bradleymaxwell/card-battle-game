using System.Collections.Generic;
using Units;

namespace Map
{
    public interface IMapService
    {
        public IList<MapSpace> GetShortestPath(MapSpace userSpace, MapSpace targetSpace, int maxDistance, bool includeStartSpace = false);
        void Move(IUnit unit, int q, int r);
        MapSpace GetSpace(IUnit unit);
        MapSpace GetClosestReachableNeighborSpace(IUnit unit, int q, int r, int maxDistance);
        IList<MapSpace> GetAllEdgeSpaces();
        IEnumerable<MapSpace> GetNeighbors(MapSpace mapSpace);
        MapSpace GetClosestReachableSpace(IUnit unit, int q, int r, int maxDistance);
        MapSpace GetSpace(int q, int r);
        IList<MapSpace> GetAreaSpaces(MapSpace centerSpace, int radius, bool includeCenterSpace = true);
    }
}