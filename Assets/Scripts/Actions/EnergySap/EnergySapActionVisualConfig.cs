using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

namespace Actions.EnergySap
{
    [CreateAssetMenu(menuName = "Game Config/Action/Visual/Energy Sap")]
    public class EnergySapActionVisualConfig : ActionVisualConfig
    {
        public override IActionVisualHandler Handler => new EnergySapActionVisualHandler();
    }
}