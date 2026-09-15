using System.Collections.Generic;
using Map;

namespace Units
{
    public class ActionPerformResult
    {
        public IAction Action { get; set; }
        public IUnit Performer { get; set; }
        public int EnergyConsumed { get; set; }
        public MapSpace TargetSpace { get; set; }
        public IUnit Target { get; set; }
        public Dictionary<IUnit, int> HealthAdjustmentByUnit { get; } = new();
    }
}