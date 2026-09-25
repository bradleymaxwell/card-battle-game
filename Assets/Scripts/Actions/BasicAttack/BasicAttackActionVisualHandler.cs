using System;
using System.Collections;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units
{
    public class BasicAttackActionVisualHandler : IActionVisualHandler
    {
        private readonly UnitViewManager _unitViewManager;
        private bool _isHitLanded;
        private bool _isAttackFinished;
        
        public BasicAttackActionVisualHandler()
        {
            _unitViewManager = Locator.Get<UnitViewManager>();
        }
        
        public IEnumerator PlayCor(ActionPerformResult result)
        {
            _isHitLanded = false;
            _isAttackFinished = false;
            var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
            var (targetPrefab, _) = _unitViewManager.GetUnitViews(result.Target);
            
            unitPrefab.Face(targetPrefab);
            unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnHit, () => _isHitLanded = true);
            unitPrefab.SubscribeToAnimationEvent(AnimationConstants.OnFinish, () => _isAttackFinished = true);
            unitPrefab.Animator.SetTrigger(AnimationConstants.Attack);
            yield return new WaitUntil(() => _isHitLanded);
            _unitViewManager.OnHealthAdjusted(result.Target, result.HealthAdjustmentByUnit[result.Target]);
            
            yield return new WaitUntil(() => _isAttackFinished);
        }
    }
}