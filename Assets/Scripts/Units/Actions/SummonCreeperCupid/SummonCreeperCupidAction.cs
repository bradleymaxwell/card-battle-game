using Battles;
using Map;

namespace Units.CreeperCupid
{
    public class SummonCreeperCupidAction : Action
    {
        private readonly SummonCreeperCupidActionConfig _config;
        private readonly UnitService _unitService;
        
        public SummonCreeperCupidAction(SummonCreeperCupidActionConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
        }

        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) && targetSpace.Occupant == null;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult(_config.EnergyCost);
            var config = new SpawnConfig(_config.CupidUnit, targetSpace.Q, targetSpace.R, TeamType.Enemy)
            {
                BrainConfig = _config.CupidBrain
            };
            
            _unitService.Spawn(config);
            return result;
        }
    }
}