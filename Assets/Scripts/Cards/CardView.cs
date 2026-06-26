using System;
using Battles;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardView : MonoBehaviour, IPoolable, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private Image selectedBackground;
    [SerializeField] private bool isInspectCard;
    private ICard _card;
    private Button _button;
    private CardService _cardService;
    public event Action<ICard> OnHovered;
    public event Action<ICard> OnUnhovered;
    private Color _defaultManaTextColor;
    private Deck _deck;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _cardService = Locator.Get<CardService>();
        _defaultManaTextColor = manaCostText.color;
        _deck = _cardService.GetDeck();
    }
    
    private void OnEnable()
    {
        if (!isInspectCard)
        {
            _button.onClick.AddListener(OnSelect);
        }
        
        _cardService.OnManaChanged += Refresh;
    }
    
    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnSelect);
        if (_cardService != null)
        {
            _cardService.OnManaChanged -= Refresh;
        }
    }

    public void Bind(ICard card)
    {
        _card = card;
        Refresh();
    }

    public void Reset()
    {
        Unbind();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInspectCard)
        {
            selectedBackground.gameObject.SetActive(true);
        }
        
        OnHovered?.Invoke(_card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInspectCard)
        {
            selectedBackground.gameObject.SetActive(false);
        }
        
        OnUnhovered?.Invoke(null);
    }

    private void Unbind()
    {
        _card = null;
    }

    private void Refresh()
    {
        image.sprite = _card.Config.Icon;
        nameText.text = _card.Config.Name;
        descriptionText.text = _card.Description;
        manaCostText.text = _card.ManaCost.ToString();
        manaCostText.color = _card.ManaCost > _deck.CurrentMana ? Color.indianRed : _defaultManaTextColor;
    }

    private void Refresh(int _)
    {
        Refresh();
    }

    private void OnSelect()
    {
        if (_card == null || isInspectCard)
        {
            return;
        }
        
        _cardService.Select(_card);
    }
    
    public GameObject Prefab { get; set; }
}
