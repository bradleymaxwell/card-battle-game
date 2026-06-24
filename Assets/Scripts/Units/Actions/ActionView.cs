using System;
using Targeting;
using UnityEngine;
using UnityEngine.UI;

namespace Units
{
    [RequireComponent(typeof(Button))]
    public class ActionView : MonoBehaviour, IPoolable
    {
        [SerializeField] private Image image;
        private Button _button;
        private IUnit _unit;
        private IAction _action;
        private UnitService _unitService;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _unitService = Locator.Get<UnitService>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnSelect);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnSelect);
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

        public void OnSelect()
        {
            if (_unit.CurrentEnergy < _action.Config.EnergyCost)
            {
                return;
            }
            
            _unitService.Perform(_unit, _action);
        }

        public GameObject Prefab { get; set; }
    }
}