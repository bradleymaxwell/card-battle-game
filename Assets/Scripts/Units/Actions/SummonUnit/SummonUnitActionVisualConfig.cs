using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonCreeperCupidActionVisualConfig", menuName = "Game Config/Action/Visual/Summon Creeper Cupid")]
public class SummonUnitActionVisualConfig : ActionVisualConfig
{
    public override IActionVisualHandler Handler => new SummonUnitActionVisualHandler();
}
