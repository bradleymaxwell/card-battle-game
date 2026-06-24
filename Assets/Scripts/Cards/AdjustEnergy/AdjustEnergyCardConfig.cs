using UnityEngine;

namespace Cards.AdjustEnergy
{
    [CreateAssetMenu(menuName = "Game Config/Card/Adjust Energy", fileName = "AdjustEnergyCard")]
    public class AdjustEnergyCardConfig : CardConfig
    {
        [SerializeField] private int adjustment;
        public int Adjustment => adjustment;
        
        public override ICard Card => new AdjustEnergyCard(this);
    }
}