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
    [SerializeField] private Color activeUnitColor = Color.softYellow;
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
    private UnitService _unitService;
    private bool _isUnitActive;
    
    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _camera = Camera.main;
        _fillImage = healthSlider.fillRect.GetComponent<Image>();
        _unitService = Locator.Get<UnitService>();
    }

    private void LateUpdate()
    {
        UpdateScreenPosition();
    }

    public void Bind(UnitPrefab unitPrefab)
    {
        _unitPrefab = unitPrefab;
        _unitPrefab.Unit.OnCurrentEnergyChanged += RefreshEnergy;
        _unitService.OnActiveUnitChanged += OnActiveUnitChanged;
        var activeUnit = _unitService.GetActiveUnit(_unitPrefab.Unit.Team);
        if (activeUnit == _unitPrefab.Unit)
        {
            _isUnitActive = true;
        }
        
        SetHealthBarColor();
        _logger.Log($"{gameObject.name} bound to {unitPrefab.Unit.Team}");
        Refresh();
    }

    public void RefreshHealth()
    {
        healthSlider.maxValue = _unitPrefab.Unit.Config.Health;
        healthSlider.value = _unitPrefab.Unit.CurrentHealth;
    }
    
    private void Unbind()
    {
        if (!_unitPrefab)
        {
            return;
        }

        if (_unitPrefab.Unit == null)
        {
            return;
        }
        
        _unitPrefab = null;
    }
    
    private void Refresh()
    {
        RefreshHealth();
        RefreshEnergy();
    }

    private void RefreshEnergy(int _ = 0)
    {
        energySlider.maxValue = _unitPrefab.Unit.Energy;
        energySlider.value = _unitPrefab.Unit.CurrentEnergy;
    }
    
    private void OnActiveUnitChanged(TeamType team, IUnit unit)
    {
        if (team != TeamType.Player)
        {
            return;
        }

        if (_isUnitActive && unit != _unitPrefab.Unit)
        {
            _isUnitActive = false;
            SetHealthBarColor();
            return;
        }
        
        if (!_isUnitActive && unit == _unitPrefab.Unit)
        {
            _isUnitActive = true;
            SetHealthBarColor();
        }
    }

    private void SetHealthBarColor()
    {
        if (_isUnitActive)
        {
            _fillImage.color = activeUnitColor;
            return;
        }
        
        _fillImage.color = _unitPrefab.Unit.Team switch
        {
            TeamType.Player => friendlyHealthColor,
            TeamType.Enemy => enemyHealthColor,
            _ => _fillImage.color
        };
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
