using UnityEngine;

namespace AI
{
    public abstract class UnitBrainConfig : ScriptableObject
    {
        public abstract IUnitBrain Brain { get; }
    }
}