using System;
using System.Collections.Generic;
using AI;
using Cards;
using Units;
using UnityEngine;

namespace Battles
{
    [CreateAssetMenu(menuName = "Game Config/Battle")]
    public class BattleConfig : ScriptableObject
    {
        [Header("Player")]
        [SerializeField] private List<BattleUnitConfig> playerUnits;
        public IReadOnlyList<BattleUnitConfig> PlayerUnits => playerUnits;
        
        [SerializeField] private DeckConfig playerDeck;
        public DeckConfig PlayerDeck => playerDeck;
        
        [Header("Enemy")]
        [SerializeField] private List<BattleUnitConfig> enemyUnits;
        public IReadOnlyList<BattleUnitConfig> EnemyUnits => enemyUnits;
        
        [SerializeField] private DeckConfig enemyDeck;
        public DeckConfig EnemyDeck => enemyDeck;
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

        [SerializeField] private UnitBrainConfig brain;
        public UnitBrainConfig Brain => brain;
    }
}