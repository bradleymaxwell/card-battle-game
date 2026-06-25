using System.Collections.Generic;
using Targeting;

namespace Cards
{
    public interface ICard
    {
        bool CanPlay();
        void OnPlay(IEnumerable<ISelectable> targets);
        int ManaCost { get; }
        string Description { get; }
        CardConfig Config { get; }
        ISelectContext SelectContext { get; }
    }
}