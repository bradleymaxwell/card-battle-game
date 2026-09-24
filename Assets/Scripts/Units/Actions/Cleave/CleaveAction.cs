using System.Linq;
using Map;
using Units;
using Units.Cleave;
using UnityEngine;

public class CleaveAction : Action
{
    private readonly ICleaveActionConfigProvider _config;
    private readonly IMapService _mapService;
    private readonly IUnitService _unitService;
    
    public CleaveAction(ICleaveActionConfigProvider config) : this(
        config, 
        Locator.Get<MapService>(),
        Locator.Get<UnitService>())
    {
    }
    
    public CleaveAction(
        ICleaveActionConfigProvider config, 
        IMapService mapService,
        IUnitService unitService) : base(config)
    {
        _config = config;
        _mapService = mapService;
        _unitService = unitService;
    }

    public override bool CanPerform(MapSpace userSpace, MapSpace targetSpace)
    {
        return base.CanPerform(userSpace, targetSpace) 
               && targetSpace.IsNeighbourOf(userSpace) 
               && targetSpace.Occupant != null 
               && targetSpace.Occupant.Team != userSpace.Occupant.Team;
    }

    public override ActionPerformResult OnPerform(MapSpace userSpace, MapSpace targetSpace)
    {
        var result = new ActionPerformResult();
        var direction = userSpace.GetDirectionTo(targetSpace);
        var left1 = direction.GetLeft();
        var left2 = left1.GetLeft();
        
        var left1Offset = left1.GetOffset();
        var left2Offset = left2.GetOffset();
        
        var left1Space = _mapService.GetSpace(left1Offset.q, left1Offset.r);
        var left2Space = _mapService.GetSpace(left2Offset.q, left2Offset.r);

        var spaces = new[] { left2Space, left1Space, targetSpace };
        var hitSpaces = spaces.Where(space => space?.Occupant != null && space.Occupant.Team != userSpace.Occupant.Team).ToList();
        result.IsPerfectHit = hitSpaces.Count >= _config.PerfectHitThreshold;
        var modifierIndex = -1;
        foreach (var space in hitSpaces)
        {
            float damage = _config.BaseDamage;
            if (modifierIndex >= 0)
            {
                var modifier = _config.DamagePercentIncreasePerHit.ElementAt(modifierIndex);
                damage += 1f + modifier;
            }
            
            if (result.IsPerfectHit)
            {
                damage *= 1f + _config.PerfectHitDamagePercentIncrease;
            }
            
            var roundedDamage = Mathf.CeilToInt(damage);
            _unitService.Damage(space.Occupant, roundedDamage);
            modifierIndex++;
            result.HealthAdjustmentByUnit.Add(space.Occupant, -roundedDamage);
        }

        return result;
    }
}
