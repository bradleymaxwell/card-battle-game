using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private UnitResourceBarView resourceBarViewPrefab;
    private UnitService _unitService;
    private PoolService _poolService;
    private IDictionary<IUnit, Tuple<UnitPrefab, UnitResourceBarView>> _unitViews = new Dictionary<IUnit, Tuple<UnitPrefab, UnitResourceBarView>>();
    
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
        _unitService.OnUnitDefeated += OnUnitDefeated;
    }

    private void OnDisable()
    {
        if (_unitService != null)
        {
            _unitService.OnUnitSpawned -= OnUnitSpawned;
            _unitService.OnUnitDefeated -= OnUnitDefeated;
        }
    }
    
    private void OnUnitSpawned(IUnit unit)
    {
        var unitView = _poolService.Get(unit.Config.Prefab);
        unitView.Bind(unit);
        var resourceBarView = _poolService.Get(resourceBarViewPrefab);
        resourceBarView.transform.SetParent(canvas.transform, false);
        resourceBarView.Bind(unitView);
        _unitViews.Add(unit, new Tuple<UnitPrefab, UnitResourceBarView>(unitView, resourceBarView));
    }

    private void OnUnitDefeated(IUnit unit)
    {
        var viewsFound = _unitViews.TryGetValue(unit, out var views);
        if (!viewsFound)
        {
            return;
        }
        
        _poolService.Return(views.Item1);
        _poolService.Return(views.Item2);
        _unitViews.Remove(unit);
    }
}
