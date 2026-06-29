using System.Collections.Generic;
using System.Linq;
using Units;

namespace Map
{
    public class MapService
    {
        private readonly IList<MapSpace> _mapSpaces = new List<MapSpace>();
        private readonly Logger _logger = new(nameof(MapService));
        private readonly IDictionary<MapSpace, MapSpacePrefab> _prefabsBySpace = new Dictionary<MapSpace, MapSpacePrefab>();
        private readonly IDictionary<IUnit, IDictionary<MapSpace, IList<MapSpace>>> _reachableSpacesByUnit = new Dictionary<IUnit, IDictionary<MapSpace, IList<MapSpace>>>();
        
        public void Initialize(MapSpaceContainer mapSpaceContainer)
        {
            var mapSpacePrefabs = mapSpaceContainer.GetMapSpacePrefabs();
            foreach (var prefab in mapSpacePrefabs)
            {
                var space = new MapSpace(prefab.Q, prefab.R);
                prefab.Bind(space);
                _mapSpaces.Add(space);
                _prefabsBySpace.Add(space, prefab);
            }
        }

        public void Move(IUnit unit, int q, int r)
        {
            var newSpace = _mapSpaces.FirstOrDefault(m => m.Q == q && m.R == r);
            if (newSpace == null || newSpace.Occupant != null)
            {
                _logger.LogError($"Cannot move unit to map at q={q}, r={r} because no space exists there or already occupied.");
                return;
            }
            
            var oldSpace = GetSpace(unit);
            if (oldSpace != null)
            {
                oldSpace.Occupant = null;
            }

            newSpace.Occupant = unit;
            InvalidateReachableSpacesCache();
            unit.OnMapSpaceChanged?.Invoke(newSpace);
        }

        public MapSpacePrefab GetPrefab(MapSpace mapSpace)
        {
            if (_prefabsBySpace.TryGetValue(mapSpace, out var prefab))
            {
                return prefab;
            }
            
            _logger.LogError($"map space {mapSpace} not bound to any prefab");
            return null;
        }

        public MapSpace GetSpace(IUnit unit)
        {
            return _mapSpaces.FirstOrDefault(m => m.Occupant == unit);
        }

        public MapSpace GetSpace(int q, int r)
        {
            return _mapSpaces.FirstOrDefault(m => m.Q == q && m.R == r);
        }

        public void Remove(IUnit unit)
        {
            var space = GetSpace(unit);
            if (space != null)
            {
                space.Occupant = null;
                InvalidateReachableSpacesCache();
            }
        }

        public void InvalidateReachableSpacesCache(IUnit unit)
        {
            _reachableSpacesByUnit.Remove(unit);
        }

        public void InvalidateReachableSpacesCache()
        {
            _reachableSpacesByUnit.Clear();
        }
        
        public IList<MapSpace> GetShortestPath(
            MapSpace userSpace,
            MapSpace targetSpace,
            int maxDistance,
            bool includeStartSpace = false)
        {
            var reachablePaths = GetReachablePaths(userSpace, maxDistance);
            var pathFound = reachablePaths.TryGetValue(targetSpace, out var path);
            if (!pathFound || path.Count <= 0)
            {
                return new List<MapSpace>();
            }
            
            var shortestPath = new List<MapSpace>(path);
            if (!includeStartSpace)
            {
                shortestPath.RemoveAt(0);
            }
            
            return shortestPath;
        }
        
        public IDictionary<MapSpace, IList<MapSpace>> GetReachablePaths(
            MapSpace startSpace,
            int maxDistance)
        {
            if (startSpace?.Occupant == null || maxDistance <= 0)
            {
                return new Dictionary<MapSpace, IList<MapSpace>>();
            }

            if (_reachableSpacesByUnit.TryGetValue(startSpace.Occupant, out var cachedReachablePaths))
            {
                return cachedReachablePaths;
            }

            var reachablePaths = BuildReachablePaths(startSpace, maxDistance);
            _reachableSpacesByUnit[startSpace.Occupant] = reachablePaths;

            return reachablePaths;
        }
        
        public MapSpace GetClosestReachableSpace(IUnit unit, int q, int r, int maxDistance)
        {
            var startSpace = GetSpace(unit);
            if (startSpace == null)
            {
                return null;
            }

            var targetSpace = GetSpace(q, r);
            if (targetSpace == null)
            {
                return null;
            }
            
            var reachablePaths = GetReachablePaths(startSpace, maxDistance);
            if (reachablePaths.Count <= 0)
            {
                return null;
            }
            
            return reachablePaths
                .OrderBy(entry => entry.Key.GetDistanceTo(targetSpace))
                .ThenBy(entry => entry.Value.Count)
                .Select(entry => entry.Key)
                .FirstOrDefault();
        }
        
        public IList<MapSpace> GetAllEdgeSpaces()
        {
            return _mapSpaces.Where(space => GetNeighbors(space).Count() < 6).ToList();
        }
        
        public IEnumerable<MapSpace> GetNeighbors(MapSpace mapSpace)
        {
            var neighborOffsets = new[]
            {
                (-1, 0),
                (1, 0),
                (-1, 1),
                (0, 1),
                (0, -1),
                (1, -1)
            };

            foreach (var (offsetQ, offsetR) in neighborOffsets)
            {
                var neighbor = _mapSpaces.FirstOrDefault(m =>
                    m.Q == mapSpace.Q + offsetQ &&
                    m.R == mapSpace.R + offsetR);

                if (neighbor != null)
                {
                    yield return neighbor;
                }
            }
        }
        
        private IDictionary<MapSpace, IList<MapSpace>> BuildReachablePaths(
            MapSpace startSpace,
            int maxDistance)
        {
            var reachablePaths = new Dictionary<MapSpace, IList<MapSpace>>();

            if (startSpace == null)
            {
                return reachablePaths;
            }

            var visited = new HashSet<MapSpace> { startSpace };
            var cameFrom = new Dictionary<MapSpace, MapSpace>();
            var spacesToCheck = new Queue<(MapSpace space, int distance)>();
            spacesToCheck.Enqueue((startSpace, 0));

            while (spacesToCheck.Count > 0)
            {
                var (currentSpace, currentDistance) = spacesToCheck.Dequeue();

                foreach (var neighbor in GetNeighbors(currentSpace))
                {
                    if (visited.Contains(neighbor))
                    {
                        continue;
                    }

                    if (neighbor.Occupant != null)
                    {
                        continue;
                    }

                    var neighborDistance = currentDistance + 1;
                    if (neighborDistance > maxDistance)
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    cameFrom[neighbor] = currentSpace;
                    reachablePaths[neighbor] = BuildPath(startSpace, neighbor, cameFrom);
                    spacesToCheck.Enqueue((neighbor, neighborDistance));
                }
            }

            return reachablePaths;
        }
        
        private IList<MapSpace> BuildPath(
            MapSpace startSpace,
            MapSpace targetSpace,
            IDictionary<MapSpace, MapSpace> cameFrom)
        {
            var path = new List<MapSpace>();
            var currentSpace = targetSpace;

            path.Add(currentSpace);

            while (currentSpace != startSpace)
            {
                currentSpace = cameFrom[currentSpace];
                path.Add(currentSpace);
            }

            path.Reverse();
            return path;
        }
    }
}