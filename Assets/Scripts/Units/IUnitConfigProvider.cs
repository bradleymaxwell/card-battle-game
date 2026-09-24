using System.Collections.Generic;

namespace Units
{
    public interface IUnitConfigProvider
    {
        string Name { get; }
        UnitPrefab Prefab { get; }
        int Attack { get; }
        int Health { get; }
        IReadOnlyList<ActionConfig> Actions { get; }
    }
}