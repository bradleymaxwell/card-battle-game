using UnityEngine;

namespace Units
{
    [CreateAssetMenu(menuName = "Game Config/Action/Move")]
    public class MoveActionConfig : ActionConfig
    {
        public override IAction Action => new MoveAction(this);
    }
}