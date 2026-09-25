using System.Collections.Generic;
using Map;

namespace Units
{
    public class MoveActionPerformResult : ActionPerformResult
    {
        public IList<MapSpace> Path { get; set; }
    }
}