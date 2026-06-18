using Map;

namespace Units
{
    public class MoveAction : IAction
    {
        private readonly ActionConfig _config;
        private readonly MapService _mapService;
        
        public MoveAction(ActionConfig config) : this(config, Locator.Get<MapService>())
        {
        }

        public MoveAction(ActionConfig config, MapService mapService)
        {
            _config = config;
            _mapService = mapService;
        }
        
        public bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var hasEnergy = userSpace.Occupant.Energy - _config.EnergyCost >= 0;
            if (!hasEnergy)
            {
                return false;
            }

            if (targetSpace.Occupant != null)
            {
                return false;
            }

            return true;
        }

        public void OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            _mapService.Move(userSpace.Occupant, targetSpace.Q, targetSpace.R);
        }
    }
}