namespace Units
{
    public class ActionPerformResult
    {
        public int EnergyConsumed { get; set; }

        public ActionPerformResult(int energyConsumed)
        {
            EnergyConsumed = energyConsumed;
        }
    }
}