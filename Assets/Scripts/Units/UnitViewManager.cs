using System;
using System.Collections.Generic;
using Map;
using Units;
using UnityEngine;

public class UnitViewManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private UnitResourceBarView resourceBarViewPrefab;
    private UnitService _unitService;
    private PoolService _poolService;
    private readonly IDictionary<IUnit, Tuple<UnitPrefab, UnitResourceBarView>> _unitViews = new Dictionary<IUnit, Tuple<UnitPrefab, UnitResourceBarView>>();
    private readonly Logger _logger = new(nameof(UnitViewManager));

    private void Awake()
    {
        Locator.Register(this);
        _unitService = Locator.Get<UnitService>();
        _poolService = Locator.Get<PoolService>();
    }
    
    public void Initialize()
    {
        foreach (var unit in _unitService.Units)
        {
            OnUnitSpawned(unit);
        }
    }
    
    public void OnHealthAdjusted(IUnit unit, int adjustment)
    {
        if (unit.CurrentHealth <= 0)
        {
            OnUnitDefeated(unit);
        }
        else
        {
            var (_, resourceBar) = GetUnitViews(unit);
            resourceBar.AdjustCurrentHealth(adjustment);
        }
    }
    
    public Tuple<UnitPrefab, UnitResourceBarView> GetUnitViews(IUnit unit)
    {
        var found = _unitViews.TryGetValue(unit, out var views);
        if (!found)
        {
            _logger.LogError($"Could not find any existing views bound for unit: {unit.Config.Name}");
            return new Tuple<UnitPrefab, UnitResourceBarView>(null, null);
        }
        
        return views;
    }
    
    public void OnUnitDefeated(IUnit unit)
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

    public void OnUnitSpawned(IUnit unit, MapSpace space = null)
    {
        var unitView = _poolService.Get(unit.Config.Prefab);
        unitView.Bind(unit);
        unitView.Spawn(space);
        var resourceBarView = _poolService.Get(resourceBarViewPrefab);
        resourceBarView.transform.SetParent(canvas.transform, false);
        resourceBarView.Bind(unitView);
        _unitViews.Add(unit, new Tuple<UnitPrefab, UnitResourceBarView>(unitView, resourceBarView));
    }
}
