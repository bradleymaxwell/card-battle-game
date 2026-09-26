using System.Collections.Generic;

namespace Battles
{
    public interface IBattleConfigProvider
    {
        IReadOnlyList<BattleUnitConfig> PlayerUnits { get; }
        IReadOnlyList<BattleUnitConfig> EnemyUnits { get; }
    }
}