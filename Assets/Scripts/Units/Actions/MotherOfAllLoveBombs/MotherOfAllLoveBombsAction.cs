using System.Linq;
using Map;
using Targeting;
using Units.KissOfDeath;

namespace Units.MotherOfAllLoveBombs
{
    public class MotherOfAllLoveBombsAction : Action
    {
        private readonly MotherOfAllLoveBombsActionConfig _config;
        private readonly MapService _mapService;
        private readonly UnitService _unitService;
        private readonly SelectService _selectService;
        private readonly Logger _logger;
        
        public MotherOfAllLoveBombsAction(MotherOfAllLoveBombsActionConfig config) : base(config)
        {
            _config = config;
            _mapService = Locator.Get<MapService>();
            _unitService = Locator.Get<UnitService>();
            _selectService = Locator.Get<SelectService>();
            _logger = new Logger(nameof(MotherOfAllLoveBombsAction));
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult();
            var spaces = _mapService.GetAllMapSpaces();
            foreach (var space in spaces)
            {
                if (space.Occupant == null || space.Occupant == userSpace.Occupant)
                {
                    continue;
                }

                if (space.Occupant.Team != userSpace.Occupant.Team)
                {
                    _unitService.Damage(space.Occupant, _config.Damage);
                    result.HealthAdjustmentByUnit[space.Occupant] = -_config.Damage;
                    continue;
                }

                if (space.Occupant.Config != _config.GaslightExplosiveConfig)
                {
                    continue;
                }
                
                var detonateAction = space.Occupant.Actions.FirstOrDefault(a => a is DetonateAction);
                if (detonateAction == null)
                {
                    _logger.LogError($"could not find detonate action for {space.Occupant.Config.Name}, which needs to be triggered by the M.O.A.L.B");
                    continue;
                }
                    
                _unitService.Perform(space.Occupant, detonateAction);
                _selectService.Select(space, space.Occupant.Team);
            }

            return result;
        }
    }
}