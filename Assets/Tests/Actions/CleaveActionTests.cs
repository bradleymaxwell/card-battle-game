using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using Battles;
using FakeItEasy;
using Map;
using NUnit.Framework;
using Units;
using Units.Cleave;

[TestFixture]
public class CleaveActionTests
{
    private ICleaveActionConfigProvider _configProvider;
    private IMapService _mapService;
    private IUnitService _unitService;
    private CleaveAction _sut;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization()
        {
            ConfigureMembers = true
        });

        _configProvider = fixture.Freeze<ICleaveActionConfigProvider>();
        _mapService = fixture.Freeze<IMapService>();
        _unitService = fixture.Freeze<IUnitService>();

        fixture.Register(() => new CleaveAction(_configProvider, _mapService, _unitService));
        _sut = fixture.Create<CleaveAction>();
    }

    [Test]
    public void CanPerformReturnsFalseWhenTargetSpaceIsNoAdjacentToUserSpace()
    {
        // Arrange
        var user = CreateUnit(TeamType.Player);
        var target = CreateUnit(TeamType.Enemy);
        var userSpace = new MapSpace(0, 0)
        {
            Occupant = user
        };
        var targetSpace = new MapSpace(2, 0)
        {
            Occupant = target
        };

        A.CallTo(() => user.CurrentEnergy).Returns(1);
        A.CallTo(() => _configProvider.EnergyCost).Returns(1);

        // Act
        var result = _sut.CanPerform(userSpace, targetSpace);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void OnPerformReturnsUnmodifiedDamageWhenOnlyOneTargetHit()
    {
        // Arrange
        const int attack = 10;
        var user = CreateUnit(TeamType.Player, attack);
        var target = CreateUnit(TeamType.Enemy);
        var userSpace = new MapSpace(0, 0)
        {
            Occupant = user
        };
        
        var targetOffset = MapDirection.Right.GetOffset();
        var targetSpace = new MapSpace(targetOffset.q, targetOffset.r)
        {
            Occupant = target
        };
        
        var leftOffset = MapDirection.RightUp.GetOffset();
        var leftDownOffset = MapDirection.LeftUp.GetOffset();
        
        A.CallTo(() => _mapService.GetSpace(leftOffset.q, leftOffset.r)).Returns(null);
        A.CallTo(() => _mapService.GetSpace(leftDownOffset.q, leftDownOffset.r)).Returns(new MapSpace(leftDownOffset.q, leftDownOffset.r));
        A.CallTo(() => _configProvider.PercentDamagePerHit).Returns(new List<float> { 0.3f, 0.6f });
        A.CallTo(() => _configProvider.PerfectHitThreshold).Returns(3);
        
        // Act
        var result = _sut.OnPerform(userSpace, targetSpace);

        // Assert
        A.CallTo(() => _unitService.Damage(target, 3)).MustHaveHappenedOnceExactly();
        Assert.That(result.HealthAdjustmentByUnit[target], Is.EqualTo(-3));
        Assert.That(result.IsPerfectHit, Is.False);
    }

    [Test]
    public void OnPerformReturnsExpectedModifiedDamageWhenMoreThanOneTargetHit()
    {
        // Arrange
        const int attack = 10;
        var user = CreateUnit(TeamType.Player, attack);
        var firstTarget = CreateUnit(TeamType.Enemy);
        var secondTarget = CreateUnit(TeamType.Enemy);
        var userSpace = new MapSpace(0, 0)
        {
            Occupant = user
        };
        
        var targetOffset = MapDirection.LeftDown.GetOffset();
        var targetSpace = new MapSpace(targetOffset.q, targetOffset.r)
        {
            Occupant = secondTarget
        };
        
        var leftDownOffset = MapDirection.Right.GetOffset();
        var leftDownSpace = new MapSpace(0, -1)
        {
            Occupant = firstTarget
        };

        A.CallTo(() => _mapService.GetSpace(leftDownOffset.q, leftDownOffset.r)).Returns(leftDownSpace);
        A.CallTo(() => _configProvider.PercentDamagePerHit).Returns(new List<float> { 0.3f, 0.6f });
        A.CallTo(() => _configProvider.PerfectHitThreshold).Returns(3);

        // Act
        var result = _sut.OnPerform(userSpace, targetSpace);

        // Assert
        A.CallTo(() => _unitService.Damage(firstTarget, 3)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitService.Damage(secondTarget, 6)).MustHaveHappenedOnceExactly();
        Assert.That(result.HealthAdjustmentByUnit[firstTarget], Is.EqualTo(-3));
        Assert.That(result.HealthAdjustmentByUnit[secondTarget], Is.EqualTo(-6));
        Assert.That(result.IsPerfectHit, Is.False);
    }

    [Test]
    public void OnPerformReturnsPerfectHitWhenPerfectHitThresholdSucceeded()
    {
        // Arrange
        var user = CreateUnit(TeamType.Player, attack: 10);
        var firstTarget = CreateUnit(TeamType.Enemy);
        var secondTarget = CreateUnit(TeamType.Enemy);
        var thirdTarget = CreateUnit(TeamType.Enemy);
        var userSpace = new MapSpace(0, 0)
        {
            Occupant = user
        };

        var targetSpaceOffset = MapDirection.LeftUp.GetOffset();
        var targetSpace = new MapSpace(targetSpaceOffset.q, targetSpaceOffset.r)
        {
            Occupant = thirdTarget
        };
        
        var left1Offset = MapDirection.Left.GetOffset();
        var left1Space = new MapSpace(left1Offset.q, left1Offset.r)
        {
            Occupant = secondTarget
        };
        
        var left2Offset = MapDirection.LeftDown.GetOffset();
        var left2Space = new MapSpace(left2Offset.q, left2Offset.r)
        {
            Occupant = firstTarget
        };

        A.CallTo(() => _mapService.GetSpace(left1Offset.q, left1Offset.r)).Returns(left1Space);
        A.CallTo(() => _mapService.GetSpace(left2Offset.q, left2Offset.r)).Returns(left2Space);
        A.CallTo(() => _configProvider.PercentDamagePerHit).Returns(new List<float> { 0.3f, 0.6f, 0.9f });
        A.CallTo(() => _configProvider.PerfectHitThreshold).Returns(3);
        A.CallTo(() => _configProvider.PerfectHitDamagePercentIncrease).Returns(0.5f);

        // Act
        var result = _sut.OnPerform(userSpace, targetSpace);

        // Assert
        Assert.That(result.IsPerfectHit, Is.True);
    }

    [Test]
    public void OnPerformReturnsIncreasedModifiedDamageWhenPerfectHit()
    {
        // Arrange
        const int attack = 10;
        var user = CreateUnit(TeamType.Player, attack);
        var firstTarget = CreateUnit(TeamType.Enemy);
        var secondTarget = CreateUnit(TeamType.Enemy);
        var thirdTarget = CreateUnit(TeamType.Enemy);
        var userSpace = new MapSpace(0, 0)
        {
            Occupant = user
        };
        
        var targetOffset = MapDirection.Right.GetOffset();
        var targetSpace = new MapSpace(targetOffset.q, targetOffset.r)
        {
            Occupant = thirdTarget
        };
        
        var leftOffset = MapDirection.RightUp.GetOffset();
        var leftSpace = new MapSpace(leftOffset.q, leftOffset.r)
        {
            Occupant = secondTarget
        };
        
        var leftDownOffset = MapDirection.LeftUp.GetOffset();
        var leftDownSpace = new MapSpace(leftDownOffset.q, leftDownOffset.r)
        {
            Occupant = firstTarget
        };

        A.CallTo(() => _mapService.GetSpace(leftOffset.q, leftDownOffset.r)).Returns(leftSpace);
        A.CallTo(() => _mapService.GetSpace(leftDownOffset.q, leftDownOffset.r)).Returns(leftDownSpace);
        A.CallTo(() => _configProvider.PercentDamagePerHit).Returns(new List<float> { 0.3f, 0.6f, 0.9f });
        A.CallTo(() => _configProvider.PerfectHitThreshold).Returns(3);
        A.CallTo(() => _configProvider.PerfectHitDamagePercentIncrease).Returns(0.5f);

        // Act
        var result = _sut.OnPerform(userSpace, targetSpace);

        // Assert
        A.CallTo(() => _unitService.Damage(firstTarget, 5)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitService.Damage(secondTarget, 9)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitService.Damage(thirdTarget, 14)).MustHaveHappenedOnceExactly();
        Assert.That(result.HealthAdjustmentByUnit[firstTarget], Is.EqualTo(-5));
        Assert.That(result.HealthAdjustmentByUnit[secondTarget], Is.EqualTo(-9));
        Assert.That(result.HealthAdjustmentByUnit[thirdTarget], Is.EqualTo(-14));
        Assert.That(result.IsPerfectHit, Is.True);
    }

    private static IUnit CreateUnit(TeamType team, int attack = 10)
    {
        var unit = A.Fake<IUnit>();
        var config = A.Fake<IUnitConfigProvider>();

        A.CallTo(() => config.Attack).Returns(attack);
        A.CallTo(() => unit.Config).Returns(config);
        A.CallTo(() => unit.Team).Returns(team);

        return unit;
    }
}