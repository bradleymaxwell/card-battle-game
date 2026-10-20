using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.Tooltips
{
    [RequireComponent(typeof(RectTransform))]
    public class TooltipPrefab : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshProUGUI text;
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public void UpdateMessage(string message)
        {
            text.text = message;
        }

        public void Reset()
        {
            text.text = string.Empty;
        }

        public GameObject Prefab { get; set; }
    }
}