using UnityEngine;

namespace AI.CreeperCupid
{
    [CreateAssetMenu(menuName = "Game Config/Brain/Creeper Cupid")]
    public class CreeperCupidBrainConfig : UnitBrainConfig
    {
        public override IUnitBrain Brain => new CreeperCupidBrain();
    }
}