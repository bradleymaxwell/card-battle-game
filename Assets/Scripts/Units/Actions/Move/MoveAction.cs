using Map;

namespace Units
{
    public class MoveAction : Action
    {
        private readonly MapService _mapService;
        
        public MoveAction(ActionConfig config) : this(config, Locator.Get<MapService>())
        {
        }

        public MoveAction(ActionConfig config, MapService mapService) : base(config)
        {
            _mapService = mapService;
        }
        
        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            if (targetSpace.Occupant != null || !base.CanPerform(userSpace, targetSpace))
            {
                return false;
            }
            
            var maxDistance = Config.EnergyCost != 0 ? userSpace.Occupant.CurrentEnergy / Config.EnergyCost : int.MaxValue;
            if (maxDistance <= 0)
            {
                return false;
            }
            
            var shortestPath = _mapService.GetShortestPath(userSpace, targetSpace, maxDistance, includeStartSpace: false);
            if (shortestPath == null || shortestPath.Count == 0)
            {
                return false;
            }
            
            var hasEnoughEnergy = shortestPath.Count * Config.EnergyCost <= userSpace.Occupant.CurrentEnergy;
            return hasEnoughEnergy;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new MoveActionPerformResult();
            var maxDistance = Config.EnergyCost != 0 ? userSpace.Occupant.CurrentEnergy / Config.EnergyCost : int.MaxValue;
            var shortestPath = _mapService.GetShortestPath(userSpace, targetSpace, maxDistance, includeStartSpace: false);
            result.Path = shortestPath;
            _mapService.Move(userSpace.Occupant, targetSpace.Q, targetSpace.R);
            return result;
        }
    }
}