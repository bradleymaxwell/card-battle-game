using UnityEngine;

namespace Cards.AdjustHealth
{
    [CreateAssetMenu(menuName = "Game Config/Card/Adjust Health", fileName = "AdjustHealthCard")]
    public class AdjustHealthCardConfig : CardConfig
    {
        [SerializeField] private int adjustment;
        public int Adjustment => adjustment;
        
        public override ICard Card => new AdjustHealthCard(this);
    }
}