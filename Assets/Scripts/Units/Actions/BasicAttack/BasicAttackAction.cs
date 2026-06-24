using Map;

namespace Units
{
    public class BasicAttackAction : Action
    {
        private readonly UnitService _unitService;
        private readonly BasicAttackActionConfig _config;
        
        public BasicAttackAction(BasicAttackActionConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
        }

        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) 
                   && userSpace != targetSpace 
                   && targetSpace.Occupant != null
                   && targetSpace.Occupant.Team != userSpace.Occupant.Team;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult(_config.EnergyCost);
            _unitService.Damage(targetSpace.Occupant, userSpace.Occupant.Config.Attack);
            return result;
        }
    }
}