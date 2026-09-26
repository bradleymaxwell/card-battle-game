using System;
using Battles;
using Map;

namespace Units
{
    public interface IUnitService
    {
        IUnit Spawn(SpawnConfig config);
        void ToggleActions(bool allowed);
        void Perform(IUnit unit, IAction action, bool isSubAction = false, Action<ActionPerformResult> onPerformed = null);
        void SetActiveUnit(TeamType team, IUnit unit);
        void DeactivateUnit(TeamType team);
        IUnit GetActiveUnit(TeamType team);
        void Damage(IUnit unit, int damage);
        void AdjustEnergy(IUnit unit, int change);
        void AdjustHealth(IUnit unit, int change);
        void Eliminate(IUnit unit);
        event Action<IUnit> OnUnitDefeated;
        event Action<IUnit> OnUnitSpawned;
    }
}