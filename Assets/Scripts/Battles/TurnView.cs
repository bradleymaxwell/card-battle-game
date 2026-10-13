using System;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace Battles
{
    [RequireComponent(typeof(Image))]
    public class TurnView : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] [Range(0, 1)] private float nonActiveOpacity;
        [SerializeField] private Color playerTeamColor;
        [SerializeField] private Color enemyTeamColor;
        [SerializeField] private Color activeUnitColor;
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void Set(IUnit unit, bool isActive)
        {
            nameText.text = unit?.Config?.Name;
            var color = isActive ? activeUnitColor : unit?.Team == TeamType.Player ? playerTeamColor : enemyTeamColor;
            _image.color = new Color(color.r, color.g, color.b, isActive ? 1 : nonActiveOpacity);
        }
        
        public void Reset()
        {
            Set(null, false);
        }

        public GameObject Prefab { get; set; }
    }
}