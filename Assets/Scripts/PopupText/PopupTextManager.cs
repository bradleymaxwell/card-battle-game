using System.Collections;
using UnityEngine;

public class PopupTextManager : MonoBehaviour
{
    [SerializeField] private PoolableTextMeshPro textPrefab;
    [SerializeField] private float displayTime = 1f;
    [SerializeField] private Canvas canvas;
    private Camera _camera;
    private PoolService _poolService;
    
    private void Awake()
    {
        _camera = Camera.main;
        _poolService = Locator.Get<PoolService>();
    }
    
    private void OnEnable()
    {
        Locator.Register(this);
    }
    
    public void Display(string text, Vector3 worldPosition)
    {
        var screenPosition = _camera.WorldToScreenPoint(worldPosition);
        StartCoroutine(DisplayCor(text, screenPosition));
    }

    private IEnumerator DisplayCor(string text, Vector2 screenPosition)
    {
        var textInstance = _poolService.Get(textPrefab);
        textInstance.TextMeshPro.text = text;
        textInstance.transform.SetParent(canvas.transform, false);
        textInstance.TextMeshPro.rectTransform.position = screenPosition;
        textInstance.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        _poolService.Return(textInstance);      
    }
}
