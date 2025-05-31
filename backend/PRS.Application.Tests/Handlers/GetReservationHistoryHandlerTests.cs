using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetReservationHistoryHandlerTests
{
    private readonly Mock<IReservationRepository> _mockRepo = new();
    private readonly GetReservationHistoryHandler _handler;

    public GetReservationHistoryHandlerTests()
    {
        _handler = new GetReservationHistoryHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyReservationsWithToDateInPast()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S20");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Amy", "amy@ex.com", role);

        var pastFrom = DateTime.Today.AddDays(-5);
        var pastTo = DateTime.Today.AddDays(-3);
        var pastReservation = new Reservation(spot, user, pastFrom, pastTo);

        var futureFrom = DateTime.Today.AddDays(1);
        var futureTo = futureFrom.AddDays(1);
        var futureReservation = new Reservation(spot, user, futureFrom, futureTo);

        _mockRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { pastReservation, futureReservation });

        // Act
        var result = await _handler.Handle(new GetReservationHistoryQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value!.ToList();
        dtos.Count.Should().Be(1);
        dtos[0].Id.Should().Be(pastReservation.Id.ToString());
        dtos[0].From.Should().Be(pastFrom);
        dtos[0].To.Should().Be(pastTo);
    }
}