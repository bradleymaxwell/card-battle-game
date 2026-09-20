using Units;

namespace AI.Passive
{
    public class PassiveUnitBrain : IUnitBrain
    {
        private IUnit _unit;
        
        public void Initialize(IUnit unit)
        {
            _unit = unit;
        }

        public UnitTurnIntention GetTurnIntention()
        {
            var intention = new UnitTurnIntention()
            {
                Description = $"{_unit.Config.Name} doesn't seem to do anything..."
            };

            return intention;
        }
    }
}