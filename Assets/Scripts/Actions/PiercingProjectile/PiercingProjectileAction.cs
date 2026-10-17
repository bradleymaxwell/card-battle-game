using Map;
using UnityEngine;

namespace Units.PiercingProjectile
{
    public class PiercingProjectileAction : Action
    {
        private readonly PiercingProjectileActionConfig _config;
        private readonly IMapService _mapService;
        private readonly IUnitService _unitService;
        
        public PiercingProjectileAction(PiercingProjectileActionConfig config) : this(config, Locator.Get<MapService>(), Locator.Get<UnitService>())
        {
        }

        public PiercingProjectileAction(PiercingProjectileActionConfig config, IMapService mapService, IUnitService unitService) : base(config)
        {
            _mapService = mapService;
            _unitService = unitService;
            _config = config;
        }

        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) 
                   && userSpace != targetSpace
                   && targetSpace.IsOnSameDirectionalAxisAs(userSpace) 
                   && userSpace.GetDistanceTo(targetSpace) <= _config.TravelDistance;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult();
            var user = userSpace?.Occupant;
            if (user == null)
            {
                return result;
            }
            
            result.ProjectileTravelPath = _mapService.GetDirectionPath(userSpace, targetSpace, false, _config.TravelDistance);
            var targetsHit = 0;
            foreach (var space in result.ProjectileTravelPath)
            {
                var target = space.Occupant;
                if (target == null || target.Team == user.Team)
                {
                    continue;
                }
                
                var damage = Mathf.CeilToInt(user.Config.Attack * _config.DamagePercent);
                _unitService.Damage(space.Occupant, damage);
                targetsHit++;
                result.HealthAdjustmentByUnit.Add(target, -damage);
            }

            if (targetsHit >= _config.PerfectHitThreshold)
            {
                result.IsPerfectHit = true;
                var basicAttack = user.GetBasicAttack();
                if (basicAttack != null)
                {
                    _unitService.AdjustActionCharges(basicAttack, 1);
                }
            }
            
            return result;
        }
    }
}