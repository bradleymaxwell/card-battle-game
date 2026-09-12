using Map;
using Units;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitPrefab : MonoBehaviour, IPoolable
{
    [SerializeField] private float yOffset = 1f;
    public IUnit Unit { get; private set; }
    private MapService _mapService;
    private Logger _logger = new(nameof(UnitPrefab));
    public Animator Animator { get; private set; }
    
    private void Awake()
    {
        _mapService = Locator.Get<MapService>();
        Animator = GetComponent<Animator>();
    }
    
    public void Bind(IUnit unit)
    {
        Unit = unit;
        var space = _mapService.GetSpace(unit);
        OnMapSpaceUpdated(space);
    }

    public void Reset()
    {
        Unit = null;
    }

    private void OnMapSpaceUpdated(MapSpace mapSpace)
    {
        var mapSpacePrefab = _mapService.GetPrefab(mapSpace);
        transform.position = new Vector3(mapSpacePrefab.transform.position.x, yOffset, mapSpacePrefab.transform.position.z);
    }
    
    public GameObject Prefab { get; set; }
}
