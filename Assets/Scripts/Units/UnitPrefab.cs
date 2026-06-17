using Map;
using Units;
using UnityEngine;

public class UnitPrefab : MonoBehaviour, IPoolable
{
    private IUnit Unit { get; set; }
    private MapService _mapService;

    private void Awake()
    {
        _mapService = Locator.Get<MapService>();
    }
    
    public void Bind(IUnit unit)
    {
        Unit = unit;
        unit.OnMapSpaceChanged += OnMapSpaceUpdated;
    }

    public void Reset()
    {
        if (Unit != null)
        {
            Unit.OnMapSpaceChanged -= OnMapSpaceUpdated;
        }
        
        Unit = null;
    }

    public GameObject Prefab { get; set; }

    private void OnMapSpaceUpdated(MapSpace mapSpace)
    {
        var mapSpacePrefab = _mapService.GetPrefab(mapSpace);
        transform.position = new Vector3(mapSpacePrefab.transform.position.x, transform.position.y, mapSpacePrefab.transform.position.z);
    }
}
