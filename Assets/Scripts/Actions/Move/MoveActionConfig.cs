using UnityEngine;

namespace Units
{
    [CreateAssetMenu(menuName = "Game Config/Action/Move")]
    public class MoveActionConfig : ActionConfig, IMoveActionConfigProvider
    {
        public override IAction Action => new MoveAction(this);
    }
}