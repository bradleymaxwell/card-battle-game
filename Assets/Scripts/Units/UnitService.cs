using System;
using System.Collections.Generic;
using System.Linq;
using Battles;
using Map;
using Targeting;
using UnityEngine;

namespace Units
{
    public class UnitService
    {
        private readonly Logger _logger = new(nameof(UnitService));
        private readonly MapService _mapService;
        private readonly PoolService _poolService;
        private readonly SelectService _selectService;
        public event Action<IUnit> OnUnitDefeated;
        public event Action<TeamType, IUnit> OnActiveUnitChanged;
        private readonly Dictionary<TeamType, IUnit> _activeUnitByTeam = new Dictionary<TeamType, IUnit>();
        
        public UnitService() : this(Locator.Get<MapService>(), Locator.Get<PoolService>(), Locator.Get<SelectService>())
        {
        }
        
        public UnitService(MapService mapService, PoolService poolService, SelectService selectService)
        {
            _mapService = mapService;
            _poolService = poolService;
            _selectService = selectService;
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

        public void Perform(IUnit unit, IAction action)
        {
            
            var unitSpace = _mapService.GetSpace(unit);
            var selectConfig = new SelectContextConfig
            {
                Team = unit.Team,
                RequiredAmount = 1
            };
            
            _selectService.RequestSelection<MapSpace>(
                selectConfig, 
                mapSpace => action.CanPerform(unitSpace, mapSpace),
                selectedMapSpaces => action.OnPerform(unitSpace, selectedMapSpaces.First())
                );
        }

        public void SetActiveUnit(TeamType team, IUnit unit)
        {
            if (_activeUnitByTeam.TryGetValue(team, out var previousUnit))
            {
                if (previousUnit == unit)
                {
                    return;
                }
            }
            
            _activeUnitByTeam[team] = unit;
            OnActiveUnitChanged?.Invoke(team, unit);
        }

        public void DeactivateUnit(TeamType team)
        {
            _activeUnitByTeam.Remove(team);
            OnActiveUnitChanged?.Invoke(team, null);
        }

        public IUnit GetActiveUnit(TeamType team)
        {
            return _activeUnitByTeam.GetValueOrDefault(team);
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