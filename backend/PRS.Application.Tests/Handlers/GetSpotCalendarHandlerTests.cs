using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetSpotCalendarHandlerTests
{
    private readonly Mock<IReservationRepository> _mockRepo = new();
    private readonly GetSpotCalendarHandler _handler;

    public GetSpotCalendarHandlerTests()
    {
        _handler = new GetSpotCalendarHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenNoReservationsForSpot_ShouldReturnEmpty()
    {
        // Arrange
        var spotId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetBySpotAsync(spotId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetSpotCalendarQuery(spotId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenReservationsExist_ShouldMapToDtos()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S40");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Fred", "fred@ex.com", role);
        var from = DateTime.Today.AddDays(1);
        var to = from.AddDays(1);
        var res1 = new Reservation(spot, user, from, to);

        _mockRepo.Setup(r => r.GetBySpotAsync(spot.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync([res1]);

        // Act
        var result = await _handler.Handle(new GetSpotCalendarQuery(spot.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value!.ToList();
        dtos.Count.Should().Be(1);
        dtos[0].Id.Should().Be(res1.Id.ToString());
        dtos[0].SpotId.Should().Be(spot.Id.ToString());
        dtos[0].UserId.Should().Be(user.Id.ToString());
    }
}