using System.Collections.Generic;

namespace Units.MotherOfAllLoveBombs
{
    public class MotherOfAllLoveBombsActionResult : ActionPerformResult
    {
        public IList<ActionPerformResult> DetonationResults { get; } = new List<ActionPerformResult>();
    }
}