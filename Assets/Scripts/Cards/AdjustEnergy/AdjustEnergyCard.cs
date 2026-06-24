using System.Linq;
using Battles;
using Map;
using Targeting;
using Units;

namespace Cards.AdjustEnergy
{
    public class AdjustEnergyCard : Card
    {
        private readonly AdjustEnergyCardConfig _config;
        private readonly SelectService _selectService;
        private readonly UnitService _unitService;
        
        public AdjustEnergyCard(AdjustEnergyCardConfig config) : base(config)
        {
            _config = config;
        }

        public override void OnPlay(TeamType team)
        {
            var selectConfig = new SelectContextConfig(team, SelectContextType.Map);
            _selectService.RequestSelection<MapSpace>(
                selectConfig,
                mapSpace => mapSpace.Occupant != null,
                mapSpaces => _unitService.AdjustEnergy(mapSpaces.FirstOrDefault()?.Occupant, _config.Adjustment));
        }

        public override string Description => $"{(_config.Adjustment > 0 ? "Increase": "Decrease")} a unit's energy by {_config.Adjustment}";
    }
}