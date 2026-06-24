using Battles;
using Units;
using UnityEngine;
using UnityEngine.UI;

public class UnitResourceBarView : MonoBehaviour, IPoolable
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Vector2 offset = new(0f, 55f);
    [SerializeField] private Color enemyHealthColor = Color.red;
    [SerializeField] private Color friendlyHealthColor = Color.green;
    private UnitPrefab _unitPrefab;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Camera _camera;
    private Image _fillImage;
    private readonly Logger _logger = new(nameof(UnitResourceBarView));
    private Vector2 _lastAnchoredPosition;
    private Vector3 _lastWorldPosition;
    private bool _hasLastAnchoredPosition;
    private bool _hasLastWorldPosition;
    
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
        _unitPrefab.Unit.OnCurrentEnergyChanged += Refresh;
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

        if (_unitPrefab.Unit == null)
        {
            return;
        }
        
        _unitPrefab.Unit.OnCurrentHealthChanged -= Refresh;
        _unitPrefab = null;
    }

    private void Refresh(int _)
    {
        healthSlider.maxValue = _unitPrefab.Unit.Config.Health;
        healthSlider.value = _unitPrefab.Unit.CurrentHealth;
        energySlider.maxValue = _unitPrefab.Unit.Energy;
        energySlider.value = _unitPrefab.Unit.CurrentEnergy;
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
            
            if (_hasLastAnchoredPosition && Vector2.SqrMagnitude(_lastAnchoredPosition - localPosition) < 0.01f)
            {
                return;
            }
            
            _lastAnchoredPosition = localPosition;
            _hasLastAnchoredPosition = true;
            _rectTransform.anchoredPosition = localPosition;
            return;
        }
        
        if (_hasLastWorldPosition && Vector3.SqrMagnitude(_lastWorldPosition - screenPosition) < 0.01f)
        {
            return;
        }
        
        _lastWorldPosition = screenPosition;
        _hasLastWorldPosition = true;
        _rectTransform.position = screenPosition;
    }
    
    public void Reset()
    {
        Unbind();
        _hasLastAnchoredPosition = false;
        _hasLastWorldPosition = false;
    }

    public GameObject Prefab { get; set; }
}
