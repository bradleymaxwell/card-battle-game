using AI;
using Units;

public interface IUnitBrain
{
    void Initialize(IUnit unit);
    UnitTurnIntention GetTurnIntention();
}
