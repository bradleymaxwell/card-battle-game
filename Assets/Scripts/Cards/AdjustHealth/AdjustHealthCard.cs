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
        private readonly SelectService _selectService;
        private readonly UnitService _unitService;

        public AdjustHealthCard(AdjustHealthCardConfig config) : base(config)
        {
            _config = config;
            _selectService = Locator.Get<SelectService>();
            _unitService = Locator.Get<UnitService>();
        }

        public override void OnPlay(TeamType team)
        {
            var selectConfig = new SelectContextConfig(team, SelectContextType.Map);
            _selectService.RequestSelection<MapSpace>(
                selectConfig,
                mapSpace => mapSpace.Occupant != null,
                mapSpaces => _unitService.AdjustHealth(mapSpaces.FirstOrDefault()?.Occupant, _config.Adjustment)
            );
        }

        public override string Description => _config.Adjustment > 0
            ? $"Heal a unit by {_config.Adjustment}"
            : $"Deal {_config.Adjustment} damage to a unit";
    }
}