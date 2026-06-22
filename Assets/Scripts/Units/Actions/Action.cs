using Map;
using UnityEngine;

namespace Units
{
    public abstract class Action : IAction
    {
        public ActionConfig Config { get; }
        public Sprite Icon => Config.Icon;
        
        protected Action(ActionConfig config)
        {
            Config = config;
        }
        
        public virtual bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var hasEnergy = userSpace.Occupant.CurrentEnergy - Config.EnergyCost >= 0;
            var inRange = true;
            if (Config is IRangedAction rangedAction)
            {
                inRange = userSpace.GetDistanceTo(targetSpace) <= rangedAction.Range;
            }
            
            return hasEnergy && inRange;
        }

        public abstract void OnPerform(MapSpace userSpace, MapSpace targetSpace);
    }
}