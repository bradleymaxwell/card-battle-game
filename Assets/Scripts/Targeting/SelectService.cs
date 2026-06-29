using System;
using System.Collections.Generic;
using Battles;
using DefaultNamespace;
using UnityEngine.InputSystem;

namespace Targeting
{
    public class SelectService : IDisposable
    {
        private readonly InputAction _cancelAction;
        private readonly IDictionary<TeamType, ISelectContext> _activeContextByTeam = new Dictionary<TeamType, ISelectContext>();
        public event Action<TeamType, ISelectContext> OnActiveContextChanged;

        public SelectService() : this(Locator.Get<InputService>())
        {
        }
        
        public SelectService(InputService inputService)
        {
            _cancelAction = inputService.GetAction(PlayerInputConstants.UI, PlayerInputConstants.Cancel);
            _cancelAction.performed += OnCancel;
        }

        public void RequestSelection(ISelectContext context)
        {
            var activeContextFound = _activeContextByTeam.TryGetValue(context.Config.Team, out var activeContext);
            if (activeContextFound)
            {
                if (!context.Config.IsOverriding)
                {
                    return;
                }
                
                Cancel(context.Config.Team, activeContext);
            }

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

        public void Cancel(TeamType team)
        {
            var activeContextFound = _activeContextByTeam.TryGetValue(team, out var activeContext);
            if (!activeContextFound)
            {
                return;
            }
            
            Cancel(team, activeContext);
        }

        private void Cancel(TeamType team, ISelectContext context)
        {
            context?.DeselectAll();
            _activeContextByTeam.Remove(team);
            OnActiveContextChanged?.Invoke(team, null);
        }
        
        public void Dispose()
        {
            if (_cancelAction != null)
            {
                _cancelAction.performed -= OnCancel;
            }
        }
        
        private void ConfirmSelection(ISelectContext activeContext)
        {
            activeContext.OnSelectionConfirmed();
            activeContext.DeselectAll();
            _activeContextByTeam.Remove(activeContext.Config.Team);
            OnActiveContextChanged?.Invoke(activeContext.Config.Team, null);
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            Cancel(TeamType.Player);
        }
    }
}