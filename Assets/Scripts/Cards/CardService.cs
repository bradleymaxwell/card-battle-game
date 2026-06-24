using System.Collections.Generic;
using Battles;
using Cards;
using UnityEngine;

public class CardService
{
    private readonly Dictionary<TeamType, Deck> _decks = new();
    private readonly Logger _logger = new(nameof(CardService));
    
    public void Initialize(DeckConfig playerDeckConfig, DeckConfig enemyDeckConfig)
    {
        _decks.Clear();
        var playerDeck = CreateDeck(playerDeckConfig);
        var enemyDeck = CreateDeck(enemyDeckConfig);
        Draw(playerDeck, 4);
        Draw(enemyDeck, 4);
    }

    public Deck GetDeck(TeamType team)
    {
        var found = _decks.TryGetValue(team, out var deck);
        if (!found)
        {
            _logger.LogError($"Deck for team {team} not found");
            return null;
        }
        
        return deck;
    }

    public void Draw(TeamType team, int count)
    {
        var deck = GetDeck(team);
        Draw(deck, count);
    }

    private void Draw(Deck deck, int count)
    {
        var drawPileFound = deck.CardPiles.TryGetValue(CardPileType.Draw, out var drawPile);
        if (!drawPileFound || drawPile.Count <= 0)
        {
            _logger.LogError($"No cards left to draw");
            return;
        }
        
        var cardsToDraw = Mathf.Min(count, drawPile.Count);
        for (var i = 0; i < cardsToDraw; i++)
        {
            var card = drawPile[0];
            drawPile.RemoveAt(0);
            deck.CardPiles[CardPileType.Hand].Add(card);
        }
    }

    private IList<ICard> GetCards(DeckConfig config)
    {
        var cards = new List<ICard>();
        foreach (var entry in config.Entries)
        {
            if (entry.Count > 0)
            {
                for (var i = 0; i < entry.Count; i++)
                {
                    var card = entry.Card.Card;
                    cards.Add(card);
                }
            }
        }
        
        return cards;
    }

    private static void Shuffle(IList<ICard> cards)
    {
        for (var i = cards.Count - 1; i > 0; i--)
        {
            var randomIndex = Random.Range(0, i + 1);

            (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
        }
    }

    private Deck CreateDeck(DeckConfig config)
    {
        var cards = GetCards(config);
        Shuffle(cards);
        var deck = new Deck
        {
            CardPiles =
            {
                [CardPileType.Draw] = cards,
                [CardPileType.Hand] = new List<ICard>(),
                [CardPileType.Discard] = new List<ICard>()
            }
        };
        
        _decks[TeamType.Player] = deck;
        return deck;
    }
}
