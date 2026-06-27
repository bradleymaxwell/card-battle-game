using Battles;
using Targeting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Units
{
    [RequireComponent(typeof(Button))]
    public class ActionView : MonoBehaviour, IPoolable, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private Image selectedBackground;
        private Button _button;
        private IUnit _unit;
        private IAction _action;
        private UnitService _unitService;
        private SelectService _selectService;
        private bool _isSelected;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _unitService = Locator.Get<UnitService>();
            _selectService = Locator.Get<SelectService>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnSelect);
            _selectService.OnActiveContextChanged += OnActiveContextChanged;
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnSelect);
            if (_selectService != null)
            {
                _selectService.OnActiveContextChanged -= OnActiveContextChanged;
            }
        }

        public void Bind(IUnit unit, IAction action)
        {
            _unit = unit;
            _action = action;
            _unit.OnCurrentEnergyChanged += OnCurrentEnergyChanged;
            Refresh();
        }

        private void Unbind()
        {
            if (_unit != null)
            {
                _unit.OnCurrentEnergyChanged -= OnCurrentEnergyChanged;
            }
            
            _unit = null;
            _action = null;
        }

        private void Refresh()
        {
            image.sprite = _action.Icon;
            var canPerform = _unit.CurrentEnergy >= _action.Config.EnergyCost;
            _button.interactable = canPerform;
            image.color = canPerform ? Color.white : Color.gray;
        }

        private void OnCurrentEnergyChanged(int _)
        {
            Refresh();
        }

        public void Reset()
        {
            Unbind();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isSelected)
            {
                selectedBackground.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isSelected)
            {
                selectedBackground.gameObject.SetActive(false);
            }
        }

        private void OnSelect()
        {
            if (_unit.CurrentEnergy < _action.Config.EnergyCost)
            {
                return;
            }
            
            _unitService.Perform(_unit, _action);
            UpdateSelected(true);
        }
        
        private void UpdateSelected(bool isSelected)
        {
            selectedBackground.gameObject.SetActive(isSelected);
            _isSelected = isSelected;
        }
        
        private void OnActiveContextChanged(TeamType team, ISelectContext context)
        {
            if (team != TeamType.Player)
            {
                return;
            }

            if (_isSelected)
            {
                UpdateSelected(false);
            }
        }
        
        public GameObject Prefab { get; set; }
       
    }
}