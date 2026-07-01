using AI;
using UnityEngine;

namespace Units.CreeperCupid
{
    [CreateAssetMenu(menuName = "Game Config/Action/Summon Creeper Cupid", fileName =  "SummonCreeperCupidActionConfig")]
    public class SummonCreeperCupidActionConfig : ActionConfig
    {
        [SerializeField] private UnitConfig cupidUnit;
        public UnitConfig CupidUnit => cupidUnit;

        [SerializeField] private UnitBrainConfig cupidBrain;
        public UnitBrainConfig CupidBrain => cupidBrain;
        
        public override IAction Action => new SummonCreeperCupidAction(this);
    }
}