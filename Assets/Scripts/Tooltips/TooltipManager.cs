using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Tooltips;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour, IDomainEventListener<HoveredElementUpdatedDomainEvent>
{
    [SerializeField] private TooltipPrefab tooltipPrefab;
    [SerializeField] private Canvas canvas;
    private DomainEventService _domainEventService;
    private PoolService _poolService;
    private InputService _inputService;
    private TooltipPrefab _activeTooltip;
    private readonly Logger _logger = new(nameof(TooltipManager));
    
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
        var pointerPosition = pointAction.ReadValue<Vector2>();

        _activeTooltip.UpdateMessage(tooltipProvider.GetTooltip());
        _activeTooltip.gameObject.SetActive(true);

        var tooltipRect = _activeTooltip.RectTransform;
        var canvasRect = canvas.GetComponent<RectTransform>();
        var canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            pointerPosition,
            canvasCamera,
            out var localPointerPosition
        );

        tooltipRect.anchoredPosition = localPointerPosition;

        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        Canvas.ForceUpdateCanvases();

        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvasRect, tooltipRect);
        var canvasBounds = canvasRect.rect;

        var adjustment = Vector2.zero;

        if (bounds.min.x < canvasBounds.xMin)
        {
            adjustment.x = canvasBounds.xMin - bounds.min.x;
        }
        else if (bounds.max.x > canvasBounds.xMax)
        {
            adjustment.x = canvasBounds.xMax - bounds.max.x;
        }

        if (bounds.min.y < canvasBounds.yMin)
        {
            adjustment.y = canvasBounds.yMin - bounds.min.y;
        }
        else if (bounds.max.y > canvasBounds.yMax)
        {
            adjustment.y = canvasBounds.yMax - bounds.max.y;
        }

        tooltipRect.anchoredPosition += adjustment;
        return UniTask.CompletedTask;
    }
}
