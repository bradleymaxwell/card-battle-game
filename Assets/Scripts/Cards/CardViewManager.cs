using Battles;
using Targeting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class CardViewManager : MonoBehaviour
    {
        [SerializeField] private CardView cardViewPrefab;
        [SerializeField] private GameObject playerHandContainer;
        [SerializeField] private CardView inspectedCardView;
        [SerializeField] private Slider manaSlider;
        [SerializeField] private TextMeshProUGUI manaText;
        private CardService _cardService;
        private PoolService _poolService;
        private SelectService _selectService;
        private ICard _selectedCard;
        
        private void Awake()
        {
            _cardService = Locator.Get<CardService>();
            _poolService = Locator.Get<PoolService>();
            _selectService = Locator.Get<SelectService>();
        }

        private void OnEnable()
        {
            _cardService.OnSelectedCardChanged += OnSelectedCardChanged;
            _cardService.OnCardDrawn += OnCardDrawn;
            _cardService.OnManaChanged += RefreshMana;
            _selectService.OnActiveContextChanged += OnActiveContextChanged;
            
            var deck = _cardService.GetDeck();
            if (deck == null)
            {
                return;
            }
            
            if (deck.CardPiles.TryGetValue(CardPileType.Hand, out var hand) && hand.Count > 0)
            {
                foreach (var card in hand)
                {
                    OnCardDrawn(card);
                }
            }
            
            RefreshMana(deck.CurrentMana);
        }
        
        private void OnSelectedCardChanged(ICard card)
        {
            if (card == null)
            {
                _selectedCard = null;
                UpdateInspected(null);
            }
            else
            {
                _selectedCard = card;
                UpdateInspected(card);
            }
        }

        private void OnCardDrawn(ICard card)
        {
            var cardView = _poolService.Get(cardViewPrefab);
            cardView.OnHovered += OnHoveredChanged;
            cardView.OnUnhovered += OnHoveredChanged;
            cardView.Bind(card);
            cardView.transform.SetParent(playerHandContainer.transform, false);
        }

        private void OnHoveredChanged(ICard card)
        {
            // selected card to play will always be inspected over a hovered card
            if (_selectedCard != null)
            {
                return;
            }
            
            UpdateInspected(card);
        }

        private void UpdateInspected(ICard card)
        {
            if (card == null)
            {
                inspectedCardView.gameObject.SetActive(false);
            }
            else
            {
                inspectedCardView.gameObject.SetActive(true);
                inspectedCardView.Bind(card);
            }
        }

        private void RefreshMana(int mana)
        {
            manaSlider.value = mana;
            manaText.text = $"{mana}/{manaSlider.maxValue}";
        }
        
        private void OnActiveContextChanged(TeamType team, ISelectContext context)
        {
            if (team != TeamType.Player)
            {
                return;
            }

            if (_selectedCard != null)
            {
                OnSelectedCardChanged(null);
            }
        }
    }
}