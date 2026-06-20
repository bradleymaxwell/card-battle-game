using System;
using System.Collections.Generic;
using Battles;
using UnityEngine;

namespace Targeting
{
    public class SelectService
    {
        private readonly IDictionary<TeamType, ISelectContext> _activeContextByTeam = new Dictionary<TeamType, ISelectContext>();
        
        public void RequestSelection<T>(SelectContextConfig contextConfig, Func<T, bool> validateSelection, Action<IEnumerable<T>> onSelectionConfirmed) where T : ISelectable
        {
            if (!contextConfig.IsOverriding && _activeContextByTeam.ContainsKey(contextConfig.Team))
            {
                return;
            }
            
            var context = new SelectContext<T>(contextConfig, validateSelection, onSelectionConfirmed);
            _activeContextByTeam[contextConfig.Team] = context;
            Debug.Log($"selection count: {context.SelectionCount}");
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
            
            Debug.Log($"selection count: {activeContext.SelectionCount}");
            activeContext.AddToSelection(selectable);
            selectable.OnSelect?.Invoke();
            
            Debug.Log($"selection count: {activeContext.SelectionCount}");
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