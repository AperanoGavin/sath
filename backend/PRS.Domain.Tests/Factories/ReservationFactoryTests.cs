using FluentAssertions;

using PRS.Domain.Entities;
using PRS.Domain.Factories;
using PRS.Domain.Tests.Stubs;

namespace PRS.Domain.Tests.Factories;

public class ReservationFactoryTests
{
    private readonly Spot _spot;
    private readonly User _user;

    public ReservationFactoryTests()
    {
        _spot = Spot.Create("Z01").Value;
        var role = new UserRole(
                key: "Employee",
                name: "Employee",
                description: "");

        _user = User.Create("Alice", "alice@acme.com", role).Value;
    }

    [Fact]
    public async Task Create_WhenSpotNotFound_ReturnsFailure()
    {
        // Arrange
        var factory = new ReservationFactory(
                new StubSpotRepo(null),
                new StubUserRepo(_user),
                new FakeOverlapSpec(true));

        // Act
        var result = await factory.Create(
                Guid.NewGuid(),
                _user.Id,
                DateTime.Today,
                DateTime.Today.AddDays(1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Reservation.NotFound");
    }

    [Fact]
    public async Task Create_WhenOverlapSpecFails_ReturnsFailure()
    {
        // Arrange
        var factory = new ReservationFactory(
                new StubSpotRepo(_spot),
                new StubUserRepo(_user),
                new FakeOverlapSpec(false));

        // Act
        var result = await factory.Create(
                _spot.Id,
                _user.Id,
                DateTime.Today,
                DateTime.Today.AddDays(1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Reservation.Overlap");
    }

    [Fact]
    public async Task Create_ValidInputs_ReturnsSuccess()
    {
        // Arrange
        var factory = new ReservationFactory(
                new StubSpotRepo(_spot),
                new StubUserRepo(_user),
                new FakeOverlapSpec(true));

        // Act
        var result = await factory.Create(
                _spot.Id,
                _user.Id,
                DateTime.Today,
                DateTime.Today.AddDays(1));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Spot.Should().BeSameAs(_spot);
        result.Value.User.Should().BeSameAs(_user);
        result.Value.From.Should().Be(DateTime.Today);
        result.Value.To.Should().Be(DateTime.Today.AddDays(1));
    }
}