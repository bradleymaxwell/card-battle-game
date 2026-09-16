using Battles;
using Map;

namespace Units.CreeperCupid
{
    public class SummonCreeperCupidAction : Action
    {
        private readonly SummonCreeperCupidActionConfig _config;
        private readonly UnitService _unitService;
        private readonly MapService _mapService;
        
        public SummonCreeperCupidAction(SummonCreeperCupidActionConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
            _mapService = Locator.Get<MapService>();
        }

        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) && targetSpace.Occupant == null;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult();
            var config = new SpawnConfig(_config.CupidUnit, targetSpace.Q, targetSpace.R, TeamType.Enemy)
            {
                BrainConfig = _config.CupidBrain
            };
            
            var unit = _unitService.Spawn(config);
            var space = _mapService.GetSpace(unit);
            result.SpawnSpaceByUnit[unit] = space;
            return result;
        }
    }
}