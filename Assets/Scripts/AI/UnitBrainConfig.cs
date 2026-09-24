using UnityEngine;

namespace AI
{
    public abstract class UnitBrainConfig : ScriptableObject, IUnitBrainConfigProvider
    {
        public abstract IUnitBrain Brain { get; }
    }
}