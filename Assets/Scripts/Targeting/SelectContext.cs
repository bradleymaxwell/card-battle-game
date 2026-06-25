using System;
using System.Collections.Generic;
using System.Linq;

namespace Targeting
{
    public class SelectContext : ISelectContext
    {
        private readonly IList<ISelectable> _selection = new List<ISelectable>();
        public SelectContextConfig Config { get; }

        private readonly Func<ISelectable, bool> _validateSelection;
        private readonly Action<IEnumerable<ISelectable>> _onSelectionConfirmed;
        public int SelectionCount => _selection.Count;
        
        public SelectContext(SelectContextConfig config, Func<ISelectable, bool> validateSelection, Action<IEnumerable<ISelectable>> onSelectionConfirmed)
        {
            Config = config;
            _validateSelection = validateSelection;
            _onSelectionConfirmed = onSelectionConfirmed;
        }

        public bool CanSelect(ISelectable selectable)
        {
            return _validateSelection(selectable);
        }

        public void AddToSelection(ISelectable selectable)
        {
            _selection.Add(selectable);
        }

        public void OnSelectionConfirmed()
        {
            _onSelectionConfirmed(_selection);
        }

        public void DeselectAll()
        {
            foreach (var selection in _selection.ToList())
            {
                selection.OnDeselect?.Invoke();
                _selection.Remove(selection);
            }
        }

        public bool IsSelected(ISelectable selectable)
        {
            return _selection.Contains(selectable);
        }

        public void RemoveFromSelection(ISelectable selectable)
        {
            _selection.Remove(selectable);
            selectable.OnDeselect?.Invoke();
        }
    }
    
    public class SelectContext<T> : ISelectContext where T : ISelectable
    {
        private readonly IList<T> _selection = new List<T>();
        public SelectContextConfig Config { get; }

        private readonly Func<T, bool> _validateSelection;
        private readonly Action<IEnumerable<T>> _onSelectionConfirmed;
        public int SelectionCount => _selection.Count;

        public SelectContext(SelectContextConfig config, Func<T, bool> validateSelection) : this(config,
            validateSelection, selections => { })
        {
            _onSelectionConfirmed = selections => { };
        }
        
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
                selection.OnDeselect?.Invoke();
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
            selectable.OnDeselect?.Invoke();
        }
    }
}