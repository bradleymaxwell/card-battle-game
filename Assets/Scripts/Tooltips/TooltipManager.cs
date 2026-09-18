using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Tooltips;
using UnityEngine;

public class TooltipManager : MonoBehaviour, IDomainEventListener<HoveredElementUpdatedDomainEvent>
{
    [SerializeField] private TooltipPrefab tooltipPrefab;
    [SerializeField] private Canvas canvas;
    private DomainEventService _domainEventService;
    private PoolService _poolService;
    private InputService _inputService;
    private TooltipPrefab _activeTooltip;
    private Logger _logger = new(nameof(TooltipManager));
    
    private void Awake()
    {
        _domainEventService = Locator.Get<DomainEventService>();
        _poolService = Locator.Get<PoolService>();
        _inputService = Locator.Get<InputService>();
    }

    private void OnEnable()
    {
        _domainEventService.Register(this);
    }

    private void OnDisable()
    {
        _domainEventService.Unregister(this);
    }

    public UniTask OnEventRaisedAsync(HoveredElementUpdatedDomainEvent domainEvent)
    {
        _logger.Log($"Updated hovered element: {domainEvent.HoveredElement?.name}");
        if (!domainEvent.HoveredElement)
        {
            if (_activeTooltip)
            {
                _poolService.Return(_activeTooltip);
                _activeTooltip = null;
            }
            
            return UniTask.CompletedTask;
        }

        var tooltipProvider = domainEvent.HoveredElement.GetComponent<ITooltipProvider>();
        if (tooltipProvider == null)
        {
            return UniTask.CompletedTask;
        }

        if (!_activeTooltip)
        {
            _activeTooltip = _poolService.Get(tooltipPrefab);
            _activeTooltip.transform.SetParent(canvas.transform, false);
        }

        var pointAction = _inputService.GetAction(PlayerInputConstants.UI, PlayerInputConstants.Point);
        _activeTooltip.transform.position = pointAction.ReadValue<Vector2>();
        _activeTooltip.UpdateMessage(tooltipProvider.GetTooltip());
        _activeTooltip.gameObject.SetActive(true);
        return UniTask.CompletedTask;
    }
}
