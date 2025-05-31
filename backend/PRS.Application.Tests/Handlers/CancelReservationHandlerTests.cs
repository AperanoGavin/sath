using FluentAssertions;

using Moq;

using PRS.Application.Commands;
using PRS.Application.Handlers;
using PRS.Domain.Entities;
using PRS.Domain.Enums;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class CancelReservationHandlerTests
{
    private readonly Mock<IReservationRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUow = new();
    private readonly CancelReservationHandler _handler;

    public CancelReservationHandlerTests()
    {
        _handler = new CancelReservationHandler(_mockRepo.Object, _mockUow.Object);
    }

    [Fact]
    public async Task Handle_WhenReservationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetAsync(missingId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _handler.Handle(new CancelReservationCommand(missingId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Should().BeOfType<ReservationNotFoundError>();
        ((ReservationNotFoundError)result.Error).Id.Should().Be(missingId);

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCancelFails_ShouldReturnThatError()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S2");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Alice", "a@a.com", role);
        var res = new Reservation(spot, user, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));

        // Force an initial cancellation
        res.Cancel().IsSuccess.Should().BeTrue();
        // Now status is Cancelled
        // Next Cancel() should fail with ReservationAlreadyCancelledError
        _mockRepo.Setup(r => r.GetAsync(res.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(res);

        // Act
        var result = await _handler.Handle(new CancelReservationCommand(res.Id), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Should().BeOfType<ReservationAlreadyCancelledError>();

        // Should never call UpdateAsync or SaveAsync in this failure path
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCancelSucceeds_ShouldUpdateAndSaveAndReturnSuccess()
    {
        // Arrange
        var spot = new Spot(Guid.NewGuid(), "S3");
        var role = new UserRole("Employee", "Employee", "desc");
        var user = new User(Guid.NewGuid(), "Bob", "b@b.com", role);
        var res = new Reservation(spot, user, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));

        _mockRepo.Setup(r => r.GetAsync(res.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(res);

        // Act
        var result = await _handler.Handle(new CancelReservationCommand(res.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        res.Status.Should().Be(ReservationStatus.Cancelled);

        _mockRepo.Verify(r => r.UpdateAsync(res, It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}