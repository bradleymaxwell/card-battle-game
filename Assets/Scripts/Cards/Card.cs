using Battles;

namespace Cards
{
    public abstract class Card : ICard
    {
        public CardConfig Config { get; }
        public int ManaCost => Config.ManaCost;
        private  readonly CardService _cardService;
        protected Card(CardConfig config)
        {
            Config = config;
            _cardService = Locator.Get<CardService>();
        }

        public virtual bool CanPlay(TeamType team)
        {
            var deck = _cardService.GetDeck(team);
            return deck?.CurrentMana >= ManaCost;
        }

        public abstract void OnPlay(TeamType team);
        public abstract string Description { get; }
    }
}