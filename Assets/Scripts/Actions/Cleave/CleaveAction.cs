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
        var relativeLeft = direction.GetLeft();
        var relativeLeftDown = relativeLeft.GetLeft();
        
        var leftOffset = relativeLeft.GetOffset();
        var leftDownOffset = relativeLeftDown.GetOffset();
        
        var leftSpace = _mapService.GetSpace(userSpace.Q + leftOffset.q, userSpace.R + leftOffset.r);
        var leftDownSpace = _mapService.GetSpace(userSpace.Q +leftDownOffset.q, userSpace.R + leftDownOffset.r);

        var spaces = new[] { leftDownSpace, leftSpace, targetSpace };
        var hitSpaces = spaces.Where(space => space?.Occupant != null && space.Occupant.Team != userSpace.Occupant.Team).ToList();
        result.IsPerfectHit = hitSpaces.Count >= _config.PerfectHitThreshold;
        var modifierIndex = 0;
        foreach (var space in hitSpaces)
        {
            var target = space.Occupant;
            var modifier = _config.PercentDamagePerHit.ElementAt(modifierIndex);
            float damage = userSpace.Occupant.Config.Attack;
            damage *= modifier;
            if (result.IsPerfectHit)
            {
                damage *= 1f + _config.PerfectHitDamagePercentIncrease;
            }
            
            var roundedDamage = Mathf.CeilToInt(damage);
            _unitService.Damage(target, roundedDamage);
            modifierIndex++;
            result.HealthAdjustmentByUnit.Add(target, -roundedDamage);
        }

        return result;
    }
}
