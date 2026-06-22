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
        private readonly SelectService _selectService;
        public event Action<IUnit> OnUnitDefeated;
        public event Action<TeamType, IUnit> OnActiveUnitChanged;
        public event Action<IUnit> OnUnitSpawned;
        private readonly Dictionary<TeamType, IUnit> _activeUnitByTeam = new();
        public IList<IUnit> Units { get; } = new List<IUnit>();
        
        public UnitService() : this(Locator.Get<MapService>(), Locator.Get<SelectService>())
        {
        }
        
        public UnitService(MapService mapService, SelectService selectService)
        {
            _mapService = mapService;
            _selectService = selectService;
        }

        public IUnit Spawn(BattleUnitConfig config, TeamType team)
        {
            var unit = new Unit
            {
                Team = team,
                Config = config.Unit,
                Actions = config.Unit.Actions?.Select(a => a.Action).ToList(),
                Energy = 10
            };

            ResetHealth(unit);
            AdjustEnergy(unit, 5);
            _mapService.Move(unit, config.StartQ, config.StartR);
            
            Units.Add(unit);
            OnUnitSpawned?.Invoke(unit);
            return unit;
        }

        public void Perform(IUnit unit, IAction action)
        {
            
            var unitSpace = _mapService.GetSpace(unit);
            var selectConfig = new SelectContextConfig(unit.Team, SelectContextType.Map);
            
            _selectService.RequestSelection<MapSpace>(
                selectConfig, 
                mapSpace => action.CanPerform(unitSpace, mapSpace),
                selectedMapSpaces => OnPerform(unit, action, unitSpace, selectedMapSpaces.First())
                );
        }

        private static void OnPerform(IUnit unit, IAction action, MapSpace userSpace, MapSpace targetSpace)
        {
            action.OnPerform(userSpace, targetSpace);
            AdjustEnergy(unit, -action.Config.EnergyCost);
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
            SetHealth(unit, unit.CurrentHealth - damage);
            if (unit.CurrentHealth <= 0)
            {
                OnUnitDefeated?.Invoke(unit);
            }
        }

        public static void AdjustEnergy(IUnit unit, int change)
        {
            if (change == 0)
            {
                return;
            }
            
            var energyBefore = unit.CurrentEnergy;
            unit.CurrentEnergy = Mathf.Clamp(unit.CurrentEnergy + change, 0, unit.Energy);
            if (unit.CurrentEnergy != energyBefore)
            {
                unit.OnCurrentEnergyChanged?.Invoke(unit.CurrentEnergy);
            }
        }
        
        private static void ResetHealth(IUnit unit)
        {
            SetHealth(unit, unit.Config.Health);
        }

        private static void SetHealth(IUnit unit, int health)
        {
            if (unit.CurrentHealth == health)
            {
                return;
            }
            
            unit.CurrentHealth = Mathf.Clamp(health, 0, unit.Config.Health);
            unit.OnCurrentHealthChanged?.Invoke(unit.CurrentHealth);
        }
    }
}