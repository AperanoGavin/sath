using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetAllReservationsHandlerTests
{
    private readonly Mock<IReservationRepository> _mockRepo = new();
    private readonly GetAllReservationsHandler _handler;

    public GetAllReservationsHandlerTests()
    {
        _handler = new GetAllReservationsHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenNoReservationsExist_ShouldReturnEmpty()
    {
        // Arrange
        _mockRepo.Setup(static r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetAllReservationsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenReservationsExist_ShouldMapToDtos()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S10");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Zoe", "zoe@ex.com", role);
        var from = DateTime.Today;
        var to = from.AddDays(1);
        var reservation = new Reservation(spot, user, from, to);

        _mockRepo.Setup(static r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([reservation]);

        // Act
        var result = await _handler.Handle(new GetAllReservationsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value!.ToList();
        dtos.Count.Should().Be(1);
        dtos[0].Id.Should().Be(reservation.Id.ToString());
        dtos[0].SpotId.Should().Be(spot.Id.ToString());
        dtos[0].UserId.Should().Be(user.Id.ToString());
    }
}