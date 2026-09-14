using System.Collections.Generic;

namespace Units.KissOfDeath
{
    public class KissOfDeathActionPerformResult : ActionPerformResult
    {
        public IList<IUnit> UnitsHit { get; set; }
    }
}