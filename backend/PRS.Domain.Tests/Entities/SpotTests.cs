using FluentAssertions;

using PRS.Domain.Entities;
using PRS.Domain.Enums;

namespace PRS.Domain.Tests.Entities;

public class SpotTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidKey_ReturnsFailure(string invalidKey)
    {
        // Act
        var result = Spot.Create(invalidKey);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Spot.InvalidKey");
    }

    [Fact]
    public void Create_ValidKey_ReturnsSuccessAndSetsKey()
    {
        // Arrange
        var key = "A01";

        // Act
        var result = Spot.Create(key);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Key.Should().Be(key);
        result.Value.Capabilities.Should().BeEmpty();
    }

    [Fact]
    public void AddCapability_Duplicate_ReturnsFailure()
    {
        // Arrange
        var spot = Spot.Create("B01").Value;
        spot.AddCapability(SpotCapability.ElectricCharger).IsSuccess.Should().BeTrue();

        // Act
        var second = spot.AddCapability(SpotCapability.ElectricCharger);

        // Assert
        second.IsFailure.Should().BeTrue();
        second.Error!.Code.Should().Be("Spot.DuplicateCapability");
    }

    [Fact]
    public void AddCapability_NewCapability_ReturnsSuccessAndAddsIt()
    {
        // Arrange
        var spot = Spot.Create("C01").Value;

        // Act
        var result = spot.AddCapability(SpotCapability.ElectricCharger);

        // Assert
        result.IsSuccess.Should().BeTrue();
        spot.Capabilities.Should().ContainSingle()
            .Which.Should().Be(SpotCapability.ElectricCharger);
    }
}
