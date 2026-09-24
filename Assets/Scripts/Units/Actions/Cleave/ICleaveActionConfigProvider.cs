using System.Collections.Generic;

namespace Units.Cleave
{
    public interface ICleaveActionConfigProvider : IActionConfigProvider
    {
        int BaseDamage { get; }
        IList<float> DamagePercentIncreasePerHit { get; }
        int PerfectHitThreshold { get; }
        float PerfectHitDamagePercentIncrease { get; }
    }
}