using System.Linq;
using Battles;
using Map;

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
            var result = new ActionPerformResult(_config.EnergyCost);
            var hitSpaces = _mapService.GetAreaSpaces(targetSpace, _config.Radius);
            var hitPlayerSpaces = hitSpaces.Where(s => s.Occupant is { Team: TeamType.Player }).ToList();
            var damage = _config.Damage / hitPlayerSpaces.Count;
            foreach (var space in hitPlayerSpaces)
            {
                _unitService.Damage(space.Occupant, damage);
            }

            return result;
        }
    }
}