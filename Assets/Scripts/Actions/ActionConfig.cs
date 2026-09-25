using Units.VisualPlayback;
using UnityEngine;

namespace Units
{
    public abstract class ActionConfig : ScriptableObject, IActionConfigProvider
    {
        [SerializeField] private int energyCost;
        public int EnergyCost => energyCost;

        [SerializeField] private new string name;
        public string Name => name;
        
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        
        [TextArea(3, 5)]
        [SerializeField] private string description;
        public string Description => description;
        
        [SerializeField] private ActionVisualConfig visual;
        public ActionVisualConfig Visual => visual;       
        
        public abstract IAction Action { get; }
    }
}