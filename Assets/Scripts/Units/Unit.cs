using System;
using System.Collections.Generic;
using Battles;
using Map;

namespace Units
{
    public class Unit : IUnit
    {
        public IList<IAction> Actions { get; set; }
        public Action<int> OnCurrentHealthChanged { get; set; }
        public Action<MapSpace> OnMapSpaceChanged { get; set; }
        public TeamType Team { get; set; }
        public UnitConfig Config { get; set; }
        public int Energy { get; set; }
        public int CurrentHealth { get; set; }
        public System.Action OnSelect { get; set; }
        public System.Action OnDeselect { get; set; }
    }
}