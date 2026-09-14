using System;
using System.Collections;
using Units;
using Units.Actions.VisualPlayback;
using UnityEngine;

public class SummonCreeperCupidActionVisualHandler : IActionVisualHandler
{
    private readonly UnitViewManager _unitViewManager;
    private bool _isSummoned;
    private bool _isFinished;
    
    public SummonCreeperCupidActionVisualHandler()
    {
        _unitViewManager = Locator.Get<UnitViewManager>();
    }
    
    public IEnumerator PlayCor(ActionPerformResult result)
    {
        var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
        unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isSummoned = true);
        unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnFinish, () => _isFinished = true);
        unitPrefab.Animator.SetTrigger(AnimationConstants.Summon);
        yield return new WaitUntil(() => _isSummoned);
        _unitViewManager.OnUnitSpawned(result.TargetSpace.Occupant);
        yield return new WaitUntil(() => _isFinished);
    }
}
