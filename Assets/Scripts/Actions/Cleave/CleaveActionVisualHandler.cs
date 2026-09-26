using System;
using System.Collections;
using Units;
using Units.Actions.VisualPlayback;
using UnityEngine;

public class CleaveActionVisualHandler : IActionVisualHandler
{
    private readonly UnitViewManager _unitViewManager;
    private readonly PopupTextManager _popupTextManager;
    private bool _isHitLanded;
    
    public CleaveActionVisualHandler()
    {
        _unitViewManager = Locator.Get<UnitViewManager>();
        _popupTextManager = Locator.Get<PopupTextManager>();
    }
    
    public IEnumerator PlayCor(ActionPerformResult result)
    {
        _isHitLanded = false;
        var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
        var (targetPrefab, _) = _unitViewManager.GetUnitViews(result.Target);
        var targetPrefabPosition = targetPrefab.transform.position;
        unitPrefab.Face(targetPrefab);
        unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isHitLanded = true);
        unitPrefab.Animator.SetTrigger(AnimationConstants.SideSlash);
        yield return new WaitUntil(() => _isHitLanded);
        foreach (var (target, adjustment) in result.HealthAdjustmentByUnit)
        {
            _unitViewManager.OnHealthAdjusted(target, adjustment);
        }

        if (result.IsPerfectHit)
        {
            _popupTextManager.Display("Perfect!", targetPrefabPosition);
        }
    }
}
