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

        public void Set(IUnit unit, int q, int r)
        {
            var space = _mapSpaces.FirstOrDefault(m => m.Q == q && m.R == r);
            if (space == null || space.Occupant != null)
            {
                _logger.LogError($"Cannot add unit to map at q={q}, r={r} because no space exists there or already occupied.");
                return;
            }
            
            space.SetOccupant(unit);
            unit.OnMapSpaceChanged?.Invoke(space);
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
    }
}