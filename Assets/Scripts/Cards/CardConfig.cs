using UnityEngine;

namespace Cards
{
    public abstract class CardConfig : ScriptableObject
    {
        [SerializeField] private new string name;
        public string Name => name;
        
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        
        [SerializeField] private int manaCost;
        public int ManaCost => manaCost;
        
        public abstract ICard Card { get; }
    }
}