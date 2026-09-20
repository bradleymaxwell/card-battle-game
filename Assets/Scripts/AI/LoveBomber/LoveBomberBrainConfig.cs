using UnityEngine;

namespace AI.LoveBomber
{
    [CreateAssetMenu(menuName = "Game Config/Brain/Love Bomber")]
    public class LoveBomberBrainConfig : UnitBrainConfig
    {
        [SerializeField] private float motherOfAllLoveBombsThreshold;
        public float MotherOfAllLoveBombsThreshold => motherOfAllLoveBombsThreshold;

        [SerializeField] private int gaslightExplosivesPerTurn;
        public int GaslightExplosivesPerTurn => gaslightExplosivesPerTurn;

        [SerializeField] private int gaslightExplosiveSearchRadius;
        public int GaslightExplosiveSearchRadius => gaslightExplosiveSearchRadius;
        
        public override IUnitBrain Brain => new LoveBomberBrain(this);
    }
}