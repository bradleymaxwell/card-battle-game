using AI;
using DefaultNamespace.Indicators;
using UnityEngine;

public class UnitIndicatorBarView : MonoBehaviour
{
    [SerializeField] private IntentionIndicatorView intentionPrefab;
    private PoolService _poolService;
    private IntentionIndicatorView _indicator;

    private void Awake()
    {
        _poolService = Locator.Get<PoolService>();
    }

    public void ShowIntention(UnitTurnIntention intention)
    {
        if (_indicator)
        {
            return;
        }

        var indicator = _poolService.Get(intentionPrefab);
        indicator.Bind(intention);
        indicator.transform.SetParent(transform, false);
        indicator.gameObject.SetActive(true);
        _indicator = indicator;
    }

    public void HideIntention()
    {
        if (!_indicator)
        {
            return;
        }
        
        _poolService.Return(_indicator);
        _indicator = null;
    }
}
