using System.Collections.Generic;
using System.Linq;
using Battles;
using Map;
using Targeting;
using Units;

namespace Cards.AdjustHealth
{
    public class AdjustHealthCard : Card
    {
        private readonly AdjustHealthCardConfig _config;
        private readonly UnitService _unitService;

        public AdjustHealthCard(AdjustHealthCardConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
        }

        public override void OnPlay(IEnumerable<ISelectable> targets)
        {
            var mapSpace = targets.FirstOrDefault() as MapSpace;
            _unitService.AdjustHealth(mapSpace?.Occupant, _config.Adjustment);
        }

        public override string Description => _config.Adjustment > 0
            ? $"Heal a unit by {_config.Adjustment}"
            : $"Deal {-_config.Adjustment} damage to a unit";

        public override ISelectContext SelectContext => new SelectContext<MapSpace>(
            new SelectContextConfig(TeamType.Player, SelectContextType.Map),
            mapSpace => mapSpace.Occupant != null);
    }
}