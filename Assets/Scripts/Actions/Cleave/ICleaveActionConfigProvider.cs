using System.Collections.Generic;

namespace Units.Cleave
{
    public interface ICleaveActionConfigProvider : IActionConfigProvider
    {
        IList<float> PercentDamagePerHit { get; }
        int PerfectHitThreshold { get; }
        float PerfectHitDamagePercentIncrease { get; }
    }
}