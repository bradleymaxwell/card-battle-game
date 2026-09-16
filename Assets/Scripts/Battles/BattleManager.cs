using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private UnitViewManager unitViewManager;
    [SerializeField] private VisualPlaybackManager visualPlaybackManager;
    private BattleService _battleService;

    private void Awake()
    {
        _battleService = Locator.Get<BattleService>();
    }
    
    private void Start()
    {
        StartCoroutine(StartCor());
    }

    private IEnumerator StartCor()
    {
        yield return new WaitUntil(() => _battleService.IsInitialized);
        unitViewManager.Initialize();
        visualPlaybackManager.Initialize();
        _battleService.StartNextTurn();
    }
}
