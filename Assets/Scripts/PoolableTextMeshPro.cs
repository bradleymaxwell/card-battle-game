using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PoolableTextMeshPro : MonoBehaviour, IPoolable
{
    public TextMeshProUGUI TextMeshPro { get; private set; }

    private void Awake()
    {
        TextMeshPro = GetComponent<TextMeshProUGUI>();
    }
    
    public void Reset()
    {
        TextMeshPro.text = string.Empty;
    }

    public GameObject Prefab { get; set; }
}
