using System.Linq;
using Battles;
using Map;
using Units.Actions;

namespace Units.ToxicLovePotion
{
    public class ToxicLovePotionAction : Action
    {
        private readonly ToxicLovePotionActionConfig _config;
        private readonly UnitService _unitService;
        private readonly MapService _mapService;
        
        public ToxicLovePotionAction(ToxicLovePotionActionConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
            _mapService = Locator.Get<MapService>();
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new AoEActionPerformResult();
            var hitSpaces = _mapService.GetAreaSpaces(targetSpace, _config.Radius);
            result.UnitsHit = hitSpaces.Where(s => s.Occupant is { Team: TeamType.Player }).Select(s => s.Occupant).ToList();
            var damage = _config.Damage / result.UnitsHit.Count;
            foreach (var unit in result.UnitsHit)
            {
                _unitService.Damage(unit, damage);
            }

            return result;
        }
    }
}