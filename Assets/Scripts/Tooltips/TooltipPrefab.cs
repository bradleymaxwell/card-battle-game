using TMPro;
using UnityEngine;

namespace DefaultNamespace.Tooltips
{
    public class TooltipPrefab : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshProUGUI text;

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