using UnityEngine;

namespace Units
{
    public abstract class ActionConfig : ScriptableObject
    {
        [SerializeField] private int energyCost;
        public int EnergyCost => energyCost;

        [SerializeField] private new string name;
        public string Name => name;
        
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        
        [SerializeField] private string description;
        public string Description => description;
        
        public abstract IAction Action { get; }
    }
}