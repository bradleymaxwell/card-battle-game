using System.Collections.Generic;
using System.Linq;
using Map;
using Units.Actions;

namespace Units.KissOfDeath
{
    public class DetonateAction : Action
    {
        private readonly DetonateActionConfig _config;
        private readonly MapService _mapService;
        private readonly UnitService _unitService;
        
        public DetonateAction(DetonateActionConfig config) : base(config)
        {
            _config = config;
            _mapService = Locator.Get<MapService>();
            _unitService = Locator.Get<UnitService>();
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult();
            var nearbySpaces = _mapService.GetAreaSpaces(userSpace, _config.Radius);
            var otherTeamSpaces = nearbySpaces.Where(s => s.Occupant != null && s.Occupant.Team != userSpace.Occupant.Team);
            foreach (var space in otherTeamSpaces)
           {
                var target = space.Occupant;
                _unitService.Damage(target, _config.Damage);
                result.HealthAdjustmentByUnit[target] = -_config.Damage;
            }
            
            _unitService.Eliminate(userSpace.Occupant);
            return result;
        }
    }
}