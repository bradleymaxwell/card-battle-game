using System;
using UnityEngine;

namespace Cards.AdjustEnergy
{
    [Obsolete]
    [CreateAssetMenu(menuName = "Game Config/Card/Adjust Energy", fileName = "AdjustEnergyCard")]
    public class AdjustEnergyCardConfig : CardConfig
    {
        [SerializeField] private int adjustment;
        public int Adjustment => adjustment;
        
        public override ICard Card => new AdjustEnergyCard(this);
    }
}