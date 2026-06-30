using System.Linq;
using Map;

namespace Units.KissOfDeath
{
    public class KissOfDeathAction : Action
    {
        private readonly KissOfDeathActionConfig _config;
        private readonly MapService _mapService;
        private readonly UnitService _unitService;
        
        public KissOfDeathAction(KissOfDeathActionConfig config) : base(config)
        {
            _config = config;
            _mapService = Locator.Get<MapService>();
            _unitService = Locator.Get<UnitService>();
        }
        
        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) && targetSpace.Occupant != null && userSpace.Occupant.Team != targetSpace.Occupant.Team;
        } 

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult(_config.EnergyCost);
            var nearbySpaces = _mapService.GetAreaSpaces(targetSpace, _config.Radius);
            var otherTeamSpaces = nearbySpaces.Where(s => s.Occupant != null && s.Occupant.Team != userSpace.Occupant.Team);
            foreach (var space in otherTeamSpaces)
            {
                _unitService.Damage(space.Occupant, _config.Damage);
            }
            
            _unitService.Eliminate(userSpace.Occupant);
            return result;
        }
    }
}