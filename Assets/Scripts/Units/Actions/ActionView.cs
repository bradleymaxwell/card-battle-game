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
            Refresh();
        }

        private void Unbind()
        {
            _unit = null;
            _action = null;
        }

        private void Refresh()
        {
            image.sprite = _action.Icon;
        }

        public void Reset()
        {
            Unbind();
        }

        public void OnSelect()
        {
            _unitService.Perform(_unit, _action);
        }

        public GameObject Prefab { get; set; }
    }
}