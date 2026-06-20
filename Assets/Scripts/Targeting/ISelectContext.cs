namespace Targeting
{
    public interface ISelectContext
    {
        bool CanSelect(ISelectable selectable);
        void AddToSelection(ISelectable selectable);
        int SelectionCount { get; }
        SelectContextConfig Config { get; }
        bool IsSelected(ISelectable selectable);
        void RemoveFromSelection(ISelectable selectable);
        void OnSelectionConfirmed();
        void DeselectAll();
    }
}