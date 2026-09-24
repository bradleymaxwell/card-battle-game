using Battles;

namespace Targeting
{
    public interface ISelectService
    {
        void RequestSelection(ISelectContext context);
        void Select(ISelectable selectable, TeamType team);
        void Deselect(ISelectable selectable, TeamType team);
        void Cancel(TeamType team);
    }
}