using Battles;
using Map;
using Units;

public class BattleService
{
    private readonly MapService _mapService;
    private readonly UnitService _unitService;
    
    public BattleService() : this(Locator.Get<MapService>(), Locator.Get<UnitService>())
    {
    }
    
    public BattleService(MapService mapService, UnitService unitService)
    {
        _mapService = mapService;
        _unitService = unitService;
    }
    
    public void Initialize(BattleConfig battleConfig, MapSpaceContainer mapSpaceContainer)
    {
        _mapService.Initialize(mapSpaceContainer);
        foreach (var unit in battleConfig.EnemyUnits)
        {
            _unitService.Spawn(unit);
        }
    }
}
