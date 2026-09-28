using DefaultNamespace.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace.Indicators
{
    public abstract class IndicatorView : MonoBehaviour, IPoolable, IPointerEnterHandler, IPointerExitHandler, ITooltipProvider
    {
        private DomainEventService _domainEventService;
        
        private void Awake()
        {
            _domainEventService = Locator.Get<DomainEventService>();
        }
        
        public void Reset()
        {
        }

        public GameObject Prefab { get; set; }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _domainEventService.Raise(new HoveredElementUpdatedDomainEvent
            {
                HoveredElement = gameObject
            });
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _domainEventService.Raise(new HoveredElementUpdatedDomainEvent
            {
                HoveredElement = null
            });
        }

        public abstract string GetTooltip();
    }
}