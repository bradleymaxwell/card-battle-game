using System.Linq;
using Map;

namespace Units.MotherOfAllLoveBombs
{
    public class MotherOfAllLoveBombsAction : Action
    {
        private readonly MotherOfAllLoveBombsActionConfig _config;
        private readonly MapService _mapService;
        private readonly UnitService _unitService;
        
        public MotherOfAllLoveBombsAction(MotherOfAllLoveBombsActionConfig config) : base(config)
        {
            _config = config;
            _mapService = Locator.Get<MapService>();
            _unitService = Locator.Get<UnitService>();
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult(_config.EnergyCost);
            var hitSpaces = _mapService.GetAreaSpaces(userSpace, _config.Radius, includeCenterSpace: false);
            var otherTeamSpaces = hitSpaces.Where(s => s.Occupant != null && s.Occupant.Team != userSpace.Occupant.Team);
            foreach (var space in otherTeamSpaces)
            {
                _unitService.Damage(space.Occupant, _config.Damage);
            }

            return result;
        }
    }
}