using Battles;
using UnityEngine;
using UnityEngine.UI;

public class UnitResourceBarView : MonoBehaviour, IPoolable
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Vector2 offset = new(0f, 55f);
    [SerializeField] private Color enemyHealthColor = Color.red;
    [SerializeField] private Color friendlyHealthColor = Color.green;
    private UnitPrefab _unitPrefab;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Camera _camera;
    private Image _fillImage;
    private Logger _logger = new(nameof(UnitResourceBarView));
    
    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _camera = Camera.main;
        _fillImage = healthSlider.fillRect.GetComponent<Image>();
    }

    private void LateUpdate()
    {
        UpdateScreenPosition();
    }

    public void Bind(UnitPrefab unitPrefab)
    {
        _unitPrefab = unitPrefab;
        _unitPrefab.Unit.OnCurrentHealthChanged += Refresh;
        _fillImage.color = unitPrefab.Unit.Team switch
        {
            TeamType.Player => friendlyHealthColor,
            TeamType.Enemy => enemyHealthColor,
            _ => _fillImage.color
        };
        
        _logger.Log($"{gameObject.name} bound to {unitPrefab.Unit.Team}");
        Refresh(_unitPrefab.Unit.CurrentHealth);
    }

    public void Unbind()
    {
        if (!_unitPrefab)
        {
            return;
        }
        
        _unitPrefab.Unit.OnCurrentHealthChanged -= Refresh;
        _unitPrefab = null;
    }

    private void Refresh(int currentHealth)
    {
        healthSlider.maxValue = _unitPrefab.Unit.Config.Health;
        healthSlider.value = _unitPrefab.Unit.CurrentHealth;
    }

    private void UpdateScreenPosition()
    {
        if (!_unitPrefab || !_camera)
        {
            return;
        }

        var screenPosition = _camera.WorldToScreenPoint(_unitPrefab.transform.position);
        screenPosition += (Vector3)offset;

        if (_canvas && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                screenPosition,
                _canvas.worldCamera,
                out var localPosition);

            _rectTransform.anchoredPosition = localPosition;
            return;
        }

        _rectTransform.position = screenPosition;
    }

    public void Reset()
    {
        Unbind();
    }

    public GameObject Prefab { get; set; }
}
