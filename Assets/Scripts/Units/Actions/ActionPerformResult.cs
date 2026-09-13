using Map;

namespace Units
{
    public class ActionPerformResult
    {
        public IAction Action { get; }
        public IUnit Performer { get; }
        public int EnergyConsumed { get; }
        public MapSpace Target { get; }
        
        public ActionPerformResult(
            IAction action, 
            IUnit performer, 
            int energyConsumed,
            MapSpace target)
        {
            Action = action;
            Performer = performer;
            EnergyConsumed = energyConsumed;
            Target = target;
        }
    }
}