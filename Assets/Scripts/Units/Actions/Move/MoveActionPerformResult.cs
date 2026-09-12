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
            IList<MapSpace> path) : base(
            action,
            performer,
            energyConsumed)
        {
            Path = path;
        }
    }
}