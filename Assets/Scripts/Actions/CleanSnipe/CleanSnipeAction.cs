using System.Collections.Generic;
using System.Linq;
using Map;
using UnityEngine;

namespace Units.CleanSnipe
{
    public class CleanSnipeAction : Action
    {
        private readonly CleanSnipeActionConfig _config;
        private readonly IMapService _mapService;
        private IList<MapSpace> _path;
        private readonly IUnitService _unitService;
        
        public CleanSnipeAction(CleanSnipeActionConfig config) : base(config)
        {
            _config = config;
            _mapService = Locator.Get<MapService>();
            _unitService = Locator.Get<UnitService>();
        }

        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            _path = _mapService.GetDirectionPath(userSpace, targetSpace, false);
            return base.CanPerform(userSpace, targetSpace) 
                   && targetSpace.Occupant != null 
                   && targetSpace.Occupant.Team != userSpace.Occupant.Team
                   && targetSpace.IsOnSameDirectionalAxisAs(userSpace)
                   && !_path.Any(s => s.Occupant != null && s != targetSpace);
        }
        
        public override int GetEnergyCost(MapSpace userSpace, MapSpace targetSpace)
        {
            var energyCost = base.GetEnergyCost(userSpace, targetSpace);
            if (energyCost == 0)
            {
                return energyCost;
            }

            if (_path.Count >= _config.PerfectHitTravelThreshold)
            {
                energyCost /= 2;
            }
            
            return energyCost;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult
            {
                ProjectileTravelPath = _path
            };
            
            if (_path.Count >= _config.PerfectHitTravelThreshold)
            {
                result.IsPerfectHit = true;
            }
            
            var damage = userSpace.Occupant.Config.Attack * Mathf.CeilToInt(_config.BaseDamagePercent + (_config.PercentIncreasePerSpace * _path.Count));
            var target = targetSpace.Occupant;
            _unitService.Damage(target, damage);
            result.HealthAdjustmentByUnit[target] = -damage;
            return result;
        }
    }
}