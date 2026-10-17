using System.Linq;

namespace Units
{
    public static class IUnitExtensions
    {
        public static BasicAttackAction GetBasicAttack(this IUnit unit)
        {
            var basicAttack = unit.Actions.FirstOrDefault(a => a is BasicAttackAction);
            return (BasicAttackAction)basicAttack;
        }
    }
}
