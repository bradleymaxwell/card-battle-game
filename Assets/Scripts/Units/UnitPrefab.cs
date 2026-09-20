using System.Collections.Generic;
using Map;
using Units;
using UnityEngine;
using Action = System.Action;

[RequireComponent(typeof(Animator))]
public class UnitPrefab : MonoBehaviour, IPoolable
{
    [SerializeField] private float yOffset;
    private MapService _mapService;
    private readonly IDictionary<string, IList<Action>> _callbacksByEventName = new Dictionary<string, IList<Action>>();
    public IUnit Unit { get; private set; }
    public Animator Animator { get; private set; }
    private Material _defaultMaterial;
    private Renderer _renderer;
    private Logger _logger = new(nameof(UnitPrefab));
    
    private void Awake()
    {
        _mapService = Locator.Get<MapService>();
        Animator = GetComponent<Animator>();
        _renderer = GetComponent<Renderer>();
        _defaultMaterial = _renderer?.material;
    }
    
    public void Bind(IUnit unit)
    {
        Unit = unit;
    }

    public void SetMaterial(Material material)
    {
        if (!_renderer || !material)
        {
            _logger.LogWarning($"Cannot set material for {gameObject.name} because it has no renderer or material, so ignoring request");
            return;
        }
        
        _renderer.material = material;
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
        _callbacksByEventName.Clear();
        if (_renderer && _defaultMaterial)
        {
            _renderer.material = _defaultMaterial;
        }
    }

    public void Spawn(MapSpace mapSpace = null)
    {
        mapSpace ??= _mapService.GetSpace(Unit);
        var mapSpacePrefab = _mapService.GetPrefab(mapSpace);
        transform.position = new Vector3(mapSpacePrefab.transform.position.x, yOffset, mapSpacePrefab.transform.position.z);
    }
    
    public GameObject Prefab { get; set; }
}
