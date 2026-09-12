namespace Units
{
    public class ActionPerformResult
    {
        public IAction Action { get; }
        public IUnit Performer { get; }
        public int EnergyConsumed { get; }
        
        public ActionPerformResult(IAction action, IUnit performer, int energyConsumed)
        {
            Action = action;
            Performer = performer;
            EnergyConsumed = energyConsumed;
        }
    }
}