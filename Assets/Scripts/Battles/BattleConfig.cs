using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battles
{
    [CreateAssetMenu(menuName = "Game Config/Battle")]
    public class BattleConfig : ScriptableObject
    {
        [SerializeField] private List<BattleUnitConfig> enemyUnits;
        public IReadOnlyList<BattleUnitConfig> EnemyUnits => enemyUnits;
    }
    
    [Serializable]
    public class BattleUnitConfig
    {
        [SerializeField] private UnitPrefab unitPrefab;
        public UnitPrefab UnitPrefab => unitPrefab;

        [SerializeField] private int startQ;
        public int StartQ => startQ;
        
        [SerializeField] private int startR;
        public int StartR => startR;
    }
}