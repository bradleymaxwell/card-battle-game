using Battles;
using Map;
using UnityEngine;

namespace Units
{
    public class UnitService
    {
        private readonly Logger _logger = new(nameof(UnitService));
        private readonly MapService _mapService;
        private readonly PoolService _poolService;
 
        public UnitService() : this(Locator.Get<MapService>(), Locator.Get<PoolService>())
        {
        }
        
        public UnitService(MapService mapService, PoolService poolService)
        {
            _mapService = mapService;
            _poolService = poolService;
        }
        
        public void Perform(IUnit unit, IAction action)
        {
            if (!unit.Actions.Contains(action))
            {
                _logger.LogError($"{action} is not part of {unit}'s actions and cannot be performed");
                return;
            }

            if (!action.CanPerform())
            {
                _logger.LogError($"{action} cannot perform {unit}'s action");
                return;
            }
            
            action.OnPerform();
        }

        public IUnit Spawn(BattleUnitConfig config)
        {
            var unitPrefab = _poolService.Get(config.UnitPrefab);
            var unit = new Unit();
            unitPrefab.Bind(unit);
            _mapService.Set(unit, config.StartQ, config.StartR);
            return unit;
        }
    }
}