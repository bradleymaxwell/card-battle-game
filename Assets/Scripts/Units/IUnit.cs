using System;
using Battles;
using Map;

namespace Units
{
    public interface IUnit
    {
        Action<MapSpace> OnMapSpaceChanged { get; set; }
        TeamType Team { get; }
        int CurrentHealth { get; set; }
        UnitConfig Config { get; }
    }
}