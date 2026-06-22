using Map;

namespace Units
{
    public class BasicAttackAction : Action
    {
        private readonly UnitService _unitService;
        
        public BasicAttackAction(BasicAttackActionConfig config) : base(config)
        {
            _unitService = Locator.Get<UnitService>();
        }

        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) 
                   && userSpace != targetSpace 
                   && targetSpace.Occupant != null
                   && targetSpace.Occupant.Team != userSpace.Occupant.Team;
        }

        public override void OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            _unitService.Damage(targetSpace.Occupant, userSpace.Occupant.Config.Attack);
        }
    }
}