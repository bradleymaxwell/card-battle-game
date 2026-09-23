using Units.VisualPlayback;
using UnityEngine;

namespace Units
{
    public interface IActionConfigProvider
    {
        Sprite Icon { get; } 
        int EnergyCost { get; }
        string Description { get; }
        ActionVisualConfig Visual { get; }
        string Name { get; }
    }
}