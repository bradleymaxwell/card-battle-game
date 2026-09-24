namespace AI.LoveBomber
{
    public interface ILoveBomberBrainConfigProvider
    {
        float MotherOfAllLoveBombsThreshold { get; }
        int GaslightExplosivesPerTurn { get; }
        int GaslightExplosiveSearchRadius { get; }
    }
}