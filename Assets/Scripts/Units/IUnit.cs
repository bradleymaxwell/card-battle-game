using System;
using System.Collections.Generic;
using Map;

namespace Units
{
    public interface IUnit
    {
        IList<IAction> Actions { get; }
        Action<MapSpace> OnMapSpaceChanged { get; set; }
    }
}