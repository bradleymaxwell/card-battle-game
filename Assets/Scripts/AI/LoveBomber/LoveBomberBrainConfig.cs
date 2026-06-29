using UnityEngine;

namespace AI.LoveBomber
{
    [CreateAssetMenu(menuName = "Game Config/Brain/Love Bomber")]
    public class LoveBomberBrainConfig : UnitBrainConfig
    {
        [SerializeField] private float motherOfAllLoveBombsThreshold;
        public float MotherOfAllLoveBombsThreshold => motherOfAllLoveBombsThreshold;
        
        public override IUnitBrain Brain => new LoveBomberBrain(this);
    }
}