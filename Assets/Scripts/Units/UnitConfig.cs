using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(menuName = "Game Config/Unit")]
    public class UnitConfig : ScriptableObject
    {
        [SerializeField] private new string name;
        public string Name => name;
        
        [SerializeField] private UnitPrefab prefab;
        public UnitPrefab Prefab => prefab;

        [SerializeField] private int attack;
        public int Attack => attack;
        
        [SerializeField] private int health;
        public int Health => health;

        [SerializeField] private List<ActionConfig> actions;
        public IReadOnlyList<ActionConfig> Actions => actions;
    }
}