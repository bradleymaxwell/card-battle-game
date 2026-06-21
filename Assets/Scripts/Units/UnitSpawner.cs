using Units;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private UnitResourceBarView resourceBarViewPrefab;
    private UnitService _unitService;
    private PoolService _poolService;
    
    private void Awake()
    {
        _unitService = Locator.Get<UnitService>();
        _poolService = Locator.Get<PoolService>();
        
        foreach (var unit in _unitService.Units)
        {
            OnUnitSpawned(unit);
        }
    }

    private void OnEnable()
    {
        _unitService.OnUnitSpawned += OnUnitSpawned;
    }
    
    private void OnUnitSpawned(IUnit unit)
    {
        var unitView = _poolService.Get(unit.Config.Prefab);
        unitView.Bind(unit);
        var resourceBarView = _poolService.Get(resourceBarViewPrefab);
        resourceBarView.transform.SetParent(canvas.transform, false);
        resourceBarView.Bind(unitView);
    }
}
