using System;
using System.Collections.Generic;
using Battles;
using Map;

namespace Units
{
    public class Unit : IUnit
    {
        public IList<IAction> Actions { get; private set; }
        public Action<MapSpace> OnMapSpaceChanged { get; set; }
        public TeamType Team { get; set; }
        public UnitConfig Config { get; set; }
        public int CurrentHealth { get; set; }
    }
}