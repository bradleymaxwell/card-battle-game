using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(menuName = "Game Config/Deck")]
    public class DeckConfig : ScriptableObject
    {
        [SerializeField] private List<DeckEntryConfig> entries;
        public IReadOnlyList<DeckEntryConfig> Entries => entries;
    }

    [Serializable]
    public class DeckEntryConfig
    {
        [SerializeField] private CardConfig card;
        public CardConfig Card => card;
        
        [SerializeField] private int count = 1;
        public int Count => count;
    }
}