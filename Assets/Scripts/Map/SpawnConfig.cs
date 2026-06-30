using AI;
using Battles;
using Units;

namespace Map
{
    public class SpawnConfig
    {
        public UnitConfig UnitConfig { get; }
        public int Q { get; }
        public int R { get; }
        public TeamType Team { get; }
        public UnitBrainConfig BrainConfig { get; set; }
        
        public SpawnConfig(UnitConfig unitConfig, int q, int r, TeamType team)
        {
            UnitConfig = unitConfig;
            Q = q;
            R = r;
            Team = team;
        }
    }
}