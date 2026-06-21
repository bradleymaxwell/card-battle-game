using System;
using System.Collections.Generic;
using Battles;

namespace Targeting
{
    public class SelectService
    {
        private readonly IDictionary<TeamType, ISelectContext> _activeContextByTeam = new Dictionary<TeamType, ISelectContext>();
        
        public void RequestSelection<T>(SelectContextConfig contextConfig, Func<T, bool> validateSelection, Action<IEnumerable<T>> onSelectionConfirmed) where T : ISelectable
        {
            var activeContextFound = _activeContextByTeam.TryGetValue(contextConfig.Team, out var activeContext);
            if (!contextConfig.IsOverriding && activeContextFound)
            {
                return;
            }
            
            var context = new SelectContext<T>(contextConfig, validateSelection, onSelectionConfirmed);
            activeContext?.DeselectAll();
            _activeContextByTeam[contextConfig.Team] = context;
        }

        public void Select(ISelectable selectable, TeamType team)
        {
            if (!_activeContextByTeam.TryGetValue(team, out var activeContext))
            {
                return;
            }

            if (activeContext.SelectionCount >= activeContext.Config.RequiredAmount)
            {
                return;
            }

            if (activeContext.IsSelected(selectable) && !activeContext.Config.AllowDuplicates)
            {
                return;
            }

            if (!activeContext.CanSelect(selectable))
            {
                return;
            }
            
            activeContext.AddToSelection(selectable);
            selectable.OnSelect?.Invoke();
            
            if (activeContext.SelectionCount >= activeContext.Config.RequiredAmount && activeContext.Config.AutoConfirm)
            {
                ConfirmSelection(activeContext);
            }
        }
        
        public void Deselect(ISelectable selectable, TeamType team)
        {
            if (!_activeContextByTeam.TryGetValue(team, out var activeContext))
            {
                return;
            }

            if (!activeContext.IsSelected(selectable))
            {
                return;
            }
            
            activeContext.RemoveFromSelection(selectable);
            selectable.OnDeselect?.Invoke();
        }
        
        private void ConfirmSelection(ISelectContext activeContext)
        {
            activeContext.OnSelectionConfirmed();
            activeContext.DeselectAll();
            _activeContextByTeam.Remove(activeContext.Config.Team);
        }
    }
}