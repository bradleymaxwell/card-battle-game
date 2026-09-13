using System.Collections.Generic;
using Map;

namespace Units
{
    public class MoveActionPerformResult : ActionPerformResult
    {
        public IList<MapSpace> Path { get; }
        
        public MoveActionPerformResult(
            IAction action, 
            IUnit performer,
            int energyConsumed,
            MapSpace targetSpace,
            IList<MapSpace> path) : base(
            action,
            performer,
            energyConsumed,
            targetSpace)
        {
            Path = path;
        }
    }
}