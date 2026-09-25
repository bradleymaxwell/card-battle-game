using System.Collections.Generic;
using Battles;
using UnityEngine;

namespace Units
{
    public class ActionViewContainer : MonoBehaviour
    {
        [SerializeField] private ActionView actionViewPrefab;
        private PoolService _poolService;
        private readonly IList<ActionView> _actionViews = new List<ActionView>();
        
        private void Awake()
        {
            _poolService = Locator.Get<PoolService>();
        }

        public void OnActiveUnitChanged(TeamType team, IUnit unit)
        {
            if (team != TeamType.Player || unit == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            // get enough prefabs for all actions for the unit
            if (unit.Config.Actions.Count < _actionViews.Count)
            {
                while (_actionViews.Count != unit.Config.Actions.Count)
                {
                    var actionView = _actionViews[^1];
                    _actionViews.Remove(actionView);
                    _poolService.Return(actionView);
                }
            }
            else if (unit.Config.Actions.Count > _actionViews.Count)
            {
                while (_actionViews.Count != unit.Config.Actions.Count)
                {
                    var actionView = _poolService.Get(actionViewPrefab);
                    actionView.transform.SetParent(transform, false);
                    _actionViews.Add(actionView);
                }
            }

            for (var i = 0; i < unit.Config.Actions.Count; i++)
            {
                var actionView = _actionViews[i];
                actionView.Bind(unit, unit.Actions[i]);
            }
            
            gameObject.SetActive(true);
        }
    }
}