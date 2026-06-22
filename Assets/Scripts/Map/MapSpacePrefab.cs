using Battles;
using Map;
using Targeting;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MapSpacePrefab : MonoBehaviour, IHoverable
{
    [FormerlySerializedAs("targetMaterial")] [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material invalidMaterial;
    
    [SerializeField] private int q;
    public int Q => q;
    
    [SerializeField] private int r;
    public int R => r;
    
    public MapSpace Space { get; private set; }
    private Material _defaultMaterial;
    private Renderer _renderer;
    private SelectService _selectService;
    private MapSpaceState _state;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _defaultMaterial = _renderer.material;
        _selectService = Locator.Get<SelectService>();
    }

    private void OnEnable()
    {
        _selectService.OnActiveContextChanged += OnActiveSelectContextChanged;
    }
    
    private void OnDisable()
    {
        if (_selectService != null)
        {
            _selectService.OnActiveContextChanged -= OnActiveSelectContextChanged;
        }
    }

    public void Initialize(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public void Bind(MapSpace space)
    {
        Space = space;
    }

    public void OnHover()
    {
        if (_state != MapSpaceState.Invalid)
        {
            SetState(MapSpaceState.Hovered);
        }
    }

    public void OnUnhover()
    {
        if (_state != MapSpaceState.Invalid)
        {
            SetState(MapSpaceState.Default);
        }
    }

    public void OnHoverClick()
    {
        _selectService.Select(Space, TeamType.Player);
    }

    private void OnActiveSelectContextChanged(TeamType team, ISelectContext context)
    {
        if (context == null || context.Config.Type != SelectContextType.Map)
        {
            _renderer.material = _state == MapSpaceState.Hovered ? hoverMaterial : _defaultMaterial;
            return;
        }

        var isValid = context.CanSelect(Space);
        SetState(!isValid ? MapSpaceState.Invalid : MapSpaceState.Default);
    }

    private void SetState(MapSpaceState state)
    {
        switch (state)
        {
            case MapSpaceState.Default:
                _renderer.material = _defaultMaterial;
                break;
            case MapSpaceState.Hovered:
                _renderer.material = hoverMaterial;
                break;
            case MapSpaceState.Invalid:
                _renderer.material = invalidMaterial;
                break;
        }
        
        _state = state;
    }
}
