using System.Collections.Generic;
using System.Linq;
using Battles;
using Map;
using Targeting;
using Units;
using UnityEngine;

namespace Cards.AdjustEnergy
{
    public class AdjustEnergyCard : Card
    {
        private readonly AdjustEnergyCardConfig _config;
        private readonly UnitService _unitService;
        
        public AdjustEnergyCard(AdjustEnergyCardConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
        }

        public override void OnPlay(IEnumerable<ISelectable> targets)
        {
            var mapSpace = targets?.FirstOrDefault() as MapSpace;
            _unitService.AdjustEnergy(mapSpace?.Occupant, _config.Adjustment);
        }

        public override string Description => $"{(_config.Adjustment > 0 ? "Increase": "Decrease")} a unit's energy by {Mathf.Abs(_config.Adjustment)}";

        public override ISelectContext SelectContext => new SelectContext<MapSpace>(
            new SelectContextConfig(TeamType.Player, SelectContextType.Map),
            mapSpace => mapSpace.Occupant != null);
    }
}