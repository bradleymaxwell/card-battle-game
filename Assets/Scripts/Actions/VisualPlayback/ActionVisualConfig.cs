using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units.VisualPlayback
{
    public abstract class ActionVisualConfig : ScriptableObject
    {
        public abstract IActionVisualHandler Handler { get; }
    }
}