using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(menuName = "Game Config/Action/Visual/Basic Attack")]
    public class BasicAttackActionVisualHandlerConfig : ActionVisualConfig
    {
        public override IActionVisualHandler Handler => new BasicAttackActionVisualHandler();
    }
}