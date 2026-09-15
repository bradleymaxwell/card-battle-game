using System.Collections.Generic;

namespace Units.Actions
{
    public class AoEActionPerformResult : ActionPerformResult
    {
        public IList<IUnit> UnitsHit { get; set; }
    }
}