using Map;
using UnityEngine;

namespace Units
{
    public abstract class Action : IAction
    {
        private readonly ActionConfig _config;
        public Sprite Icon => _config.Icon;
        
        protected Action(ActionConfig config)
        {
            _config = config;
        }
        
        public virtual bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var hasEnergy = userSpace.Occupant.Energy - _config.EnergyCost >= 0;
            return hasEnergy;
        }

        public abstract void OnPerform(MapSpace userSpace, MapSpace targetSpace);
    }
}