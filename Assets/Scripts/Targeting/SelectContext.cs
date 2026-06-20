using System;
using System.Collections.Generic;
using System.Linq;

namespace Targeting
{
    public class SelectContext<T> : ISelectContext where T : ISelectable
    {
        private IList<T> _selection = new List<T>();
        public SelectContextConfig Config { get; }

        private readonly Func<T, bool> _validateSelection;
        private readonly Action<IEnumerable<T>> _onSelectionConfirmed;
        public int SelectionCount => _selection.Count;
        
        public SelectContext(SelectContextConfig config, Func<T, bool> validateSelection, Action<IEnumerable<T>> onSelectionConfirmed)
        {
            Config = config;
            _validateSelection = validateSelection;
            _onSelectionConfirmed = onSelectionConfirmed;
        }

        public bool CanSelect(ISelectable selectable)
        {
            if (selectable is not T castedSelectable)
            {
                return false;
            }
            
            return _validateSelection(castedSelectable);
        }

        public void AddToSelection(ISelectable selectable)
        {
            _selection.Add((T)selectable);
        }

        public void OnSelectionConfirmed()
        {
            _onSelectionConfirmed(_selection);
        }

        public void DeselectAll()
        {
            foreach (var selection in _selection.ToList())
            {
                selection?.OnDeselect();
                _selection.Remove(selection);
            }
        }

        public bool IsSelected(ISelectable selectable)
        {
            return _selection.Contains((T)selectable);
        }

        public void RemoveFromSelection(ISelectable selectable)
        {
            _selection.Remove((T)selectable);
            selectable.OnDeselect();
        }
    }
}