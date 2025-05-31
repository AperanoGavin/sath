using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetReservationByIdHandlerTests
{
    private readonly Mock<IReservationRepository> _mockRepo = new();
    private readonly GetReservationByIdHandler _handler;

    public GetReservationByIdHandlerTests()
    {
        _handler = new GetReservationByIdHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnError()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetAsync(missingId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _handler.Handle(new GetReservationByIdQuery(missingId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ReservationNotFoundError>();
        ((ReservationNotFoundError)result.Error).Id.Should().Be(missingId);
    }

    [Fact]
    public async Task Handle_WhenFound_ShouldReturnDto()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S30");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Eve", "eve@ex.com", role);
        var from = DateTime.Today;
        var to = from.AddDays(1);
        var reservation = new Reservation(spot, user, from, to);

        _mockRepo.Setup(r => r.GetAsync(reservation.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(reservation);

        // Act
        var result = await _handler.Handle(new GetReservationByIdQuery(reservation.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value;
        dto.Id.Should().Be(reservation.Id.ToString());
        dto.SpotId.Should().Be(spot.Id.ToString());
        dto.UserId.Should().Be(user.Id.ToString());
        dto.From.Should().Be(from);
        dto.To.Should().Be(to);
    }
}