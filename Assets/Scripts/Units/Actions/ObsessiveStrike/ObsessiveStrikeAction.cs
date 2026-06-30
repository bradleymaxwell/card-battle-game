using System.Linq;
using Map;
using UnityEngine;

namespace Units.ObsessiveStrike
{
    public class ObsessiveStrikeAction : Action
    {
        private readonly ObsessiveStrikeActionConfig _config;
        private int _stacks;
        private IUnit _previousTarget;
        private readonly UnitService _unitService;
        
        public ObsessiveStrikeAction(ObsessiveStrikeActionConfig config) : base(config)
        {
            _config = config;
            _unitService = Locator.Get<UnitService>();
        }
        
        public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            return base.CanPerform(userSpace, targetSpace) && targetSpace.Occupant != null;
        }

        public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
        {
            var result = new ActionPerformResult(_config.EnergyCost);
            if (_previousTarget != targetSpace.Occupant)
            {
                _stacks = 0;
                _previousTarget = targetSpace.Occupant;
            }
            
            var damage = GetDamage();
            _unitService.Damage(_previousTarget, damage);
            _stacks = Mathf.Min(_stacks + 1, _config.PercentIncreasePerStack.Count);
            return result;
        }

        private int GetDamage()
        {
            if (_stacks == 0)
            {
                return _config.BaseDamage;
            }

            var increase = _config.PercentIncreasePerStack.ElementAtOrDefault(_stacks - 1);
            var damage = _config.BaseDamage * (1 + increase);
            return Mathf.FloorToInt(damage);
        }
    }
}