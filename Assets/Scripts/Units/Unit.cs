using System;
using System.Collections.Generic;
using Map;

namespace Units
{
    public class Unit : IUnit
    {
        public IList<IAction> Actions { get; private set; }
        public Action<MapSpace> OnMapSpaceChanged { get; set; }
    }
}