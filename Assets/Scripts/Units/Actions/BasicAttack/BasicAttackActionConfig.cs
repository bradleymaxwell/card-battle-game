using UnityEngine;

namespace Units
{
    [CreateAssetMenu(fileName = "BasicAttackActionConfig", menuName = "Game Config/Action/Basic Attack")]
    public class BasicAttackActionConfig : ActionConfig
    {
        public override IAction Action => new BasicAttackAction(this);
    }
}