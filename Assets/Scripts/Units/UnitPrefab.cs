using Map;
using Units;
using UnityEngine;

public class UnitPrefab : MonoBehaviour, IPoolable
{
    [SerializeField] private float yOffset = 1f;
    public IUnit Unit { get; private set; }
    private MapService _mapService;
    private Logger _logger = new(nameof(UnitPrefab));
    
    private void Awake()
    {
        _mapService = Locator.Get<MapService>();
    }
    
    public void Bind(IUnit unit)
    {
        Unit = unit;
        unit.OnMapSpaceChanged += OnMapSpaceUpdated;
        var space = _mapService.GetSpace(unit);
        OnMapSpaceUpdated(space);
    }

    public void Reset()
    {
        if (Unit != null)
        {
            Unit.OnMapSpaceChanged -= OnMapSpaceUpdated;
        }
        
        Unit = null;
    }

    private void OnMapSpaceUpdated(MapSpace mapSpace)
    {
        var mapSpacePrefab = _mapService.GetPrefab(mapSpace);
        transform.position = new Vector3(mapSpacePrefab.transform.position.x, yOffset, mapSpacePrefab.transform.position.z);
    }
    
    public GameObject Prefab { get; set; }
}
