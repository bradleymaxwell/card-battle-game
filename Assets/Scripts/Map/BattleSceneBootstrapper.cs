using Battles;
using UnityEngine;

public class BattleSceneBootstrapper : SceneBootstrapper
{
    [SerializeField] private MapSpaceContainer mapSpaceContainer;
    [SerializeField] private BattleConfig battleConfig;
    protected override BootstrapType BootstrapType => BootstrapType.Sync;
    
    protected override void OnBootstrap()
    {
        base.OnBootstrap();
        var battleService = Locator.Get<BattleService>();
        battleService.Initialize(battleConfig, mapSpaceContainer);
    }
}
