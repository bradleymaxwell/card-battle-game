using Map;
using UnityEngine;

namespace Units
{
    public abstract class Action : IAction
    {
        public IActionConfigProvider Config { get; }
        public Sprite Icon => Config.Icon;
        public int Charges { get; set; }
        public Action OnChargesChanged { get; set; }

        protected Action(IActionConfigProvider config)
        {
            Config = config;
        }
        
        public virtual bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            if (Config.ChargesCap != null && Charges <= 0)
            {
                return false;
            }
            
            var hasEnergy = userSpace.Occupant.CurrentEnergy - GetEnergyCost(userSpace, targetSpace) >= 0;
            var inRange = true;
            if (Config is IRangedAction rangedAction && rangedAction.Range > 0)
            {
                inRange = userSpace.GetDistanceTo(targetSpace) <= rangedAction.Range;
            }
            
            return hasEnergy && inRange;
        }
        
        public virtual int GetEnergyCost(MapSpace userSpace, MapSpace targetSpace)
        {
            return Config.EnergyCost;
        }

        public abstract ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace);
    }
}