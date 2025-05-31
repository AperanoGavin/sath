using FluentAssertions;

using Moq;

using PRS.Application.Commands;
using PRS.Application.Handlers;
using PRS.Domain.Entities;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class CheckInReservationHandlerTests
{
    private readonly Mock<IReservationRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUow = new();
    private readonly CheckInReservationHandler _handler;

    public CheckInReservationHandlerTests()
    {
        _handler = new CheckInReservationHandler(_mockRepo.Object, _mockUow.Object);
    }

    [Fact]
    public async Task Handle_WhenReservationNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetAsync(missingId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _handler.Handle(new CheckInReservationCommand(missingId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ReservationNotFoundError>();
        ((ReservationNotFoundError)result.Error).Id.Should().Be(missingId);

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCheckInTooLate_ShouldReturnTooLateError()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S4");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Charlie", "c@c.com", role);
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var res = new Reservation(spot, user, yesterday, yesterday.AddDays(1));

        _mockRepo.Setup(r => r.GetAsync(res.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(res);

        // Act
        var result = await _handler.Handle(new CheckInReservationCommand(res.Id), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ReservationTooLateError>();

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task Handle_WhenCheckInBefore11AM_ShouldSucceed()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S5");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Dana", "d@d.com", role);

        var from = DateTime.UtcNow.Date.AddDays(1);
        var to = from.AddDays(1);
        var reservation = new Reservation(spot, user, from, to);

        _mockRepo
            .Setup(r => r.GetAsync(reservation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        // Act
        var result = await _handler.Handle(new CheckInReservationCommand(reservation.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepo.Verify(r => r.UpdateAsync(reservation, It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}