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
    private ICard _card;
    private Button _button;
    private CardService _cardService;
    public event Action<ICard> OnHovered;
    public event Action<ICard> OnUnhovered;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _cardService = Locator.Get<CardService>();
    }
    
    private void OnEnable()
    {
        _button.onClick.AddListener(OnSelect);
    }
    
    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnSelect);
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
        selectedBackground.gameObject.SetActive(true);
        OnHovered?.Invoke(_card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectedBackground.gameObject.SetActive(false);
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
    }

    private void OnSelect()
    {
        if (_card == null)
        {
            return;
        }
        
        _cardService.Select(_card);
    }
    
    public GameObject Prefab { get; set; }
}
