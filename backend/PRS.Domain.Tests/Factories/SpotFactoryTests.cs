using FluentAssertions;

using PRS.Domain.Enums;
using PRS.Domain.Factories;
using PRS.Domain.Tests.Stubs;

namespace PRS.Domain.Tests.Factories;

public class SpotFactoryTests
{
    [Fact]
    public async Task Create_WhenKeyIsDuplicate_ReturnsFailure()
    {
        var spec = new FakeSpotKeyUniqueSpec(allowed: false);
        var factory = new SpotFactory(spec);

        var result = await factory.Create("X01", []);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Spot.DuplicateKey");
    }

    [Fact]
    public async Task Create_WhenKeyIsUnique_AddsCapabilities()
    {
        var spec = new FakeSpotKeyUniqueSpec(allowed: true);
        var factory = new SpotFactory(spec);

        var caps = new List<SpotCapability> { SpotCapability.ElectricCharger };
        var result = await factory.Create("X02", caps);

        result.IsSuccess.Should().BeTrue();
        result.Value.Capabilities.Should().Contain(SpotCapability.ElectricCharger);
    }
}