using UnityEngine;

namespace AI.Passive
{
    [CreateAssetMenu(menuName = "Game Config/Brain/Passive")]
    public class PassiveUnitBrainConfig : UnitBrainConfig
    {
        public override IUnitBrain Brain => new PassiveUnitBrain();
    }
}