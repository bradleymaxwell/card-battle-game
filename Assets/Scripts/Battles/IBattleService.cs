using System.Collections.Generic;
using Units;

namespace Battles
{
    public interface IBattleService
    {
        void Initialize(BattleConfig battleConfig, MapSpaceContainer mapSpaceContainer);
        IList<IUnit> GetTeamUnits(TeamType team);
        bool IsTurn(TeamType team);
        void StartNextTurn();
    }
}