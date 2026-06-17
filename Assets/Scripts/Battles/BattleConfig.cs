using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Battles
{
    [CreateAssetMenu(menuName = "Game Config/Battle")]
    public class BattleConfig : ScriptableObject
    {
        [SerializeField] private List<BattleUnitConfig> enemyUnits;
        public IReadOnlyList<BattleUnitConfig> EnemyUnits => enemyUnits;
        
        [SerializeField] private List<BattleUnitConfig> playerUnits;
        public IReadOnlyList<BattleUnitConfig> PlayerUnits => playerUnits;
    }
    
    [Serializable]
    public class BattleUnitConfig
    {
        [SerializeField] private UnitConfig unit;
        public UnitConfig Unit => unit;

        [SerializeField] private int startQ;
        public int StartQ => startQ;
        
        [SerializeField] private int startR;
        public int StartR => startR;
    }
}