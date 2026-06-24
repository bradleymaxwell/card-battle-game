using System.Collections.Generic;

namespace Cards
{
    public class Deck
    {
        public IDictionary<CardPileType, IList<ICard>> CardPiles { get; } = new Dictionary<CardPileType, IList<ICard>>();
        public int CurrentMana { get; set; }
    }
}