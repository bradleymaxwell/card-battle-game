using System.Collections.Generic;
using Battles;

namespace Targeting
{
    public class TargetService
    {
        private readonly IDictionary<TeamType, ITargetable> _targetsByTeam = new Dictionary<TeamType, ITargetable>();
        
        public void UpdateTarget(ITargetable target, TeamType team)
        {
            if (_targetsByTeam.TryGetValue(team, out var oldTarget))
            {
                if (oldTarget == target)
                {
                    return;
                }
                
                oldTarget?.OnUntarget();
                _targetsByTeam.Remove(team);
            }
            
            if (target != null)
            {
                target.OnTarget();
                _targetsByTeam.Add(team, target);
            }           
        }
        
        public ITargetable GetTarget(TeamType team)
        {
            return _targetsByTeam.TryGetValue(team, out var target) ? target : null;
        }
    }
}