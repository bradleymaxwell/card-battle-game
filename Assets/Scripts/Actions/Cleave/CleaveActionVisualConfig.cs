using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

[CreateAssetMenu(fileName = "CleaveActionVisualConfig", menuName = "Game Config/Action/Visual/Cleave")]
public class CleaveActionVisualConfig : ActionVisualConfig
{
    public override IActionVisualHandler Handler => new CleaveActionVisualHandler();
}
