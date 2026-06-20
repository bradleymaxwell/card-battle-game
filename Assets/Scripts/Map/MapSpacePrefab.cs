using Battles;
using Map;
using Targeting;
using UnityEngine;

public class MapSpacePrefab : MonoBehaviour, IHoverable
{
    [SerializeField] private Material targetMaterial;
    [SerializeField] private int q;
    public int Q => q;
    
    [SerializeField] private int r;
    public int R => r;
    
    public MapSpace Space { get; private set; }
    private Material _originMaterial;
    private Renderer _renderer;
    private SelectService _selectService;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _originMaterial = _renderer.material;
        _selectService = Locator.Get<SelectService>();
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
        _renderer.material = targetMaterial;
    }

    public void OnUnhover()
    {
        _renderer.material = _originMaterial;
    }

    public void OnHoverClick()
    {
        _selectService.Select(Space, TeamType.Player);
    }
}
