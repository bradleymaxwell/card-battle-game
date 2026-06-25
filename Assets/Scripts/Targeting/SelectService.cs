using System;
using System.Collections.Generic;
using Battles;

namespace Targeting
{
    public class SelectService
    {
        private readonly IDictionary<TeamType, ISelectContext> _activeContextByTeam = new Dictionary<TeamType, ISelectContext>();
        public event Action<TeamType, ISelectContext> OnActiveContextChanged;

        public void RequestSelection(ISelectContext context)
        {
            var activeContextFound = _activeContextByTeam.TryGetValue(context.Config.Team, out var activeContext);
            if (!context.Config.IsOverriding && activeContextFound)
            {
                return;
            }
            
            activeContext?.DeselectAll();
            _activeContextByTeam[context.Config.Team] = context;
            OnActiveContextChanged?.Invoke(context.Config.Team, context);
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
            OnActiveContextChanged?.Invoke(activeContext.Config.Team, null);
        }
    }
}