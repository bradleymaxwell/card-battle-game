using UnityEngine;

namespace DefaultNamespace.Tooltips
{
    public class HoveredElementUpdatedDomainEvent : DomainEvent
    {
        public GameObject HoveredElement { get; set; }
    }
}