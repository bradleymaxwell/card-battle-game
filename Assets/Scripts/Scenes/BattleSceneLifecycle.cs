using Battles;
using BinhoGames.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleSceneLifecycle : SceneLifecycle
{
    [SerializeField] private MapSpaceContainer mapSpaceContainer;
    [SerializeField] private BattleConfig battleConfig;
    private BattleService _battleService;

    public override void Initialize()
    {
        base.Initialize();
        _battleService = Locator.Get<BattleService>();
    }

    public override UniTask OnBeforeShowAsync(SceneData sceneData)
    {
        _battleService.Initialize(battleConfig, mapSpaceContainer);
        return UniTask.CompletedTask;
    }

    public override UniTask OnShowAsync(SceneData sceneData)
    {
        return UniTask.CompletedTask;
    }

    public override UniTask OnHideAsync(SceneData sceneData)
    {
        return UniTask.CompletedTask;
    }

    public override UniTask OnAfterHideAsync(SceneData sceneData)
    {
        return UniTask.CompletedTask;
    }

    public override void HideImmediate(SceneData sceneData)
    {
        return;
    }
}
