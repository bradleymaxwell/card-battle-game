using System;
using System.Collections;
using Units;
using Units.Actions.VisualPlayback;

namespace Actions.EnergySap
{
    public class EnergySapActionVisualHandler : IActionVisualHandler
    {
        private readonly UnitViewManager _unitViewManager;

        public EnergySapActionVisualHandler()
        {
            _unitViewManager = Locator.Get<UnitViewManager>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            var (_, userResourceBar) = _unitViewManager.GetUnitViews(result.Performer);
            userResourceBar.AdjustCurrentEnergy(result.EnergyAdjustmentByUnit[result.Performer]);
            var (_, targetResourceBar) = _unitViewManager.GetUnitViews(result.Target);
            targetResourceBar.AdjustCurrentEnergy(result.EnergyAdjustmentByUnit[result.Target]);
            yield break;
        }
    }
}