using System;
using System.Collections;
using Map;
using Units.Actions.VisualPlayback;
using UnityEngine;

namespace Units
{
    public class MoveActionVisualHandler : IActionVisualHandler
    {
        private readonly Logger _logger = new(nameof(MoveActionVisualHandler));
        private readonly MoveActionVisualConfig _config;
        private readonly UnitViewManager _unitViewManager;
        private readonly MapService _mapService;
        
        public MoveActionVisualHandler(MoveActionVisualConfig config)
        {
            _config = config;
            _unitViewManager = Locator.Get<UnitViewManager>();
            _mapService = Locator.Get<MapService>();
        }

        public IEnumerator PlayCor(ActionPerformResult result)
        {
            if (result is not MoveActionPerformResult moveResult)
            {
                _logger.LogError($"in order to play the move action visuals, the result must be of type: {typeof(MoveActionPerformResult)}");
                yield break;
            }

            if (moveResult.Path is not { Count: > 0 })
            {
                _logger.LogWarning("no path present in the move action result, so nothing to play");
                yield break;
            }

            var (unitPrefab, _) = _unitViewManager.GetUnitViews(result.Performer);
            if (unitPrefab == null)
            {
                _logger.LogError($"no unit prefab instance found for unit: {result.Performer} and therefore cannot playback move");
                yield break;
            }
            
            foreach (var space in moveResult.Path)
            {
                yield return MoveToSpaceCor(unitPrefab, space);
            }
        }
        
        private IEnumerator MoveToSpaceCor(UnitPrefab unitPrefab, MapSpace space)
        {
            var mapSpacePrefab = _mapService.GetPrefab(space);
            if (mapSpacePrefab == null)
            {
                yield break;
            }

            unitPrefab.Face(mapSpacePrefab);
            unitPrefab.Animator.SetBool(AnimationConstants.IsMoving, true);
            while (Vector3.Distance(unitPrefab.transform.position, mapSpacePrefab.transform.position) > 0.01f)
            {
                unitPrefab.transform.position = Vector3.MoveTowards(
                    unitPrefab.transform.position,
                    mapSpacePrefab.transform.position,
                    _config.Speed * Time.deltaTime);

                yield return null;
            }
            
            unitPrefab.Animator.SetBool(AnimationConstants.IsMoving, false);
            unitPrefab.transform.position = mapSpacePrefab.transform.position;
        }
    }
}