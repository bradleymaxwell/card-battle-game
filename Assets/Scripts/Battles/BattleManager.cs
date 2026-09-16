using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private UnitViewManager _unitViewManager;
    private ActionVisualPlaybackManager _actionVisualPlaybackManager;
    private BattleService _battleService;

    private void Awake()
    {
        _battleService = Locator.Get<BattleService>();
    }
    
    private async UniTaskVoid Start()
    {
        _unitViewManager = Locator.Get<UnitViewManager>();
        _actionVisualPlaybackManager = Locator.Get<ActionVisualPlaybackManager>();
        await UniTask.WaitUntil(() => _battleService.IsInitialized);
        _unitViewManager.Initialize();
        _actionVisualPlaybackManager.Initialize();
        _battleService.StartNextTurn();
    }
}
