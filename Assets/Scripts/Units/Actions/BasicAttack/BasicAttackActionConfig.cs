using UnityEngine;

namespace Units
{
    [CreateAssetMenu(fileName = "BasicAttackActionConfig", menuName = "Game Config/Action/Basic Attack")]
    public class BasicAttackActionConfig : ActionConfig, IRangedAction
    {
        [SerializeField] private int range;
        public int Range => range;
        
        public override IAction Action => new BasicAttackAction(this);
    }
}