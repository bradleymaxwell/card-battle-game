using Map;
using UnityEngine;

namespace Units
{
    public interface IAction
    {
        bool CanPerform(MapSpace userSpace, MapSpace targetSpace);
        ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace);
        Sprite Icon { get; }
        IActionConfigProvider Config { get; }
        int GetEnergyCost(MapSpace userSpace, MapSpace targetSpace);
    }
}