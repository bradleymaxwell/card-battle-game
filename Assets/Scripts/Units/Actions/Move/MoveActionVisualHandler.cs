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
        private readonly UnitSpawner _unitSpawner;
        private readonly MapService _mapService;
        
        public MoveActionVisualHandler(MoveActionVisualConfig config)
        {
            _config = config;
            _unitSpawner = Locator.Get<UnitSpawner>();
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

            var unitPrefab = _unitSpawner.GetUnitPrefab(result.Performer);
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

            var unitTransform = unitPrefab.transform;
            var targetPosition = new Vector3(
                mapSpacePrefab.transform.position.x,
                unitTransform.position.y,
                mapSpacePrefab.transform.position.z);

            while (Vector3.Distance(unitTransform.position, targetPosition) > 0.01f)
            {
                unitTransform.position = Vector3.MoveTowards(
                    unitTransform.position,
                    targetPosition,
                    _config.Speed * Time.deltaTime);

                yield return null;
            }

            unitTransform.position = targetPosition;
        }
    }
}