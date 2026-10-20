using Map;
using Units;
using Units.EnergySap;
using UnityEngine;

namespace Actions.EnergySap
{
    public class EnergySapAction : Action
    {
        private readonly EnergySapActionConfig _config;
        private readonly IUnitService _unitService;
        
        public EnergySapAction(EnergySapActionConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
        }

        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) 
                && targetSpace != userSpace 
                && targetSpace.Occupant != null
                && targetSpace.Occupant.Team != userSpace.Occupant.Team;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult();
            var sapAmount = _config.SapAmount;
            var healthPercent = (float)userSpace.Occupant.CurrentEnergy / userSpace.Occupant.Energy;
            if (healthPercent < _config.PerfectHitEnergyThreshold)
            {
                result.IsPerfectHit = true;
                sapAmount += _config.PerfectBonusSnapAmount;
            }
            
            var targetEnergyBefore = targetSpace.Occupant.CurrentEnergy;
            _unitService.AdjustEnergy(targetSpace.Occupant, -sapAmount);
            var energyChange = Mathf.Abs(targetSpace.Occupant.CurrentEnergy - targetEnergyBefore);
            _unitService.AdjustEnergy(userSpace.Occupant, energyChange);
            result.EnergyAdjustmentByUnit[targetSpace.Occupant] = -energyChange;
            result.EnergyAdjustmentByUnit[userSpace.Occupant] = energyChange;
            return result;
        }
    }
}