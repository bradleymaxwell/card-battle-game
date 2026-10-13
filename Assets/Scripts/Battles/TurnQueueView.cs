using System.Collections.Generic;
using System.Linq;
using AI.Passive;
using Battles;
using Units;
using UnityEngine;

public class TurnQueueView : MonoBehaviour
{
    [SerializeField] private TurnView turnPrefab;
    [SerializeField] private int displayLimit;
    private PoolService _poolService;
    private readonly IList<TurnView> _turnInstances = new List<TurnView>();
    
    private void Awake()
    {
        _poolService = Locator.Get<PoolService>();
    }

    public void Refresh(IUnit activeUnit, IList<IUnit> turnQueue, IList<IUnit> turnOrder)
    {
        // calculate the turns to be displayed
        var displayUnits = new List<IUnit>();
        if (activeUnit != null)
        {
            if (activeUnit is not NpcUnit { Brain: PassiveUnitBrain })
            {
                displayUnits.Add(activeUnit);
            }
        }

        for (var i = 0; i < turnQueue.Count && displayUnits.Count < displayLimit; i++)
        {
            var unit = turnQueue[i];
            if (unit is not NpcUnit { Brain: PassiveUnitBrain })
            {
                displayUnits.Add(unit);
            }
        }

        if (turnOrder.Count > 0)
        {
            var turnOrderIndex = 0;
            while (displayUnits.Count < displayLimit)
            {
                var unit = turnOrder[turnOrderIndex % turnOrder.Count];
                if (unit is not NpcUnit { Brain: PassiveUnitBrain })
                {
                    displayUnits.Add(unit);
                }
                
                turnOrderIndex++;
            }
        }
        
        // get the view instances and set them based on the order to show
        while (_turnInstances.Count < displayUnits.Count)
        {
            var instance = _poolService.Get(turnPrefab);
            _turnInstances.Add(instance);
            instance.transform.SetParent(transform, false);
        }

        while (_turnInstances.Count > displayUnits.Count)
        {
            var lastIndex = _turnInstances.Count - 1;
            var instance = _turnInstances[lastIndex];
            _poolService.Return(instance);
            _turnInstances.RemoveAt(lastIndex);
        }

        for (var i = 0; i < displayUnits.Count; i++)
        {
            var unit = displayUnits[i];
            var turnView = _turnInstances[i];
            turnView.Set(unit, i == 0);
        }
    }
}
