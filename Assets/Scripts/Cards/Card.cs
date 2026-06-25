using System.Collections.Generic;
using Battles;
using Targeting;

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

        public bool CanPlay()
        {
            var deck = _cardService.GetDeck();
            return deck?.CurrentMana >= ManaCost;
        }

        public abstract void OnPlay(IEnumerable<ISelectable> targets);
        public abstract string Description { get; }
        public abstract ISelectContext SelectContext { get; }
    }
}