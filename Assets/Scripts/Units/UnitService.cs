using System;
using System.Linq;
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
        public event Action<IUnit> OnUnitDefeated;
        
        public UnitService() : this(Locator.Get<MapService>(), Locator.Get<PoolService>())
        {
        }
        
        public UnitService(MapService mapService, PoolService poolService)
        {
            _mapService = mapService;
            _poolService = poolService;
        }

        public IUnit Spawn(BattleUnitConfig config, TeamType team)
        {
            var unitPrefab = _poolService.Get(config.Unit.Prefab);
            var unit = new Unit
            {
                Team = team,
                Config = config.Unit,
                Actions = config.Unit.Actions?.Select(a => a.Action).ToList()
            };
            
            ResetHealth(unit);
            unitPrefab.Bind(unit);
            _mapService.Move(unit, config.StartQ, config.StartR);
            return unit;
        }

        public void Perform(IUnit unit, IAction action, MapSpace targetSpace)
        {
            var unitSpace = _mapService.GetSpace(unit);
            if (action.CanPerform(unitSpace, targetSpace))
            {
                action.OnPerform(unitSpace, targetSpace);
            }
        }

        public void Damage(IUnit unit, int damage)
        {
            unit.CurrentHealth = Mathf.Max(0, unit.CurrentHealth - damage);
            if (unit.CurrentHealth <= 0)
            {
                OnUnitDefeated?.Invoke(unit);
            }
        }

        private static void ResetHealth(IUnit unit)
        {
            unit.CurrentHealth = unit.Config.Health;
        }
    }
}