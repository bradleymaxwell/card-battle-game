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
            var canPerform = base.CanPerform(userSpace, targetSpace) && targetSpace.Occupant == null;
            return canPerform;
        }

        public override void OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            _mapService.Move(userSpace.Occupant, targetSpace.Q, targetSpace.R);
        }
    }
}