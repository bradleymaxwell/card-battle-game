using System;
using System.Collections.Generic;
using Battles;
using Map;
using Targeting;

namespace Units
{
    public interface IUnit : ISelectable
    {
        Action<MapSpace> OnMapSpaceChanged { get; set; }
        TeamType Team { get; }
        int CurrentHealth { get; set; }
        UnitConfig Config { get; }
        int CurrentEnergy { get; set; }
        int Energy { get; }
        IList<IAction> Actions { get; }
        Action<int> OnCurrentHealthChanged { get; set; }
        Action<int> OnCurrentEnergyChanged { get; set; }
    }
}