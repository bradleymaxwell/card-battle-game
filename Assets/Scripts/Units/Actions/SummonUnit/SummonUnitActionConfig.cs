using AI;
using UnityEngine;

namespace Units.CreeperCupid
{
    [CreateAssetMenu(menuName = "Game Config/Action/Summon Unit", fileName =  "SummonUnitActionConfig")]
    public class SummonUnitActionConfig : ActionConfig, IRangedAction
    {
        [SerializeField] private UnitConfig unit;
        public UnitConfig Unit => unit;

        [SerializeField] private UnitBrainConfig brain;
        public UnitBrainConfig Brain => brain;

        [SerializeField] private int range;
        public int Range => range;
        
        public override IAction Action => new SummonUnitAction(this);
    }
}