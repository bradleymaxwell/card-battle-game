using Map;
using UnityEngine;

namespace Units
{
    public interface IAction
    {
        bool CanPerform(MapSpace userSpace, MapSpace targetSpace);
        void OnPerform(MapSpace userSpace, MapSpace targetSpace);
        Sprite Icon { get; }
        ActionConfig Config { get; }
    }
}