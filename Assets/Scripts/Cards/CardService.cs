using System;
using System.Collections.Generic;
using Battles;
using Cards;
using Targeting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardService
{
    private readonly Dictionary<TeamType, Deck> _decks = new();
    private readonly Logger _logger = new(nameof(CardService));
    private SelectService _selectService;
    public event Action<ICard> OnSelectedCardChanged;
    public event Action<ICard> OnCardDrawn;
    public event Action<int> OnManaChanged;
    public event Action<ICard, CardPileType, CardPileType> OnCardMoved;

    public CardService() : this(Locator.Get<SelectService>())
    {
    }
    
    public CardService(SelectService selectService)
    {
        _selectService = selectService;
    }
    
    public void Initialize(DeckConfig playerDeckConfig)
    {
        _decks.Clear();
        var playerDeck = CreateDeck(playerDeckConfig);
        Draw(playerDeck, 4);
    }

    public Deck GetDeck()
    {
        _decks.TryGetValue(TeamType.Player, out var deck);
        return deck;
    }

    public void Draw(int count)
    {
        var deck = GetDeck();
        Draw(deck, count);
    }

    public void Select(ICard card)
    {
        if (card == null || !card.CanPlay())
        {
            return;
        }
        
        var deck = GetDeck();
        if (deck == null)
        {
            return;
        }
        
        var context = new SelectContext(
            card.SelectContext.Config, 
            selection => card.SelectContext.CanSelect(selection),
            selections =>
            {
                OnPlay(card, deck, selections);
                OnSelectedCardChanged?.Invoke(null);
            });
        
        _selectService.RequestSelection(context);
        OnSelectedCardChanged?.Invoke(card);
    }

    private void OnPlay(ICard card, Deck deck, IEnumerable<ISelectable> targets)
    {
        AdjustMana(deck, -card.ManaCost);
        Move(deck, card, CardPileType.Hand, CardPileType.Discard);
        card.OnPlay(targets);
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
            OnCardDrawn?.Invoke(card);
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
        
        AdjustMana(deck, 10);
        _decks[TeamType.Player] = deck;
        return deck;
    }
    
    private void AdjustMana(Deck deck, int mana, bool enforceCap = true)
    {
        var manaBefore = deck.CurrentMana;
        deck.CurrentMana = enforceCap ? Mathf.Clamp(deck.CurrentMana + mana, 0, 10) : Mathf.Max(deck.CurrentMana + mana, 0);
        if (manaBefore != deck.CurrentMana)
        {
            OnManaChanged?.Invoke(deck.CurrentMana);
        }
    }
    
    private void Move(Deck deck, ICard card, CardPileType from, CardPileType to)
    {
        if (!deck.CardPiles[from].Contains(card))
        {
            _logger.LogWarning($"Card {card.Config.Name} not found in player's {from} pile. Still adding to {to}");
        }
        else
        {
            deck.CardPiles[from].Remove(card);
        }
        
        deck.CardPiles[from].Remove(card);
        deck.CardPiles[to].Add(card);
        OnCardMoved?.Invoke(card, from, to);
    }
}
