using System.Collections;
using System.Collections.Generic;
using Map;
using Units;
using UnityEngine;
using Action = System.Action;

[RequireComponent(typeof(Animator))]
public class UnitPrefab : MonoBehaviour, IPoolable
{
    [SerializeField] private float yOffset = 1f;
    private MapService _mapService;
    private IDictionary<string, IList<Action>> _callbacksByEventName = new Dictionary<string, IList<Action>>();
    public IUnit Unit { get; private set; }
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

    public void SubscribeToAnimationEvent(string eventName, Action callback)
    {
        var found = _callbacksByEventName.TryGetValue(eventName, out var callbacks);
        if (!found)
        {
            _callbacksByEventName.Add(eventName, new List<Action> { callback });
        }
        else
        {
            callbacks.Add(callback);
        }
    }

    public void RaiseAnimationEvent(string eventName)
    {
        var found = _callbacksByEventName.TryGetValue(eventName, out var callbacks);
        if (!found)
        {
            return;
        }
        
        foreach (var callback in callbacks)
        {
            callback?.Invoke();
        }
        
        callbacks.Clear();
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
