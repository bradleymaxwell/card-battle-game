using Battles;

namespace Cards
{
    public interface ICard
    {
        bool CanPlay(TeamType team);
        void OnPlay(TeamType team);
        int ManaCost { get; }
        string Description { get; }
        CardConfig Config { get; }
    }
}