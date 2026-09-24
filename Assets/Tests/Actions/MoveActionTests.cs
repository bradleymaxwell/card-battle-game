using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using FakeItEasy;
using Map;
using NUnit.Framework;
using Units;

[TestFixture]
public class MoveActionTests
{
    private IMoveActionConfigProvider _configProvider;
    private IMapService _mapService;
    private MoveAction _sut;
    
    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization()
        {
            ConfigureMembers = true
        });
        
        _configProvider = fixture.Freeze<IMoveActionConfigProvider>();
        _mapService = fixture.Freeze<IMapService>();
        fixture.Register(() => new MoveAction(_configProvider, _mapService));
        _sut = fixture.Create<MoveAction>();
    }
    
    [Test]
    public void CanPerformReturnsFalseWhenTargetSpaceHasOccupant()
    {
        // Arrange
        var userSpace = A.Fake<MapSpace>();
        var targetSpace = A.Fake<MapSpace>();
        targetSpace.Occupant = A.Fake<IUnit>(); 
        
        // Act
        var result = _sut.CanPerform(userSpace, targetSpace);
        
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void GetEnergyCostReturnsModifiedValueBasedOnPath()
    {
        // Arrange
        const int energyCost = 1;
        var shortestPath = new List<MapSpace> { A.Fake<MapSpace>(), A.Fake<MapSpace>(), A.Fake<MapSpace>() };
        var unit = A.Fake<IUnit>();
        var userSpace = A.Fake<MapSpace>();
        var targetSpace = A.Fake<MapSpace>();
        
        userSpace.Occupant = unit;
        A.CallTo(() => unit.CurrentEnergy).Returns(shortestPath.Count * energyCost + 1);
        A.CallTo(() => _configProvider.EnergyCost).Returns(energyCost);
        A.CallTo(() => _mapService.GetShortestPath(userSpace, targetSpace, A<int>.Ignored, A<bool>.Ignored)).Returns(shortestPath);
        
        // Act
        var result = _sut.GetEnergyCost(userSpace, targetSpace);
        
        // Assert
        Assert.That(result, Is.EqualTo(shortestPath.Count * energyCost));
    }
    
    [Test]
    public void CanPerformReturnsFalseWhenUnitCannotAffordTotalPathCost()
    {
        // Arrange
        const int energyCost = 1;
        var shortestPath = new List<MapSpace> { A.Fake<MapSpace>(), A.Fake<MapSpace>(), A.Fake<MapSpace>() };
        var unit = A.Fake<IUnit>();
        var userSpace = A.Fake<MapSpace>();
        var targetSpace = A.Fake<MapSpace>();
        
        userSpace.Occupant = unit;
        A.CallTo(() => unit.CurrentEnergy).Returns(energyCost);
        A.CallTo(() => _configProvider.EnergyCost).Returns(energyCost);
        A.CallTo(() => _mapService.GetShortestPath(userSpace, targetSpace, A<int>.Ignored, A<bool>.Ignored)).Returns(shortestPath);
        
        // Act
        var result = _sut.CanPerform(userSpace, targetSpace);
        
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void CanPerformReturnsFalseIfNoShortestPathFound()
    {
        // Arrange
        const int energyCost = 1;
        var noPath = new List<MapSpace>();
        var unit = A.Fake<IUnit>();
        var userSpace = A.Fake<MapSpace>();
        var targetSpace = A.Fake<MapSpace>();
        
        userSpace.Occupant = unit;
        A.CallTo(() => unit.CurrentEnergy).Returns(energyCost);
        A.CallTo(() => _configProvider.EnergyCost).Returns(energyCost);
        A.CallTo(() => _mapService.GetShortestPath(userSpace, targetSpace, A<int>.Ignored, A<bool>.Ignored)).Returns(noPath);
        
        // Act
        var result = _sut.CanPerform(userSpace, targetSpace);
        
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void OnPerformMovesUnitToTargetSpaceWhenCanPerformReturnsTrue()
    {
        // Arrange
        const int energyCost = 1;
        var shortestPath = new List<MapSpace> { A.Fake<MapSpace>(), A.Fake<MapSpace>(), A.Fake<MapSpace>() };
        var unit = A.Fake<IUnit>();
        var userSpace = A.Fake<MapSpace>();
        var targetSpace = new MapSpace(2, 8);
        
        userSpace.Occupant = unit;
        A.CallTo(() => unit.CurrentEnergy).Returns(energyCost * shortestPath.Count + 1);
        A.CallTo(() => _configProvider.EnergyCost).Returns(energyCost);
        A.CallTo(() => _mapService.GetShortestPath(userSpace, targetSpace, A<int>.Ignored, A<bool>.Ignored)).Returns(shortestPath);
        
        // Act
        var result = _sut.OnPerform(userSpace, targetSpace);
        
        // Assert
        A.CallTo(() => _mapService.Move(unit, targetSpace.Q, targetSpace.R)).MustHaveHappened();
        var moveResult = result as MoveActionPerformResult;
        Assert.That(moveResult, Is.Not.Null);
        Assert.That(moveResult.EnergyConsumed, Is.EqualTo(energyCost * shortestPath.Count));
        Assert.That(moveResult.Path, Is.EqualTo(shortestPath));
    }
}
